using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class ClosestSceneBehaviour : PolygonDrawer {

	protected List<Dot> dots = new List<Dot> ();
	protected GameObject currentBlock;

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
		closest_points_BruteForce ();
	}

	private void closest_points_BruteForce(){
		int n = dots.Count;

		if (n <= 1) {
			return;
		}

		Block result = Enumerable.Range( 0, n-1)
			.SelectMany( i => Enumerable.Range( i+1, n-(i+1) )
				.Select( j => new Block( dots[i], dots[j] )))
			.OrderBy( seg => seg.LengthSquared())
			.First();
		if (currentBlock != null) {
			Destroy (currentBlock);
		}
		currentBlock = DrawLine (new Vector3(result.pointA.x, result.pointA.y, 0f), new Vector3(result.pointB.x, result.pointB.y, 0f), Color.white, 1f);
	}
}
