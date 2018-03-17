using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Dot {

	public Dot(float _x, float _y){
		x = _x;
		y = _y;
	}

	public float x;
	public float y;

	public override string ToString ()
	{
		return Math.Round(x, 2).ToString () + "," + Math.Round(y, 2).ToString ();
	}
}
