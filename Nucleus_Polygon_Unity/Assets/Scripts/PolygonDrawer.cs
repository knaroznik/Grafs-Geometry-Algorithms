using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonDrawer : MonoBehaviour {

	public List<Dot> userTops;

	public GameObject topPrefab;
	public GameObject lineRenderPrefab;
	public GameObject importantPrefab;

	protected GameObject scenePolygon;
	protected SceneBehaviour sceneBehaviour;

	// Use this for initialization
	void Start () {
		scenePolygon = new GameObject ();
		scenePolygon.name = this.name + " polygon";
		sceneBehaviour = FindObjectOfType<SceneBehaviour> ();
		Polygon pol = new Polygon (this, userTops);

		bool hasKernel = pol.HaveKernel ();
		SetKernelText (hasKernel);

		if (hasKernel) {
			pol.calculateCircuit ();
			pol.calculateArea ();
		} else {
			SetAreaText (-1);
			SetCircuitText (-1);
		}
	}

	public GameObject DrawTop(float x, float y){
		return Instantiate (topPrefab, new Vector3 (x, y, 0f), Quaternion.identity, scenePolygon.transform) as GameObject;
	}

	public void DrawImportantObject(float x, float y){
		Instantiate (importantPrefab, new Vector3 (x, y, 0f), Quaternion.identity, scenePolygon.transform);
	}

	public void DrawLine(Vector3 one, Vector3 two, Color color, float width){
		GameObject renderObj = Instantiate (lineRenderPrefab, scenePolygon.transform) as GameObject;
		LineRenderer renderer = renderObj.GetComponent<LineRenderer> ();
		renderer.SetPosition (0, one);
		renderer.SetPosition (1, two);
		renderer.startColor = color;
		renderer.endColor = color;

		renderer.startWidth = width;
		renderer.endWidth = width;
	}

	public void Clear(){
		Destroy (scenePolygon);
		Destroy (this.gameObject);
	}


	public void SetKernelText(bool _hasKernel){
		if (_hasKernel) {
			sceneBehaviour.kernelFound.GetComponent<Text> ().text = "Kernel found";
		} else {
			sceneBehaviour.kernelFound.GetComponent<Text> ().text = "Kernel not found";
		}
	}

	public void SetCircuitText(float circuit){
		sceneBehaviour.circuit.GetComponent<Text> ().text = "Circuit : " + circuit;
	}

	public void SetAreaText(float area){
		sceneBehaviour.area.GetComponent<Text> ().text = "Area : " + area;
	}
}
