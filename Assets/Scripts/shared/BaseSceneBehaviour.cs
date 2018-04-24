using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneBehaviour : MonoBehaviour {

	public GameObject topPrefab;
	public GameObject lineRenderPrefab;
	public GameObject importantPrefab;
	public Material greenMaterial;
	public Material redMaterial;

	protected Printer m_printer;
	protected GameObject scenePolygon;

	protected void Start(){
		m_printer = this.GetComponent<Printer> ();
		scenePolygon = new GameObject ();
		ClassStart ();
	}

	protected void Update(){
		clickPressed ();
	}

	protected virtual void ClassStart(){
		Debug.Log ("Override ClassStart mate!");
	}

	protected virtual void clickPressed(){
		return;
	}
}
