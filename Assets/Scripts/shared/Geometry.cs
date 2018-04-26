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

	public static void SplitListByPoint(Dot splitPoint, List<Dot> SY, ref List<Dot> left, ref List<Dot> right){
		for (int i = 0; i < SY.Count; i++) {
			if (SY [i].x < splitPoint.x) {
				left.Add (SY [i]);
			} else {
				right.Add (SY [i]);
			}
		}
	}

	public static void SplitListByPointByX(Dot splitPoint, List<Dot> SY, ref List<Dot> left, ref List<Dot> right){
		for (int i = 0; i < SY.Count; i++) {
			if (SY [i] == splitPoint) {
				continue;
			}

			if (SY [i].x < splitPoint.x) {
				left.Add (SY [i]);
			} else {
				right.Add (SY [i]);
			}
		}
	}

	public static void SplitListByPointByY(Dot splitPoint, List<Dot> SX, ref List<Dot> left, ref List<Dot> right){
		for (int i = 0; i < SX.Count; i++) {
			if (SX [i] == splitPoint) {
				continue;
			}

			if (SX [i].y < splitPoint.y) {
				left.Add (SX [i]);
			} else {
				right.Add (SX [i]);
			}
		}
	}

	public static void SplitListByPointByX(DotWithName splitPoint, List<DotWithName> SY, ref List<DotWithName> left, ref List<DotWithName> right){
		for (int i = 0; i < SY.Count; i++) {
			if (SY [i] == splitPoint) {
				continue;
			}

			if (SY [i].x < splitPoint.x) {
				left.Add (SY [i]);
			} else {
				right.Add (SY [i]);
			}
		}
	}

	public static void SplitListByPointByY(DotWithName splitPoint, List<DotWithName> SX, ref List<DotWithName> left, ref List<DotWithName> right){
		for (int i = 0; i < SX.Count; i++) {
			if (SX [i] == splitPoint) {
				continue;
			}

			if (SX [i].y < splitPoint.y) {
				left.Add (SX [i]);
			} else {
				right.Add (SX [i]);
			}
		}
	}

	public static void DebugListY(List<Dot> SY){
		string output = string.Empty;
		for (int i = 0; i < SY.Count; i++) {
			output += SY [i].y.ToString();
			output += " ";
		}
		Debug.Log (output);
	}

	/// <summary>
	///	Checking if line is inside polygon.
	/// </summary>
	/// <returns><c>true</c>, if line inside polygon, <c>false</c> otherwise.</returns>
	/// <param name="polygon">Polygon.</param>
	/// <param name="isClosed">If set to <c>true</c> is closed.</param>
	/// <param name="A">A.</param>
	/// <param name="B">B.</param>
	public static bool LineInsidePolygon(List<GameObject> polygon, bool isClosed, GameObject A, GameObject B){
		//Check if polygon is closed, otherwise we close it.
		if (!isClosed)
			polygon.Add (polygon [0]);
		if (A == null || B == null) {
			return false;
		}

		Block unknownLine = new Block (new Dot (A.transform.position.x, A.transform.position.y),
			                    new Dot (B.transform.position.x, B.transform.position.y));
		for (int i = 1; i < polygon.Count; i++) {
			Block line_in_polygon = new Block (
				new Dot(polygon[i-1].transform.position.x, polygon[i-1].transform.position.y), 
				new Dot(polygon[i].transform.position.x, polygon[i].transform.position.y));

			if (LinesInterSect (unknownLine, line_in_polygon)) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Checking if lines intersect.
	/// </summary>
	/// <returns><c>true</c>, if inter sect was linesed, <c>false</c> otherwise.</returns>
	/// <param name="X">X.</param>
	/// <param name="Y">Y.</param>
	public static bool LinesInterSect(Block X, Block Y){

		bool isIntersecting = false;

		//3d -> 2d
		Vector2 p1 = new Vector2(X.pointA.x, X.pointA.y);
		Vector2 p2 = new Vector2(X.pointB.x, X.pointB.y);

		Vector2 p3 = new Vector2(X.pointA.x, X.pointA.y);
		Vector2 p4 = new Vector2(X.pointB.x, X.pointB.y);

		float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

		//Direction of the lines
		Vector2 l1_dir = (p2 - p1).normalized;
		Vector2 l2_dir = (p4 - p3).normalized;

		//If we know the direction we can get the normal vector to each line
		Vector2 l1_normal = new Vector2(-l1_dir.y, l1_dir.x);
		Vector2 l2_normal = new Vector2(-l2_dir.y, l2_dir.x);



		//Make sure the denominator is > 0, if so the lines are parallel
		if (denominator != 0)
		{
			float u_a = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
			float u_b = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

			//Is intersecting if u_a and u_b are between 0 and 1
			if (u_a > 0 && u_a < 1 && u_b > 0 && u_b < 1)
			{
				//jeśli punkt przecięcia jest dalej niż długość odcinka 
				isIntersecting = true;
			}
		}

		Debug.Log(X.ToString() + " " + Y.ToString() + " " + isIntersecting);
		return isIntersecting;
	}

	//Are 2 vectors parallel?
	static bool IsParallel(Vector2 v1, Vector2 v2)
	{
		//2 vectors are parallel if the angle between the vectors are 0 or 180 degrees
		if (Vector2.Angle(v1, v2) == 0f || Vector2.Angle(v1, v2) == 180f)
		{
			return true;
		}

		return false;
	}

	//Are 2 vectors orthogonal?
	static bool IsOrthogonal(Vector2 v1, Vector2 v2)
	{
		//2 vectors are orthogonal is the dot product is 0
		//We have to check if close to 0 because of floating numbers
		if (Mathf.Abs(Vector2.Dot(v1, v2)) < 0.000001f)
		{
			return true;
		}

		return false;
	}

	//Is a point c between 2 other points a and b?
	static bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
	{
		bool isBetween = false;

		//Entire line segment
		Vector2 ab = b - a;
		//The intersection and the first point
		Vector2 ac = c - a;

		//Need to check 2 things: 
		//1. If the vectors are pointing in the same direction = if the dot product is positive
		//2. If the length of the vector between the intersection and the first point is smaller than the entire line
		if (Vector2.Dot(ab, ac) > 0f && ab.sqrMagnitude >= ac.sqrMagnitude)
		{
			isBetween = true;
		}

		return isBetween;
	}
}