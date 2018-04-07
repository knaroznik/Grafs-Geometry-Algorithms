using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Kd_Behaviour : MonoBehaviour {

	public List<Dot> input = new List<Dot> ();
	public Text sceneText;
	private string output;

	void Start(){
		Kd_Node tree = calculateTree (input);
		output = "";
		PrintTree (tree, "");
		sceneText.text = output;
	}

	public Kd_Node calculateTree(List<Dot> dots){
		List<Dot> sortedX = dots.OrderBy (p => p.x).ToList ();
		List<Dot> sortedY = dots.OrderBy (p => p.y).ToList ();
		return makeTree (sortedX, sortedY, true);
	}

	public Kd_Node makeTree(List<Dot> SX,List<Dot> SY, bool xCheck){
		int count = SX.Count;
		Kd_Node node = new Kd_Node ();
		if (xCheck) {
			Debug.Log ("X");
			node.point = SX [SX.Count / 2];
		} else {
			Debug.Log ("Y");
			node.point = SY [SY.Count / 2];
		}


		List<Dot> leftByX = new List<Dot> ();
		List<Dot> rightByX = new List<Dot> ();

		List<Dot> leftByY = new List<Dot> ();
		List<Dot> rightByY = new List<Dot> ();

		if (xCheck) {
			Geometry.SplitListByPointByX (node.point, SY, ref leftByY, ref rightByY);
			Geometry.SplitListByPointByX (node.point, SX, ref leftByX, ref rightByX);
		} else {
			Geometry.SplitListByPointByY (node.point, SY, ref leftByY, ref rightByY);
			Geometry.SplitListByPointByY (node.point, SX, ref leftByX, ref rightByX);
		}

		Debug.Log (leftByX.Count +  " " + rightByX.Count);

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
		output+=indent + "+- " + tree.point.ToString() + "\n";
		indent += "|  ";
		PrintTree(tree.left, indent);
		PrintTree(tree.right, indent);
	}
}
