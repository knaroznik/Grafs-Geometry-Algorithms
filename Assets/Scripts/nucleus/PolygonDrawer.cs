using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolygonDrawer : MonoBehaviour {

	public GameObject topPrefab;
	public GameObject lineRenderPrefab;
	public GameObject importantPrefab;

	protected GameObject scenePolygon;

	[SerializeField] private List<Dot> userTops;
	protected SceneBehaviour sceneBehaviour;

	// Use this for initialization
	private void Start () {
		scenePolygon = new GameObject ();
		scenePolygon.name = this.name + " polygon";
		sceneBehaviour = FindObjectOfType<SceneBehaviour> ();
		Polygon pol = new Polygon (this, userTops);

		bool hasKernel = pol.HaveKernel ();
		SetKernelText (hasKernel);

		if (hasKernel) {
			pol.calculateCircuit ();
		} else {
			SetCircuitText (-1);
			SetTopsText (-1);
		}
	}

	public GameObject DrawTop(float x, float y){
		return Instantiate (topPrefab, new Vector3 (x, y, 0f), Quaternion.identity, scenePolygon.transform) as GameObject;
	}

	public void DrawImportantObject(float x, float y){
		Instantiate (importantPrefab, new Vector3 (x, y, 0f), Quaternion.identity, scenePolygon.transform);
	}

	public GameObject DrawLine(Vector3 one, Vector3 two, Color color, float width){
		GameObject renderObj = Instantiate (lineRenderPrefab, scenePolygon.transform) as GameObject;
		LineRenderer renderer = renderObj.GetComponent<LineRenderer> ();
		renderer.SetPosition (0, one);
		renderer.SetPosition (1, two);
		renderer.startColor = color;
		renderer.endColor = color;

		renderer.startWidth = width;
		renderer.endWidth = width;
		return renderObj;
	}

	public void DrawLine(Dot one, Dot two, Color color, float width){
		GameObject renderObj = Instantiate (lineRenderPrefab, scenePolygon.transform) as GameObject;
		LineRenderer renderer = renderObj.GetComponent<LineRenderer> ();
		renderer.SetPosition (0, new Vector3(one.x, one.y, 0f));
		renderer.SetPosition (1, new Vector3(two.x, two.y, 0f));
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
			sceneBehaviour.KernelFound().GetComponent<Text> ().text = "Kernel found";
		} else {
			sceneBehaviour.KernelFound().GetComponent<Text> ().text = "Kernel not found";
		}
	}

	public void SetCircuitText(float circuit){
		sceneBehaviour.Circuit().GetComponent<Text> ().text = "Circuit : " + circuit;
	}

	public void SetTopsText(int tops){
		sceneBehaviour.TopsCount().GetComponent<Text> ().text = "Tops in circuit : " + tops;
	}
}
