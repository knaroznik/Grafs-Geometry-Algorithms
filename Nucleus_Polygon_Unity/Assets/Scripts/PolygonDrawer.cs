﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonDrawer : MonoBehaviour {

	public List<Dot> userTops;

	public GameObject topPrefab;
	public GameObject lineRenderPrefab;

	// Use this for initialization
	void Start () {
		Polygon pol = new Polygon (this, userTops);
		pol.findBot ();
		pol.findTop ();

		if (pol.HaveKernel ()) {
			Debug.Log ("Nie ma jądra");
		} else {
			pol.calculateCircuit ();
			pol.calculateArea ();
		}
	}

	public GameObject DrawTop(int x, int y){
		return Instantiate (topPrefab, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
	}

	public void DrawLine(Vector3 one, Vector3 two){
		GameObject renderObj = Instantiate (lineRenderPrefab) as GameObject;
		LineRenderer renderer = renderObj.GetComponent<LineRenderer> ();
		renderer.SetPosition (0, one);
		renderer.SetPosition (1, two);
	}
}
