using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneBehaviour : MonoBehaviour {

	public List<GameObject> polygons;
	public GameObject buttonPrefab;

	public Transform canvasPanel;

	private GameObject currentPolygon;

	void Start(){
		for (int i = 0; i < polygons.Count; i++) {
			GameObject obj = Instantiate (buttonPrefab, canvasPanel) as GameObject;
			obj.GetComponentInChildren<Text> ().text = polygons [i].name;
			Button but = obj.GetComponent<Button> ();
			GameObject x = polygons[i];
			but.onClick.AddListener (delegate {
				if(currentPolygon != null){
					currentPolygon.GetComponent<PolygonDrawer>().Clear();
				}
				currentPolygon = Instantiate(x) as GameObject;
			});
		}
	}
}
