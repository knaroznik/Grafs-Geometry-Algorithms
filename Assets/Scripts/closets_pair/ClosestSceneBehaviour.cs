using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Diagnostics;

public class ClosestSceneBehaviour : BaseSceneBehaviour {

	/// <summary>
	/// List keeping positions of Points for calculations.
	/// </summary>
	protected List<Dot> dots = new List<Dot> ();

	/// <summary>
	/// Block on scene, showing pair of closest points
	/// </summary>
	protected GameObject currentBlock;

	/// <summary>
	/// Keeping brute Time.
	/// </summary>
	double bruteTime = 0;

	/// <summary>
	/// Keeping CAD Time.
	/// </summary>
	double recursiveTime = 0;

	[Header("Texts")]
	[SerializeField] private Text txt_TimeInfo;
	[SerializeField] private Text txt_BlockInfo;

	/// <summary>
	/// Just naming scenePolygon.
	/// </summary>
	protected override void ClassStart ()
	{
		scenePolygon.name = "ClosestPointProblem";
	}

	/// <summary>
	/// Action after clicking mouse.
	/// </summary>
	protected override void clickPressed(){
		if (Input.GetMouseButtonDown (0)) {
			if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5) {
				return;
			}
			Vector3 mousePositionFixed = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, -Camera.main.transform.position.z));
			dots.Add (new Dot (mousePositionFixed.x, mousePositionFixed.y));
			m_printer.PrintObject(topPrefab, mousePositionFixed.x, mousePositionFixed.y, scenePolygon.transform);
		}
	}

	/// <summary>
	/// Calculate brute algorithm and CAD algorithm, nextly showing it to user.
	/// </summary>
	public void Calculate(){
		if (dots.Count < 2) {
			return;
		}

		//Brute Time
		Stopwatch x = Stopwatch.StartNew ();
		closest_points_BruteForce (dots);
		x.Stop ();
		bruteTime = x.Elapsed.TotalMilliseconds;

		//CAD Time
		Block b = closest_points_CAD_Start ();

		//Updating/Creating line
		if (currentBlock != null) {
			Destroy (currentBlock);
		}
		currentBlock = m_printer.PrintLine (lineRenderPrefab, scenePolygon.transform, new Vector3 (b.pointA.x, b.pointA.y, 0f), new Vector3 (b.pointB.x, b.pointB.y, 0f), redMaterial, 1f, false);

		//Showing Times and Block info.
		txt_BlockInfo.text = "Closest points : " + b.pointA.ToString () + ", " + b.pointB.ToString ();
		txt_TimeInfo.text = "Brutal Time : " + bruteTime + " and CAD time : " + recursiveTime;
	}

	/// <summary>
	/// Generating 100 points on scene.
	/// </summary>
	public void Generate_Points(){
		for (int i = 0; i < 100; i++) {
			float x = Random.Range (-50f, 50f);
			float y = Random.Range (-50f, 50f);
			dots.Add (new Dot (x, y));
			m_printer.PrintObject(topPrefab, x, y, scenePolygon.transform);
		}
	}

	/// <summary>
	/// Closest Point by brute algorithm.
	/// </summary>
	/// <returns>Block of two closest points</returns>
	/// <param name="input">Starting list of points</param>
	private Block closest_points_BruteForce(List<Dot> input){
		int n = input.Count;

		if (n <= 1) {
			return null;
		}
		Block result = Enumerable.Range( 0, n-1)
			.SelectMany( i => Enumerable.Range( i+1, n-(i+1) )
				.Select( j => new Block( input[i], input[j] )))
			.OrderBy( seg => seg.LengthSquared())
			.First();
		return result;
	}

	/// <summary>
	/// Starting CAD algorithm.
	/// </summary>
	/// <returns>Block of two closest points</returns>
	private Block closest_points_CAD_Start()
	{
		List<Dot> sorted = dots.OrderBy (p => p.x).ToList ();
		List<Dot> sortedY = dots.OrderBy (p => p.y).ToList ();
		Stopwatch y = Stopwatch.StartNew ();
		Block b = closest_points_CAD_Recursive(sorted, sortedY);
		y.Stop ();
		recursiveTime = y.Elapsed.TotalMilliseconds;
		return b;
	}

	/// <summary>
	/// Recursive function of CAD algorithm.
	/// </summary>
	/// <returns>Block of two closest points</returns>
	/// <param name="SX">Points sorted by X</param>
	/// <param name="SY">Points sorted by Y</param>
	private Block closest_points_CAD_Recursive(List<Dot> SX, List<Dot> SY){
		int count = SX.Count;
		if (count <= 3) {
			return closest_points_BruteForce (SX);
		}
		Dot midPoint = SX[count/2];
		List<Dot> leftByX = SX.Take(count/2).ToList();
		List<Dot> rightByX = SX.Skip(count/2).ToList();
		List<Dot> leftByY = new List<Dot> ();
		List<Dot> rightByY = new List<Dot> ();
		Geometry.SplitListByPoint (midPoint, SY, ref leftByY, ref rightByY);
		Block leftResult = closest_points_CAD_Recursive(leftByX, leftByY);
		Block rightResult = closest_points_CAD_Recursive(rightByX, rightByY);
		var result = rightResult.Length() < leftResult.Length() ? rightResult : leftResult;
		List<Dot> strip = new List<Dot> ();
		int j = 0;
		for (int i = 0; i < count; i++) {
			if (Mathf.Abs (SY [i].x - midPoint.x) < result.Length ()) {
				strip.Add(SY [i]);
				j++;
			}
		}
		Block a = stripClosest (strip, j, result.Length ());
		Block b = result;
		if (a.Length () > b.Length()) {
			return b;
		} else {
			return a;
		}
	}

	/// <summary>
	/// Checking if current block is shortest.
	/// </summary>
	/// <returns>The closest.</returns>
	/// <param name="strip">List of dots</param>
	/// <param name="size">Size of strip</param>
	/// <param name="d">D.</param>
	private Block stripClosest(List<Dot> strip, int size, float d){
		float min = d;
		Block output = new Block(new Dot(999,999), new Dot(-999, -999));
		for (int i = 0; i < size; ++i) {
			for (int j = i + 1; j < size && (strip [j].y - strip [i].y) < min; ++j) {
				if (Geometry.distance (strip [i], strip [j]) < min) {
					output = new Block (strip [i], strip [j]);
					min = Geometry.distance (strip [i], strip [j]);
				}
			}
		}

		return output;
	}
}
