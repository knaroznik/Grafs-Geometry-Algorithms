using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry{

	public static float distance(Dot a, Dot b){
		return Mathf.Sqrt (Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
	}

	public static float distanceSquared(Dot a, Dot b){
		return Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2);
	}

	public static float distance(float a_y, float b_y){
		return Mathf.Sqrt (Mathf.Pow(0 - 0, 2) + Mathf.Pow(b_y - a_y, 2));
	}

	public static Dot pointofIntersection(Block one, Block two){
		Dot x1 = one.pointA;
		Dot x2 = one.pointB;
		Dot x3 = two.pointA;
		Dot x4 = two.pointB;

		float a1 = (x2.y - x1.y) / (x2.x - x1.x);
		float c1 = x1.y - a1 * x1.x;

		float a2 = (x4.y - x3.y) / (x4.x - x3.x);
		float c2 = x3.y - a2 * x3.x;
		float b = -1f;

		float w = a1 * b - a2 * b;
		float wx = (-c1) * b - (-c2) * b;
		float wy = a1 * (-c2) - a2 * (-c1);

		return new Dot (wx / w, wy / w);
	}
}
