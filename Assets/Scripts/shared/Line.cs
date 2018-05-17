using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line {

	public float A;
	public float B;

	public Line(float _A, float _B){
		A = _A;
		B = _B;
	}

	public override string ToString ()
	{
		return "y = " + A + "x + " + B;
	}
}
