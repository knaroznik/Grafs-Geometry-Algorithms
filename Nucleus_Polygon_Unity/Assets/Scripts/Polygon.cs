using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {

	#region Starting : 

	public List<UnityDot> tops;
	private List<Dot> topsAsDots;
	protected PolygonDrawer drawer;

	protected Dot max;
	protected Dot min;

	public List<Dot> circuit = new List<Dot> ();

	protected Dot x;
	protected Dot y;
	protected Dot g;
	protected Dot z;

	protected bool maxInPolygon;
	protected bool minInPolygon;
	protected bool minIsOnePoint = false;
	protected bool maxIsOnePoint = false;

	protected Dot getDot(int i){
		return tops [(i % tops.Count + tops.Count)%tops.Count] as Dot;
	}
		
	public Polygon(PolygonDrawer drawer, List<Dot> dots){
		this.drawer = drawer;
		tops = new List<UnityDot> ();
		topsAsDots = dots;
		for (int i = 0; i < dots.Count; i++) {
			tops.Add (new UnityDot (dots [i].x, dots [i].y, drawer.DrawTop (dots [i].x, dots [i].y)));
			if(i==dots.Count-1){
				drawer.DrawLine (new Vector3 (dots [i].x, dots [i].y, 0f), new Vector3 (dots [0].x, dots [0].y, 0f), Color.white, 1f);
			}
			else{
				drawer.DrawLine (new Vector3 (dots [i].x, dots [i].y, 0f), new Vector3 (dots [(i+1)].x, dots [(i+1)].y, 0f), Color.white, 1f);
			}
		}
	}

	protected bool isInside(Dot point)
	{
		int polygonLength = tops.Count, i=0;
		bool inside = false;
		float pointX = point.x, pointY = point.y;
		float startX, startY, endX, endY;
		Dot endPoint = tops[polygonLength-1];           
		endX = endPoint.x; 
		endY = endPoint.y;
		while (i<polygonLength) {
			startX = endX;           startY = endY;
			endPoint = tops[i++];
			endX = endPoint.x;       endY = endPoint.y;
			inside ^= ( endY > pointY ^ startY > pointY )
				&&
				( (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY) ) ;
		}
		return inside;
	}

	public bool HaveKernel(){
		calculateMax ();
		calculateMin ();

		if (max.y > min.y) {
			return false;
		}
		return true;
	}

	protected void calculateMax(){
		maxInPolygon = false;
		for (int i = 0; i < tops.Count; i++) {
			Dot center = tops [i];
			Dot prev = getDot (i - 1);
			Dot next = getDot (i + 1);

			if (prev.y < center.y && next.y < center.y) {
				if(isInside(new Dot(center.x, center.y + 0.1f))){
					if (max == null)
						max = center;
					if (center.y > max.y) {
						max = center;
					}
				}
			}
		}

		if (max == null) {
			max = tops [0];
			for (int i = 1; i < tops.Count; i++) {
				if (tops [i].y < max.y) {
					max = tops [i];
				}
			}
			maxInPolygon = true;
		}
		drawer.DrawImportantObject (max.x, max.y);
	}

	protected void calculateMin(){
		minInPolygon = false;
		for (int i = 0; i < tops.Count; i++) {
			Dot center = tops [i];
			Dot prev = getDot (i - 1);
			Dot next = getDot (i + 1);

			if (prev.y > center.y && next.y > center.y) {
				if(isInside(new Dot(center.x, center.y - 0.1f))){
					if (min == null)
						min = center;
					if (center.y < min.y) {
						min = center;
					}
				}
			}
		}

		if (min == null) {
			minInPolygon = true;
			min = tops [0];
			for (int i = 1; i < tops.Count; i++) {
				if (tops [i].y > min.y) {
					min = tops [i];
				}
			}
		}

		drawer.DrawImportantObject (min.x, min.y);
	}

	#endregion

	#region Circuit : 

	public void calculateCircuit(){
		if (minInPolygon && maxInPolygon) {
			drawer.SetCircuitText (calculateCircuit(tops));
			return;
		}

		if (minInPolygon) {
			findMinPointsOnPolygon ();
			calculatePoints ();
			return;
		}

		if (maxInPolygon) {
			findMaxPointsOnPolygon ();
			calculatePoints ();
			return;
		}

		calculatePoints ();
		drawer.SetCircuitText (1);
	}
		
	protected void calculatePoints(){

		Dot leftUp = new Dot(0,0);
		Dot leftDown = new Dot(0,0);
		Dot rightUp = new Dot (0, 0);
		Dot rightDown = new Dot (0, 0);

		if (y == null && !maxIsOnePoint) {
			Dot up = new Dot(999,999);
			Dot down = new Dot(999,999);
			for (int i = 0; i < tops.Count; i++) {
				if (tops [i].y > max.y && (Mathf.Abs(tops[i].y-max.y)) < (Mathf.Abs(up.y-max.y)) && tops[i].x >= max.x) {
					up = tops [i];
				}

				if (tops [i].y < max.y && Geometry.distance(tops[i], max) < Geometry.distance(down, max) && tops[i].x >= max.x) {
					down = tops [i];
				}
			}
			x = Geometry.pointofIntersection (new Block (up, down), new Block (min, new Dot (max.x + 1, max.y)));
			rightDown = up;
			drawer.DrawImportantObject (y.x, y.y);
		}

		if (x == null) {
			Dot up = new Dot(999,999);
			Dot down = new Dot(999,999);
			for (int i = 0; i < tops.Count; i++) {
				if (tops [i].y > max.y && (Mathf.Abs(tops[i].y-max.y)) < (Mathf.Abs(up.y-max.y)) && tops[i].x <= max.x) {
					up = tops [i];
				}

				if (tops [i].y < max.y && (Mathf.Abs(tops[i].y-max.y)) < (Mathf.Abs(up.y-max.y)) && tops[i].x <= max.x) {
					down = tops [i];
				}
			}
			x = Geometry.pointofIntersection (new Block (up, down), new Block (min, new Dot (max.x + 1, max.y)));
			leftDown = up;
			drawer.DrawImportantObject (x.x, x.y);
		}

		if (z == null && !minIsOnePoint){
			Dot up = new Dot(-999,-999);
			Dot down = new Dot(999,999);
			for (int i = 0; i < tops.Count; i++) {
				if (tops [i].y > min.y && (Mathf.Abs(tops[i].y-min.y)) < (Mathf.Abs(up.y-min.y)) && tops[i].x > min.x) {
					up = tops [i];
				}

				if (tops [i].y < min.y && (Mathf.Abs(Mathf.Abs(tops[i].y)-Mathf.Abs(min.y))) < (Mathf.Abs((Mathf.Abs(up.y)-Mathf.Abs(min.y)))) && tops[i].x > min.x) {
					down = tops [i];
				}
			}
			z = Geometry.pointofIntersection (new Block (up, down), new Block (min, new Dot (min.x + 1, min.y)));
			rightUp = down;
			drawer.DrawImportantObject (z.x, z.y);
		}

		if (g == null) {
			Dot up = new Dot(-999,-999);
			Dot down = new Dot(999,999);
			for (int i = 0; i < tops.Count; i++) {
				if (tops [i].y > min.y && (Mathf.Abs(tops[i].y-min.y)) < (Mathf.Abs(up.y-min.y)) && tops[i].x <= min.x) {
					up = tops [i];
				}

				if (tops [i].y < min.y && (Mathf.Abs(tops[i].y-min.y)) < (Mathf.Abs(up.y-min.y)) && tops[i].x <= min.x) {
					down = tops [i];
				}
			}

			if (up == down) {
				g = up;
			} else {
				g = Geometry.pointofIntersection (new Block (new Dot (min.x + 1, min.y), min), new Block (down, up));
			}
			leftUp = down;
			drawer.DrawImportantObject (g.x, g.y);
		}
			
		circuit.Add (g);
		if (!minIsOnePoint) {
			circuit.Add (z);
		}

		if (!minInPolygon) {
			int index = topsAsDots.FindIndex (x=> x.x ==rightUp.x && x.y == rightUp.y);
			int index2 = topsAsDots.FindIndex (x=> x.x ==rightDown.x && x.y == rightDown.y);
			Debug.Log (rightUp);
			circuit.Add (rightUp);
			for (int i = index; i < index2; i++) {
				if (i + 1 < index2) {
					drawer.DrawLine (tops [i], tops [i + 1], Color.red, 2f);
				}
				circuit.Add (tops [i]);
			}

			circuit.Add (rightDown);
		} else {
			int index = topsAsDots.FindIndex (x=> x.x ==g.x && x.y == g.y);
			int index2 = topsAsDots.FindIndex (x=> x.x ==z.x && x.y == z.y);
			for (int i = index; i < index2; i++) {
				circuit.Add (tops [i]);
			}
		}
	
		if (!minIsOnePoint)
			circuit.Add (y);

		circuit.Add (x);

		if (!maxInPolygon) {
			int index = topsAsDots.FindIndex (x=> x.x ==leftDown.x && x.y == leftDown.y);
			int index2 = topsAsDots.FindIndex (x=> x.x ==leftUp.x && x.y == leftUp.y);

			circuit.Add (leftDown);
			for (int i = index; i < index2; i++) {
				circuit.Add (tops [i]);
			}
			circuit.Add (leftUp);
		} else {
			int index = topsAsDots.FindIndex(x=> x.x ==y.x && x.y == y.y);
			int index2 = topsAsDots.FindIndex (q=> q.x ==x.x && q.y == x.y);

			for (int i = index; i < index2; i++) {
				circuit.Add (tops [i]);
			}
		}
		drawer.debug = circuit;
		drawer.SetCircuitText (calculateCircuit (circuit));
	}

	protected void findMinPointsOnPolygon(){
		List<Dot> search = new List<Dot> ();

		for (int i = 0; i < tops.Count; i++) {
			if (tops [i].y == min.y) {
				search.Add (tops [i]);
			}
		}

		if (search.Count == 1) {
			g = search [0];
			minIsOnePoint = true;
			return;
		}


		Dot q;
		Dot w;
		float prev_count = 0;

		for (int i = 0; i < search.Count; i++) {
			for (int j = 0; j < search.Count; j++) {
				q = search [i];
				w = search [j];
				if (Geometry.distance (q, w) > prev_count) {
					g = q;
					z = w;
					prev_count = Geometry.distance (q, w);
				}
			}
		}

		if (g.x > z.x) {
			Dot temp = g;
			g = z;
			z = temp;
		}

		drawer.DrawImportantObject (g.x, g.y);
		drawer.DrawImportantObject (z.x, z.y);
	}

	protected void findMaxPointsOnPolygon(){
		List<Dot> search = new List<Dot> ();

		for (int i = 0; i < tops.Count; i++) {
			if (tops [i].y == max.y) {
				search.Add (tops [i]);
			}
		}

		if (search.Count == 1) {
			maxIsOnePoint = true;
			x = search [0];
			return;
		}

		Dot q;
		Dot w;
		float prev_count = 0;

		for (int i = 0; i < search.Count; i++) {
			for (int j = 0; j < search.Count; j++) {
				q = search [i];
				w = search [j];
				if (Geometry.distance (q, w) > prev_count) {
					x = q;
					y = w;
					prev_count = Geometry.distance (q, w);
				}
			}
		}

		if (x.x > y.x) {
			Dot temp = x;
			x = y;
			y = temp;
		}

		drawer.DrawImportantObject (x.x, x.y);
		drawer.DrawImportantObject (y.x, y.y);
	}

	protected float calculateCircuit(List<Dot> points){
		float output = 0;
		for (int i = 0; i < points.Count; i++) {
			output += Geometry.distance (points[(i % points.Count + points.Count)%points.Count], points[(i+1 % points.Count + points.Count)%points.Count]);
		}
		return output;
	}

	protected float calculateCircuit(List<UnityDot> points){
		float output = 0;
		for (int i = 0; i < points.Count; i++) {
			output += Geometry.distance (points[(i % points.Count + points.Count)%points.Count], points[(i+1 % points.Count + points.Count)%points.Count]);
		}
		return output;
	}

	#endregion
}
