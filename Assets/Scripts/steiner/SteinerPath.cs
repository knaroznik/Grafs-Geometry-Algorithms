using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SteinerPath : MonoBehaviour {

	private List<Dot> dots = new List<Dot> ();
	private Printer m_printer;

	public GameObject Dot;
	public GameObject Line;
	public Material LineColor;

	void Start () {
		m_printer = this.GetComponent<Printer> ();
		dots.Add (new Dot(3, 2)); 
		dots.Add (new Dot(0, 0)); dots.Add (new Dot(1, 2)); dots.Add (new Dot(2, 4)); 
		dots.Add (new Dot(3, 1)); 
		dots.Add (new Dot(5, 2)); dots.Add (new Dot(4, 4));
		dots.Add (new Dot(4, 1));


		for (int i = 0; i < dots.Count; i++) {
			m_printer.PrintObject (Dot, new Vector3 (dots [i].x, dots [i].y, 0f), this.transform);
		}

		getPath ();
	}

	void getPath(){
		List<Dot> sortedX = new List<Dot> (dots); 
		List<Dot> sortedY = new List<Dot> (dots);

		sortedX.Sort (delegate(Dot x, Dot y) {
			int xdiff = x.x.CompareTo (y.x);
			if (xdiff != 0)
				return xdiff;
			else
				return x.y.CompareTo (y.y);
		});

		sortedY.Sort (delegate(Dot x, Dot y) {
			int ydiff = x.y.CompareTo (y.y);
			if (ydiff != 0)
				return ydiff;
			else
				return x.x.CompareTo (y.x);
		});

		int breakdown = 0;

		while (sortedX.Count != 2 && sortedY.Count != 2) {
			drawFromY (ref sortedX, ref sortedY);
			drawFromX (ref sortedX, ref sortedY);
			breakdown++;
			if (breakdown > 10000) {
				break;
			}
		}
		drawFromY (ref sortedX, ref sortedY);
	}

	void drawFromX(ref List<Dot> X, ref List<Dot> Y, bool debug = false){
		Dot x = X [X.Count-1];
		Dot y = X [X.Count-2];
		Dot newDot = new Dot (0, 0);
		if (y.x < x.x && y.y < x.y) {
			newDot = new Dot (x.x, y.y);
		} else {
			newDot = new Dot (Mathf.Min (x.x, y.x), Mathf.Min (x.y, y.y));

		}

		//Remove and add on X list
		X.RemoveAt (X.Count - 1);
		X.RemoveAt (X.Count - 1);
		X.Add (newDot);

		//Remove and add on Y list
		int XIndex = Y.IndexOf (x);
		int YIndex = Y.IndexOf (y);


		Y [Mathf.Min (XIndex, YIndex)] = newDot;
		Y.RemoveAt (Mathf.Max (XIndex, YIndex));

		//Draw

		m_printer.PrintLine (Line, this.transform, new Vector3 (x.x, x.y, 0f), new Vector3 (newDot.x, newDot.y, 0f), LineColor, 0.05f, false);
		m_printer.PrintLine (Line, this.transform, new Vector3 (y.x, y.y, 0f), new Vector3 (newDot.x, newDot.y, 0f), LineColor, 0.05f, false);
		m_printer.PrintObject (Dot, new Vector3 (newDot.x, newDot.y, 0f), this.transform);
	}

	void drawFromY(ref List<Dot> X, ref List<Dot> Y){
		Dot x = Y [Y.Count-1];
		Dot y = Y [Y.Count-2];
		Dot newDot = new Dot (0,0);

		if (y.x < x.x && y.y < x.y) {
			newDot = new Dot (x.x, y.y);
		} else {
			newDot = new Dot (Mathf.Min (x.x, y.x), Mathf.Min (x.y, y.y));
		}

		//Remove and add on Y list
		Y.RemoveAt (Y.Count - 1);
		Y.RemoveAt (Y.Count - 1);
		Y.Add (newDot);

		//Remove and add on X list
		int XIndex = X.IndexOf (x);
		int YIndex = X.IndexOf (y);

		X [Mathf.Min (XIndex, YIndex)] = newDot;
		X.RemoveAt (Mathf.Max (XIndex, YIndex));

		//Draw

		m_printer.PrintLine (Line, this.transform, new Vector3 (x.x, x.y, 0f), new Vector3 (newDot.x, newDot.y, 0f), LineColor, 0.05f, false);
		m_printer.PrintLine (Line, this.transform, new Vector3 (y.x, y.y, 0f), new Vector3 (newDot.x, newDot.y, 0f), LineColor, 0.05f, false);
		m_printer.PrintObject (Dot, new Vector3 (newDot.x, newDot.y, 0f), this.transform);
	}

	private void DebugList(List<Dot> list){
		for (int i = 0; i < list.Count; i++) {
			Debug.Log (list [i].ToString ());
		}

	}
}
