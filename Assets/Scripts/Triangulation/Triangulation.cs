using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class Triangulation : MonoBehaviour {

	private Printer m_printer;
	private GameObject prefabParent;

	[SerializeField] private GameObject dotPrefab;
	[SerializeField] private GameObject linePrefab;
	[SerializeField] private Material whiteMaterial;
	[SerializeField] private Material redMaterial;
	[SerializeField] private Material greenMaterial;

	private List<GameObject> sortedByTime = new List<GameObject> ();
	private List<GameObject> sortedByX = new List<GameObject>();

	private List<GameObject> top = new List<GameObject>();
	private List<GameObject> down = new List<GameObject> ();


	// Use this for initialization
	void Start () {
		prefabParent = new GameObject ();
		prefabParent.name = "TriangulationParent";
		m_printer = GetComponent<Printer> ();
	}
	
	// Update is called once per frame
	void Update () {
		clickPressed ();
	}

	private void clickPressed(){
		if (Input.GetMouseButtonDown (0)) {
			if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5) {
				return;
			}
			sortedByTime.Add(m_printer.PrintObject (dotPrefab, prefabParent.transform));
		}
	}

	public void ClearPolygon(){
		sortedByTime.Clear ();
		sortedByX.Clear ();
		top.Clear ();
		down.Clear ();
		int max = prefabParent.transform.childCount;
		for (int i=max-1; i>=0; i--) {
			Destroy(prefabParent.transform.GetChild(i).gameObject);
		}
	}

	public void CreatePolygon(){
		sortedByX = sortedByTime.OrderBy (p => p.transform.position.x).ToList ();
		int count = sortedByTime.FindIndex (p => p.transform.position == sortedByX[0].transform.position);
		int startCount = count;

		List<GameObject> tmpsortedX = new List<GameObject> ();
		for (int i = 0; i < sortedByX.Count; i++) {
			tmpsortedX.Add (sortedByX [i]);
		}

		//TOP
		while (sortedByTime [(count) % sortedByTime.Count].transform.position.x <= sortedByTime [(count + 1) % sortedByTime.Count].transform.position.x ||
		       sortedByTime [(count) % sortedByTime.Count].transform.position.y <= sortedByTime [(count + 1) % sortedByTime.Count].transform.position.y) {
			tmpsortedX.Remove (sortedByTime [(count) % sortedByTime.Count]);
			top.Add (sortedByTime[(count) % sortedByTime.Count]);
			count++;
		}
		top.Add (sortedByTime[(count) % sortedByTime.Count]);
		//DOWN
		down.Add (sortedByTime [startCount]);
		for (int i = 0; i < tmpsortedX.Count; i++) {
			down.Add (tmpsortedX [i]);
		}	

		//PRINT
		for (int i = 1; i < top.Count; i++) {
			m_printer.PrintLine (linePrefab, prefabParent.transform, top [i-1].transform.position, top [i].transform.position, redMaterial, 5f, false);
		}
		for (int i = 1; i < down.Count; i++) {
			m_printer.PrintLine (linePrefab, prefabParent.transform, down [i-1].transform.position, down [i].transform.position, greenMaterial, 5f, false);
		}
	}

	public void Triangulate(){
		Stack s = new Stack ();
		s.Push (sortedByX [0]);
		s.Push (sortedByX [1]);
		for (int i = 2; i < sortedByX.Count; i++) {
			if(!sameLane(sortedByX[i], s.Last())){
				List<GameObject> a = s.ToList ();
				for (int j = 1; j < a.Count; j++) {
					m_printer.PrintLine (linePrefab, 
						prefabParent.transform, 
						sortedByX [i].transform.position, 
						a [j].transform.position, whiteMaterial, 2f, false);
				}
				s.Push (a.Last());
				s.Push (sortedByX [i]);
			}
			else{
				GameObject lastObj = s.Pop ();
				List<GameObject> tmp = new List<GameObject> ();
				int count = s.Count ();
				for(int q=0; q<count; q++){
					lastObj = s.Pop ();
					if (!IsOutside (lastObj, sortedByX [i], sortedByTime)) {
						m_printer.PrintLine (linePrefab, prefabParent.transform, sortedByX [i].transform.position, lastObj.transform.position, whiteMaterial, 2f, false);
					} else {
						tmp.Add (lastObj);
					}
				}
				s.Push (tmp);
				s.Push (lastObj);
				s.Push (sortedByX [i]);

			}
		}

		s.DebugStack ();
		s.DeleteFirstAndLast ();
		Debug.Log ("NEXT");
		s.DebugStack ();

		int x = s.Count ();
		for (int i = 0; i < x; i++) {
			GameObject lastObj = s.Pop ();
			m_printer.PrintLine (linePrefab, prefabParent.transform, 
				lastObj.transform.position, 
				sortedByX [sortedByX.Count - 1].transform.position, 
				whiteMaterial, 2f, false);
		}
	}

	private bool sameLane(GameObject x, GameObject y){
		if (top.Contains (x) && top.Contains (y))
			return true;
		if (down.Contains (x) && down.Contains (y))
			return true;
		return false;
	}

	public bool IsOutside(GameObject lineP1, GameObject lineP2, List<GameObject> region)
	{
		return !Geometry.LineInsidePolygon (region, false, lineP1, lineP2);
	}

}
