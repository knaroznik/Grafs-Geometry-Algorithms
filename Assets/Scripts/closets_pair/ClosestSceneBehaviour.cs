using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

	protected void createDot(){
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
		Stopwatch y = Stopwatch.StartNew ();
		Block b = MyClosestRec(sorted);
		y.Stop ();
		recursiveTime = y.Elapsed.TotalMilliseconds;
		return b;
	}

	private Block MyClosestRec(List<Dot> pointsByX){
		
		int count = pointsByX.Count;
		if (count <= 3) {
			return closest_points_BruteForce (pointsByX);
		}

		Dot midPoint = pointsByX[count/2];

		List<Dot> leftByX = pointsByX.Take(count/2).ToList();
		Block leftResult = MyClosestRec(leftByX);

		var rightByX = pointsByX.Skip(count/2).ToList();
		var rightResult = MyClosestRec(rightByX);

		var result = rightResult.Length() < leftResult.Length() ? rightResult : leftResult;

		List<Dot> strip = new List<Dot> ();
		int j = 0;
		for (int i = 0; i < count; i++) {
			if (Mathf.Abs (pointsByX [i].x - midPoint.x) < result.Length ()) {
				strip.Add(pointsByX [i]);
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
		List<Dot> stripY = strip.OrderBy (p => p.y).ToList ();
		for (int i = 0; i < size; ++i) {
			for (int j = i + 1; j < size && (stripY [j].y - stripY [i].y) < min; ++j) {
				if (Geometry.distance (stripY [i], stripY [j]) < min) {
					output = new Block (stripY [i], stripY [j]);
					min = Geometry.distance (stripY [i], stripY [j]);
				}
			}
		}

		return output;
	}
}
