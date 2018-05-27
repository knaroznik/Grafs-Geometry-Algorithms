using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Held_Karp_Algorithm : MonoBehaviour {

	HKNode a; //root
	HKNode b;
	HKNode c;
	HKNode d;

	void Start () {
		CreateGraph ();
		Debug.Log(FindPath (new List<HKNode> (){ a, b, c, d }, a));
	}
	
	void CreateGraph(){
		a = new HKNode (1);
		b = new HKNode (2);
		c = new HKNode (3);
		d = new HKNode (4);


		a.AddPath (b, 30);
		a.AddPath (c, 36);
		a.AddPath (d, 40);

		b.AddPath (a, 30);
		b.AddPath (c, 20);
		b.AddPath (d, 50);

		c.AddPath (a, 36);
		c.AddPath (b, 20);
		c.AddPath (d, 67);

		d.AddPath (a, 40);
		d.AddPath (b, 50);
		d.AddPath (c, 67);
	}

	int FindPath(List<HKNode> left, HKNode currentNode){
		left.Remove (currentNode);
		if (left.Count == 0) {
			int output = 0;
			currentNode.paths.TryGetValue (a, out output);
			return output;
		} else {
			int output = 9999;
			foreach (HKNode node in left) {
				int cost = 0;
				currentNode.paths.TryGetValue (node, out cost);
				int x = FindPath (left.ToList(), node);
				x += cost;
				if (x < output) {
					output = x;
				}
			}
			return output;
		}
	}
}
