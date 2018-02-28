using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry{

	public static float distance(Dot a, Dot b){
		return Mathf.Sqrt (Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
	}
}
