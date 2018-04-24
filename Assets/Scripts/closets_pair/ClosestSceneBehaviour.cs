using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Diagnostics;

public class ClosestSceneBehaviour : PolygonDrawer {

	protected List<Dot> dots = new List<Dot> ();
	protected GameObject currentBlock;

	double bruteTime = 0;
	double recursiveTime = 0;

	public Text timeInfo;
	public Text blockInfo;

	private void Start(){
		scenePolygon = new GameObject ();
		scenePolygon.name = "ClosestPointProblem";
	}

	protected void Update(){
		clickPressed ();
	}

	protected void clickPressed(){
		if (Input.GetMouseButtonDown (0)) {
			if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5) {
				return;
			}
			createDot ();
		}
	}

	protected virtual void createDot(){
		Vector3 v = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, -Camera.main.transform.position.z));
		dots.Add (new Dot (v.x, v.y));
		DrawTop (v.x, v.y);
	}

	public void calculate(){
		Stopwatch x = Stopwatch.StartNew ();
		closest_points_BruteForce (dots);
		x.Stop ();
		bruteTime = x.Elapsed.TotalMilliseconds;


		Block b = MyClosestDivide ();


		if (currentBlock != null) {
			Destroy (currentBlock);
		}
		currentBlock = DrawLine (new Vector3(b.pointA.x, b.pointA.y, 0f), new Vector3(b.pointB.x, b.pointB.y, 0f), Color.white, 1f);
		blockInfo.text = "Closest points : " + b.pointA.ToString () + ", " + b.pointB.ToString ();
		timeInfo.text = "Brutal Time : " + bruteTime + " and cad time : " + recursiveTime;
	}

	public void generate(){
		for (int i = 0; i < 100; i++) {
			float x = Random.Range (-50f, 50f);
			float y = Random.Range (-50f, 50f);
			dots.Add (new Dot (x, y));
			DrawTop (x, y);
		}
	}

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

	public Block MyClosestDivide()
	{
		List<Dot> sorted = dots.OrderBy (p => p.x).ToList ();
		List<Dot> sortedY = dots.OrderBy (p => p.y).ToList ();
		Stopwatch y = Stopwatch.StartNew ();
		Block b = MyClosestRec(sorted, sortedY);
		y.Stop ();
		recursiveTime = y.Elapsed.TotalMilliseconds;
		return b;
	}

	private Block MyClosestRec(List<Dot> SX, List<Dot> SY){
		
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

		Block leftResult = MyClosestRec(leftByX, leftByY);
		Block rightResult = MyClosestRec(rightByX, rightByY);

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

	Block stripClosest(List<Dot> strip, int size, float d)
	{
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
