using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SteinerPath : MonoBehaviour {

	private List<Dot> dots = new List<Dot> ();
	private Dot finalPoint;
	private Printer m_printer;

	public GameObject Dot;
	public GameObject Line;
	public Material LineColor;

	void Start () {
		m_printer = this.GetComponent<Printer> ();
		dots.Add (new Dot(6, 2)); 
		dots.Add (new Dot(4, 4)); 
		dots.Add (new Dot(5, 3)); 
		dots.Add (new Dot(3, 5)); 


		finalPoint = new Dot (0, 0);
		dots.Add (finalPoint);

		for (int i = 0; i < dots.Count; i++) {
			m_printer.PrintObject (Dot, new Vector3 (dots [i].x, dots [i].y, 0f), this.transform);
		}

		getPath ();
	}

	void getPath(){
		Line mainLine = getLineFromDots (finalPoint, new Dot (finalPoint.x + 1, finalPoint.y + 1));
		int breakdown = 0;

		while (dots.Count > 1) {

			//Point A -> point of Intersaction, Point B -> point from dots
			List<Block> sorted = new List<Block> ();

			foreach (Dot p in dots) {
				Dot pp = new Dot (p.x - 1, p.y + 1);
				sorted.Add (new Block(pointOfIntersaction(mainLine, getLineFromDots(p, pp)), p));
			}

			sorted.Sort (delegate(Block x, Block y) {

				float distance = Geometry.distance(finalPoint, x.pointA);
				float distance2 = Geometry.distance(finalPoint, y.pointA);

				int distDiff = distance2.CompareTo(distance);
				if(distDiff != 0){
					return distDiff;
				}else{
					int xdiff = x.pointB.x.CompareTo (y.pointB.x);
					return xdiff;
				}
			});

			Dot a = sorted[0].pointB;
			Dot b = sorted[1].pointB;
			Dot newDot = new Dot (0, 0);
			if (b.x < a.x && b.y < a.y) {
				newDot = new Dot (a.x, b.y);
			} else {
				newDot = new Dot (Mathf.Min (a.x, b.x), Mathf.Min (a.y, b.y));
			}

			m_printer.PrintLine (Line, this.transform, new Vector3 (a.x, a.y, 0f), new Vector3 (newDot.x, newDot.y, 0f), LineColor, 0.05f, false);
			m_printer.PrintLine (Line, this.transform, new Vector3 (b.x, b.y, 0f), new Vector3 (newDot.x, newDot.y, 0f), LineColor, 0.05f, false);
			m_printer.PrintObject (Dot, new Vector3 (newDot.x, newDot.y, 0f), this.transform);

			dots.Remove (a);
			dots.Remove (b);
			dots.Add (newDot);

			breakdown++;
			if (breakdown > 2000) {
				break;
			}
		}
	}

	private Line getLineFromDots(Dot A, Dot B){
		float slope = (B.y - A.y) / (B.x - A.x);
		float b = B.y - slope * B.x;
		return new Line (slope, b);
	}

	private Dot pointOfIntersaction(Line A, Line B){
		// Line A represented as a1x + b1y = c1
		//y = ax + b -> a1x - b1y = -c1
		float a1 = A.A;
		float b1 = -1;
		float c1 = -A.B;

		// Line B represented as a2x + b2y = c2
		//y = ax + b -> a2x - b2y = -c2
		float a2 = B.A;
		float b2 = -1;
		float c2 = -B.B;

		float determinant = a1*b2 - a2*b1;

		if (determinant == 0)
		{
			// The lines are parallel. This is simplified
			// by returning a pair of FLT_MAX
			return null;
		}
		else
		{
			float x = (b2*c1 - b1*c2)/determinant;
			float y = (a1*c2 - a2*c1)/determinant;
			return new Dot(x, y);
		}
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

}
