using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotWithName : Dot {

	public string alias;

	public DotWithName(float x, float y, string _name) : base(x,y){
		alias = _name;
	}
}
