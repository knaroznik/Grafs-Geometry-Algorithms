using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

	public readonly Dot pointA;
	public readonly Dot pointB;

	public Block(Dot a, Dot b){
		pointA = a;
		pointB = b;
	}

	public float Length()
	{
		return Geometry.distance (pointA, pointB);
	}

	public float LengthSquared()
	{
		return Geometry.distanceSquared (pointA, pointB);
	}

	public override string ToString(){
		return pointA.ToString() + " " + pointB.ToString ();
	}

}
