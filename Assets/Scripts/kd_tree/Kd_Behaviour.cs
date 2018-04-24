using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Kd_Behaviour : MonoBehaviour {

	private List<DotWithName> Names = new List<DotWithName> ();
	private string output;
	private List<string> letters = new List<string>(){"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l"};
	private int currentLetter = 0;
	private GameObject scenePolygon;

	[SerializeField] private GameObject letterPrefab;
	[SerializeField] private Text sceneText;

	void Start(){
		scenePolygon = new GameObject ();
		scenePolygon.name = "ClosestPointProblem";
	}

	void Update(){
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

	public void buttonClicked(){
		Kd_Node tree = calculateTree (Names);
		output = "";
		PrintTree (tree, "");
		sceneText.text = output;
	}

	public Kd_Node calculateTree(List<DotWithName> dots){
		List<DotWithName> sortedX = dots.OrderBy (p => p.x).ToList ();
		List<DotWithName> sortedY = dots.OrderBy (p => p.y).ToList ();
		return makeTree (sortedX, sortedY, true);
	}

	public Kd_Node makeTree(List<DotWithName> SX,List<DotWithName> SY, bool xCheck){
		int count = SX.Count;
		Kd_Node node = new Kd_Node ();
		if (xCheck) {
			node.point = SX [SX.Count / 2];
		} else {
			node.point = SY [SY.Count / 2];
		}


		List<DotWithName> leftByX = new List<DotWithName> ();
		List<DotWithName> rightByX = new List<DotWithName> ();

		List<DotWithName> leftByY = new List<DotWithName> ();
		List<DotWithName> rightByY = new List<DotWithName> ();

		if (xCheck) {
			Geometry.SplitListByPointByX (node.point, SY, ref leftByY, ref rightByY);
			Geometry.SplitListByPointByX (node.point, SX, ref leftByX, ref rightByX);
		} else {
			Geometry.SplitListByPointByY (node.point, SY, ref leftByY, ref rightByY);
			Geometry.SplitListByPointByY (node.point, SX, ref leftByX, ref rightByX);
		}

		if (rightByX.Count > 0) {
			node.right = makeTree (rightByX, rightByY, !xCheck);
		} else {
			node.right = null;
		}
		if (leftByX.Count > 0) {
			node.left = makeTree (leftByX, leftByY, !xCheck);
		} else {
			node.left = null;
		}
		return node;
	}

	public void PrintTree(Kd_Node tree, string indent)
	{
		if (tree == null) {
			return;
		}
		output+=indent + "+- " + tree.point.alias + "\n";
		indent += "|  ";
		PrintTree(tree.left, indent);
		PrintTree(tree.right, indent);
	}

	protected virtual void createDot ()
	{
		Vector3 v = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, -Camera.main.transform.position.z));
		Names.Add (new DotWithName (v.x, v.y, letters [currentLetter]));
		createLetter (v.x, v.y);
	}

	protected virtual void createLetter(float x, float y){
		GameObject letter = Instantiate (letterPrefab, new Vector3 (x, y, 0f), Quaternion.identity, scenePolygon.transform) as GameObject;
		letter.GetComponentInChildren<Text> ().text = letters [currentLetter];
		currentLetter++;
	}

}
