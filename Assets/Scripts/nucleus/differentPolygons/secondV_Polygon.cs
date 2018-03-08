using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondV_Polygon : PolygonDrawer {

	private List<Dot> calculatedTops;

	void Start () {

		calculate ();

		scenePolygon = new GameObject ();
		scenePolygon.name = this.name + " polygon";
		sceneBehaviour = FindObjectOfType<SceneBehaviour> ();
		Polygon pol = new Polygon (this, calculatedTops);

		bool hasKernel = pol.HaveKernel ();
		SetKernelText (hasKernel);

		if (hasKernel) {
			pol.calculateCircuit ();
		}
	}

	void calculate(){
		calculatedTops = new List<Dot> ();

		float firstX = Random.Range (-40f, -30f);
		float firstY = Random.Range (40f, 30f);
		calculatedTops.Add (new Dot (firstX, firstY));

		float _x = firstX + 10f;
		calculatedTops.Add (new Dot (_x, firstY));

		for (int i = 0; i < 4; i++) {
			float differerenceX = Random.Range (1f, 40f);
			_x += 5;
			float y = firstY - differerenceX;
			calculatedTops.Add (new Dot (_x, y));
			_x += 5;
			calculatedTops.Add (new Dot (_x, y+ differerenceX));
		}
			
		calculatedTops.Add (new Dot (-firstX, firstY));

		float prev_x = -firstX;
		float next_y = firstY;

		int differerence = Random.Range (1, 5);

		for (int i = 0; i < 10; i++) {
			next_y -= 4f;
			calculatedTops.Add (new Dot (prev_x+differerence, next_y));
			next_y -= 4f;
			calculatedTops.Add (new Dot (prev_x-differerence, next_y));
		}

		calculatedTops.Add (new Dot (firstX, next_y));

		prev_x = firstX;
		differerence = Random.Range (1, 5);

		for (int i = 0; i < 9; i++) {
			next_y += 4f;
			calculatedTops.Add (new Dot (prev_x-differerence, next_y));
			next_y += 4f;
			calculatedTops.Add (new Dot (prev_x+differerence, next_y));
		}
	}
}
