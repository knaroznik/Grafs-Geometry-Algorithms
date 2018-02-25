using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonDrawer : MonoBehaviour {

	public List<Dot> userTops;

	public GameObject topPrefab;
	public GameObject lineRenderPrefab;

	// Use this for initialization
	void Start () {
		Polygon pol = new Polygon (this, userTops);

		if (!pol.HaveKernel ()) {
			Debug.Log ("Nie ma jądra");
		} else {
			pol.calculateCircuit ();
			pol.calculateArea ();
		}
	}

	public GameObject DrawTop(float x, float y){
		return Instantiate (topPrefab, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
	}

	public void DrawLine(Vector3 one, Vector3 two, Color color, float width){
		GameObject renderObj = Instantiate (lineRenderPrefab) as GameObject;
		LineRenderer renderer = renderObj.GetComponent<LineRenderer> ();
		renderer.SetPosition (0, one);
		renderer.SetPosition (1, two);
		renderer.startColor = color;
		renderer.endColor = color;

		renderer.startWidth = width;
		renderer.endWidth = width;
	}
}
