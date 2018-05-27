using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HKNode {

	public int id;

	public Dictionary<HKNode, int> paths;

	public HKNode(int _id){
		id = _id;
		paths = new Dictionary<HKNode, int> ();
	}

	public void AddPath(HKNode _node, int _cost){
		paths.Add (_node, _cost);
	}


}
