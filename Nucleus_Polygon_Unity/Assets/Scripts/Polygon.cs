using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {
	
	public List<UnityDot> tops;

	public Polygon(PolygonDrawer drawer, List<Dot> dots){
		tops = new List<UnityDot> ();
		Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.red);
		for (int i = 0; i < dots.Count; i++) {
			tops.Add (new UnityDot (dots [i].x, dots [i].y, drawer.DrawTop (dots [i].x, dots [i].y)));
			if(i==dots.Count-1){
				drawer.DrawLine (new Vector3 (dots [i].x, dots [i].y, 0f), new Vector3 (dots [0].x, dots [0].y, 0f), Color.white, 0.1f);
			}
			else{
				drawer.DrawLine (new Vector3 (dots [i].x, dots [i].y, 0f), new Vector3 (dots [(i+1)].x, dots [(i+1)].y, 0f), Color.white, 0.1f);
			}
		}
	}

	public void findTop(){
		
	}

	public void findBot(){

	}

	public bool HaveKernel(){
		return false;
	}

	public void calculateCircuit(){
	}

	public void calculateArea(){

	}
}
