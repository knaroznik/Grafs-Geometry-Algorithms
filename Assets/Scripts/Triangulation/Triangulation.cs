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
	[SerializeField] private GameObject importantPrefab;
	[SerializeField] private Material whiteMaterial;
	[SerializeField] private Material redMaterial;
	[SerializeField] private Material greenMaterial;

	private List<GameObject> sortedByTime = new List<GameObject> ();
	private List<GameObject> sortedByX = new List<GameObject>();

	private List<GameObject> top = new List<GameObject>();
	private List<GameObject> down = new List<GameObject> ();

	private List<GameObject> debugList = new List<GameObject> ();


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
		List<GameObject> stack = new List<GameObject> ();
		stack.Add (sortedByX [0]);
		stack.Add (sortedByX [1]);

		for (int i = 2; i < sortedByX.Count-1; i++) {
			if(!sameLane(sortedByX[i], stack[stack.Count-1])){
				//Kopiowanie i czyszczenie 
				Debug.Log("FIRST");
				List<GameObject> a = new List<GameObject> ();
				for (int j = 0; j < stack.Count; j++) {
					a.Add (stack [j]);
				}
				stack.Clear ();

				//Zgłaszanie przekątnych
				for (int j = 1; j < a.Count; j++) {
					m_printer.PrintLine (linePrefab, 
						prefabParent.transform, 
						sortedByX [i].transform.position, 
						a [j].transform.position, whiteMaterial, 2f, false);
				}

				//Dodawanie do stosu
				stack.Add(a[a.Count-1]);
				stack.Add(sortedByX [i]);
			}
			else{
				Debug.Log ("SECOND");
				//Usunięcie ostatniego
				GameObject lastObj = stack[stack.Count-1];

				stack.RemoveAt (stack.Count-1);
				int count = stack.Count;
				//Pętla zgłaszania
				for(int j=count-1; j>=0; j--){
					GameObject tmp = stack [j];
					if (IsInside (tmp, sortedByX [i], sortedByTime)) {
						lastObj = tmp;
						m_printer.PrintLine (linePrefab, prefabParent.transform, sortedByX [i].transform.position, lastObj.transform.position, whiteMaterial, 2f, false);
						stack.RemoveAt (j);
					}
				}

				//Dodawanie do stosu
				stack.Add (lastObj);
				stack.Add (sortedByX [i]);

			}
		}

		stack.RemoveAt (0);
		stack.RemoveAt(stack.Count-1);
		int x = stack.Count ();
		for (int i = 0; i < x; i++) {
			GameObject lastObj = stack [i];
			m_printer.PrintLine (linePrefab, prefabParent.transform, 
				lastObj.transform.position, 
				sortedByX [sortedByX.Count - 1].transform.position, 
				greenMaterial, 1f, false);
		}
	}

	private void DebugList(List<GameObject> list){
		for (int i = 0; i < list.Count; i++) {
			m_printer.PrintObject (importantPrefab, list[i].transform.position, prefabParent.transform);
		}

	}


	private bool sameLane(GameObject x, GameObject y){
		if (down.Contains (x) && top.Contains (x)) {
			return false;
		}

		if (down.Contains (y) && top.Contains (y)) {
			return false;
		}

		if (top.Contains (x) && top.Contains (y))
			return true;
		if (down.Contains (x) && down.Contains (y))
			return true;


		return false;
	}

	public bool IsInside(GameObject lineP1, GameObject lineP2, List<GameObject> region)
	{
		return Geometry.LineInsidePolygon (region, false, lineP1, lineP2);
	}

}
