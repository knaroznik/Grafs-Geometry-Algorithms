using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {
	
	public List<UnityDot> tops;
	protected PolygonDrawer drawer;

	protected Dot getDot(int i){
		return tops [(i % tops.Count + tops.Count)%tops.Count] as Dot;
	}

	public Polygon(PolygonDrawer drawer, List<Dot> dots){
		this.drawer = drawer;
		tops = new List<UnityDot> ();
		Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.red);
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
		}

		drawer.DrawImportantObject (max.x, max.y);
	}

	protected void calculateMin(){
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
			min = tops [0];
			for (int i = 1; i < tops.Count; i++) {
				if (tops [i].y > min.y) {
					min = tops [i];
				}
			}
		}

		drawer.DrawImportantObject (min.x, min.y);
	}

	public void calculateCircuit(){
	}

	public void calculateArea(){

	}
}
