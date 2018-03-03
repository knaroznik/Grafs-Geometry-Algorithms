using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneBehaviour : MonoBehaviour {

	public List<GameObject> polygons;
	public GameObject buttonPrefab;

	public Transform canvasPanel;

	private GameObject currentPolygon;

	GameObject obj;
	Button but;

	[Header("Texts")]

	public GameObject kernelFound;
	public GameObject circuit;
	public GameObject topsCount;

	void Start(){
		for (int i = 0; i < polygons.Count; i++) {
			obj = Instantiate (buttonPrefab, canvasPanel) as GameObject;
			obj.GetComponentInChildren<Text> ().text = polygons [i].name;
			but = obj.GetComponent<Button> ();
			GameObject x = polygons[i];
			but.onClick.AddListener (delegate {
				if(currentPolygon != null){
					currentPolygon.GetComponent<PolygonDrawer>().Clear();
				}
				currentPolygon = Instantiate(x) as GameObject;
			});
		}

		//Exit button : 

		obj = Instantiate (buttonPrefab, canvasPanel) as GameObject;
		obj.GetComponentInChildren<Text> ().text = "EXIT";
		but = obj.GetComponent<Button> ();
		but.onClick.AddListener (delegate {
			Application.Quit();
		});
	}
}
