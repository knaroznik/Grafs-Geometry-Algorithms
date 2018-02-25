using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDot : Dot {

	public UnityDot(float _x, float _y, GameObject _obj) : base(_x, _y)
	{
		this.obj = _obj;
	}

	public GameObject obj;
}
