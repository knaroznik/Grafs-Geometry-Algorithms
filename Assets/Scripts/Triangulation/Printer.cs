using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer : MonoBehaviour {

	public GameObject PrintObject(GameObject prefab, Transform parent){
		Vector3 v = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, -Camera.main.transform.position.z));
		return Instantiate (prefab, new Vector3 (v.x, v.y, 0f), Quaternion.identity, parent) as GameObject;
	}

	public GameObject PrintObject(GameObject prefab, Vector3 pos, Transform parent){
		return Instantiate (prefab, pos, Quaternion.identity, parent) as GameObject;
	}

	public GameObject PrintLine(GameObject prefab, Transform parent, Vector3 one, Vector3 two, Material color, float width, bool debug){
		if (debug) {
			Debug.Log ("Printing line from : " + one.ToString () + " to " + two.ToString ());
		}
		GameObject renderObj = Instantiate (prefab, parent) as GameObject;
		LineRenderer renderer = renderObj.GetComponent<LineRenderer> ();
		renderer.SetPosition (0, one);
		renderer.SetPosition (1, two);
		renderer.material = color;
		renderer.startWidth = width;
		renderer.endWidth = width;
		return renderObj;
	}
}
