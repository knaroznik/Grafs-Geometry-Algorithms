using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry{

	public static float distance(Dot a, Dot b){
		return Mathf.Sqrt (Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
	}

	public static Dot pointofIntersection(Block one, Block two){
		Dot a = one.pointA;
		Dot b = one.pointB;
		Dot c = two.pointA;
		Dot d = two.pointB;
		return new Dot (
			(((b.x - a.x) * (d.x * c.y - d.y * c.x) - (d.x - c.x) * (b.x * a.y - b.y * a.x)) / ((b.y - a.y) * (d.x - c.x) - (d.y - c.y) * (b.x - a.x))),
			(((d.y - c.y) * (b.x * a.y - b.y * a.x) - (b.y - a.y) * (d.x * c.y - d.y * c.x)) / ((d.y - c.y) * (b.x - a.x) - (b.y - a.y) * (d.x - c.x)))
		);
	}
}
