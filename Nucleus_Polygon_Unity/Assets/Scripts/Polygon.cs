using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {

	#region Starting : 

	public List<UnityDot> tops;
	protected PolygonDrawer drawer;

	protected Dot getDot(int i){
		return tops [(i % tops.Count + tops.Count)%tops.Count] as Dot;
	}
		
	public Polygon(PolygonDrawer drawer, List<Dot> dots){
		this.drawer = drawer;
		tops = new List<UnityDot> ();
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

	protected Dot max;
	protected Dot min;

	protected bool maxInPolygon;
	protected bool minInPolygon;

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
			drawer.SetCircuitText (circuit(tops));
			return;
		}

		if (minInPolygon) {
			findMinPointsOnPolygon ();
			calculatePoints ();
			drawer.SetCircuitText (100);
			return;
		}

		if (maxInPolygon) {
			findMaxPointsOnPolygon ();
			calculatePoints ();
			drawer.SetCircuitText (200);
			return;
		}

		calculatePoints ();
		drawer.SetCircuitText (1);
	}

	protected Dot x;
	protected Dot y;
	protected Dot g;
	protected Dot z;

	protected void calculatePoints(){
		if (x == null) {
			//TODO : znaleźć punkty najbliżej po prawej (min) i przecięcie prostej 
		}

		if (y == null) {
			//TODO : znaleźć punkty najbliżej po lewej (min) i przecięcie prostej 
		}

		if (g == null) {
			//TODO : znaleźć punkty najbliżej po prawej (max) i przecięcie prostej 
		}

		if (z == null) {
			//TODO : znaleźć punkty najbliżej po lewej (max) i przecięcie prostej 
		}

		//TODO : Połączyć wszystko ze sobą i przekazać do funkcji circuit
	}

	protected void findMinPointsOnPolygon(){
		List<Dot> search = new List<Dot> ();

		for (int i = 0; i < tops.Count; i++) {
			if (tops [i].y == min.y) {
				search.Add (tops [i]);
			}
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

	protected float circuit(List<Dot> points){
		float output = 0;
		for (int i = 0; i < points.Count; i++) {
			output += Geometry.distance (points[(i % points.Count + points.Count)%points.Count], points[(i+1 % points.Count + points.Count)%points.Count]);
		}
		return output;
	}

	protected float circuit(List<UnityDot> points){
		float output = 0;
		for (int i = 0; i < points.Count; i++) {
			output += Geometry.distance (points[(i % points.Count + points.Count)%points.Count], points[(i+1 % points.Count + points.Count)%points.Count]);
		}
		return output;
	}

	#endregion

	#region Area : 

	public void calculateArea(){

	}

	#endregion
}
