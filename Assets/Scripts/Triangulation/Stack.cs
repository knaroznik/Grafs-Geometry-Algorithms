using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stack {

	private List<GameObject> stack;

	public Stack(){
		stack = new List<GameObject> ();
	}

	public void Push(GameObject obj){
		stack.Add(obj);
	}

	public void Push(List<GameObject> input){
		for (int i = 0; i < input.Count; i++) {
			stack.Add (input [i]);
		}
	}

	public GameObject Pop(){
		if (stack.Count <= 0) {
			return null;
		}
		GameObject output = stack [0];
		stack.RemoveAt (0);
		return output;
	}

	public GameObject First(){
		return stack [0];
	}

	public GameObject Last(){
		return stack.Last ();
	}

	public int Count(){
		return stack.Count;
	}

	public List<GameObject> ToList(){
		List<GameObject> output = new List<GameObject> ();
		for (int i = 0; i<Count (); i++) {
			output.Add (stack [i]);
		}
		stack.Clear ();
		return output;
	}

	public void Remove(GameObject obj){
		stack.Remove (obj);
	}

	public void DebugStack(){
		for (int i = 0; i<Count (); i++) {
			Debug.Log (stack [i].transform.position.x + " " + stack [i].transform.position.y);
		}
	}

	public void DeleteFirstAndLast(){
		for (int i = 1; i < Count(); i++) {
			if (stack [i] == stack [i - 1]) {
				stack.RemoveAt (i);
			}
		}
		stack.Remove (Last ());
		stack.Remove (Last ());
	}
}
