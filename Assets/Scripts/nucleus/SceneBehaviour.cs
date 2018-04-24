using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneBehaviour : MonoBehaviour {

	[SerializeField] private List<GameObject> polygons;
	[SerializeField] private GameObject buttonPrefab;

	[SerializeField] private Transform canvasPanel;

	private GameObject currentPolygon;

	private GameObject buttonObject;
	private Button buttonComponent;

	[Header("Texts")]
	[SerializeField] private GameObject kernelFound;
	[SerializeField] private GameObject circuit;
	[SerializeField] private GameObject topsCount;

	void Start(){
		InstantiateButtons ();
	}

	private void InstantiateButtons(){
		for (int i = 0; i < polygons.Count; i++) {
			buttonObject = Instantiate (buttonPrefab, canvasPanel) as GameObject;
			buttonObject.GetComponentInChildren<Text> ().text = polygons [i].name;
			buttonComponent = buttonObject.GetComponent<Button> ();
			GameObject x = polygons[i];
			buttonComponent.onClick.AddListener (delegate {
				if(currentPolygon != null){
					currentPolygon.GetComponent<PolygonDrawer>().Clear();
				}
				currentPolygon = Instantiate(x) as GameObject;
			});
		}
	}

	public GameObject KernelFound(){
		return kernelFound;
	}

	public GameObject Circuit(){
		return circuit;
	}

	public GameObject TopsCount(){
		return topsCount;
	}
}
