using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon {

	#region Starting : 
	protected PolygonDrawer drawer;

	public List<UnityDot> tops;
	protected List<Dot> topsAsDots;

	protected Dot MAX;
	protected Dot MIN;
	protected List<Dot> mins = new List<Dot> ();
	protected List<Dot> maxs = new List<Dot> ();

	protected bool maxInPolygon = false;
	protected bool minInPolygon = false;
	protected bool minIsOnePoint = false;
	protected bool maxIsOnePoint = false;

	protected Dot getDot(int i){
		return tops [(i % tops.Count + tops.Count)%tops.Count] as Dot;
	}
		
	public Polygon(PolygonDrawer drawer, List<Dot> dots){
		this.drawer = drawer;
		tops = new List<UnityDot> ();
		topsAsDots = dots;
		for (int i = 0; i < dots.Count; i++) {
			tops.Add (new UnityDot (dots [i].x, dots [i].y, drawer.DrawTop (dots [i].x, dots [i].y)));
		}
		drawLines (dots, 1f);
	}

	protected void drawLines(List<Dot> dots, float width){
		for (int i = 0; i < dots.Count; i++) {
			if(i==dots.Count-1){
				drawer.DrawLine (new Vector3 (dots [i].x, dots [i].y, 0f), new Vector3 (dots [0].x, dots [0].y, 0f), Color.white, width);
			}
			else{
				drawer.DrawLine (new Vector3 (dots [i].x, dots [i].y, 0f), new Vector3 (dots [(i+1)].x, dots [(i+1)].y, 0f), Color.white, width);
			}
		}
	}

	protected bool isInside(Dot point)
	{
		int polygonLength = tops.Count, i=0;
		bool inside = false;
		float pointX = point.x, pointY = point.y;
		float startX, startY, endX, endY;
		Dot endPoint = tops[polygonLength-1];           
		endX = endPoint.x; 
		endY = endPoint.y;
		while (i<polygonLength) {
			startX = endX;           startY = endY;
			endPoint = tops[i++];
			endX = endPoint.x;       endY = endPoint.y;
			inside ^= ( endY > pointY ^ startY > pointY )
				&&
				( (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY) ) ;
		}
		return inside;
	}

	public bool HaveKernel(){
		calculateImportantPoints ();

		if (MAX.y > MIN.y) {
			return false;
		}
		return true;
	}

	protected void addImportant(Dot checkingDot, float heightDifference, List<Dot> importantsList, ref Dot importantThing){
		if (isInside (new Dot (checkingDot.x, checkingDot.y + heightDifference))) {
			importantsList.Add (checkingDot);
			if (importantThing == null)
				importantThing = checkingDot;
			if (Mathf.Sign(checkingDot.y - importantThing.y) == Mathf.Sign(heightDifference)) {
				importantThing = checkingDot;
			}
		}
	}

	protected void addImportants(Dot checkingDot, float heightDifference, List<Dot> toAdd, List<Dot> importantsList, ref Dot importantThing){
		if (isInside (new Dot (checkingDot.x, checkingDot.y + heightDifference))) {
			for (int i = 0; i < toAdd.Count; i++) {
				importantsList.Add (toAdd [i]);
			}
			addImportant (checkingDot, heightDifference, importantsList, ref importantThing);
		}
	}
		
	protected void calculateImportantPoints(){
		for (int i = 0; i < tops.Count; i++) {
			Dot center = tops [i];
			Dot prev = getDot (i - 1);
			Dot next = getDot (i + 1);

			checkForMax (prev, center, next, i);
			checkForMin (prev, center, next, i);
		}

		if (MAX == null) {
			MAX = tops [0];
			for (int i = 1; i < tops.Count; i++) {
				if (tops [i].y < MAX.y) {
					MAX = tops [i];
				}
			}
			maxInPolygon = true;
		}

		if (MIN == null) {
			minInPolygon = true;
			MIN = tops [0];
			for (int i = 1; i < tops.Count; i++) {
				if (tops [i].y > MIN.y) {
					MIN = tops [i];
				}
			}
		}

		drawer.DrawImportantObject (MAX.x, MAX.y);
		drawer.DrawImportantObject (MIN.x, MIN.y);
	}

	protected void checkForMax(Dot prev, Dot center, Dot next, int i){
		if (prev.y < center.y && next.y < center.y) {
			addImportant (center, 0.1f, maxs, ref MAX);
		} 
		else if (prev.y < center.y && next.y == center.y) {
			List<Dot> local  = new List<Dot> ();
			local.Add (center);
			local.Add (next);
			int localIndex = i + 2;
			while (getDot (localIndex).y == next.y) {
				local.Add (getDot (localIndex));
				localIndex++;
			}
			if (getDot (localIndex).y < next.y) {
				addImportants (center, 0.1f, local, maxs, ref MAX);
			}
		}
		else if (prev.y == center.y && next.y > center.y) {
			List<Dot> local  = new List<Dot> ();
			local.Add (center);
			local.Add (prev);
			int localIndex = i - 2;
			while (getDot (localIndex).y == prev.y) {
				local.Add (getDot (localIndex));
				localIndex--;
			}
			if (getDot (localIndex).y < prev.y) {
				addImportants (center, 0.1f, local, maxs, ref MAX);
			}
		}
	}
		
	protected void checkForMin(Dot prev, Dot center, Dot next, int i){
		if (prev.y > center.y && next.y > center.y) {
			addImportant (center, -0.1f, mins, ref MIN);
		} else if (prev.y > center.y && next.y == center.y) {
			List<Dot> local  = new List<Dot> ();
			local.Add (center);
			local.Add (next);
			int localIndex = i + 2;
			while (getDot (localIndex).y == next.y) {
				local.Add (getDot (localIndex));
				localIndex++;
			}
			if (getDot (localIndex).y > next.y) {
				addImportants (center, -0.1f, local, mins, ref MIN);
			}
		}else if (prev.y == center.y && next.y > center.y) {
			List<Dot> local  = new List<Dot> ();
			local.Add (center);
			local.Add (prev);
			int localIndex = i - 2;
			while (getDot (localIndex).y == prev.y) {
				local.Add (getDot (localIndex));
				localIndex--;
			}
			if (getDot (localIndex).y > prev.y) {
				addImportants (center, -0.1f, local, mins, ref MIN);
			}
		}
	}

	#endregion

	#region Circuit : 


	protected Dot leftDownCorner, rightDownCorner, rightUpCorner, leftUpCorner;
	protected List<Dot> circuit = new List<Dot> ();

	public void calculateCircuit(){
		if (minInPolygon && maxInPolygon) {
			drawer.SetCircuitText (calculateCircuit (tops));
			drawer.SetTopsText (tops.Count);
			return;
		} else if (minInPolygon) {
			findMinPointsOnPolygon ();
			thirdVersion_minInPolygon_maxToCalculate ();
			return;
		} else if (maxInPolygon) {
			findMaxPointsOnPolygon ();
			secondVersion_maxInPolygon_minToCalculate ();
			return;
		} else {
			fourthVersion_minAndMaxToCalculate ();
			return;
		}
	}

	protected void secondVersion_maxInPolygon_minToCalculate(){

		Dot firstKnownPointRight = findMINPointsRIGHT();
		Dot firstKnownPointLeft = findMINPointsLEFT ();
		List<Dot> circuit = new List<Dot> ();
		circuit.Add (leftUpCorner);
		circuit.Add (rightUpCorner);
		addToList (circuit, 
			tops.FindIndex (x => x.x == firstKnownPointRight.x && x.y == firstKnownPointRight.y), 
			tops.FindIndex (x => x.x == rightDownCorner.x && x.y == rightDownCorner.y));
		addToList(circuit, 
			tops.FindIndex (x => x.x == leftDownCorner.x && x.y == leftDownCorner.y), 
			tops.FindIndex (x => x.x == firstKnownPointLeft.x && x.y == firstKnownPointLeft.y));
		drawLines (circuit, 2f);
		drawer.SetTopsText(circuit.Count);
		drawer.SetCircuitText (calculateCircuit (circuit));
	}

	protected void addToList(List<Dot> toAddList, int firstIndex, int lastIndex){
		for(int i=firstIndex; i<lastIndex+1; i++){
			toAddList.Add (tops [i]);
		}
	}

	protected void addSince(List<Dot> toAddList, int firstIndex, Dot lastDot){
		int i = firstIndex;
		int maxCount = firstIndex + tops.Count;
		while (getDot(i) != lastDot) {
			if (i > maxCount) {
				return;
			}
			toAddList.Add (tops [i]);
			i++;
		}
	}


	protected void thirdVersion_minInPolygon_maxToCalculate(){
		Dot lastKnownPointRight = findMAXPointsRIGHT();
		Dot lastKnownPointLeft = findMAXPointsLEFT ();
		List<Dot> circuit = new List<Dot> ();
		circuit.Add (leftUpCorner);
		addToList (circuit, 
			tops.FindIndex (x => x.x == rightUpCorner.x && x.y == rightUpCorner.y), 
			tops.FindIndex (x => x.x == lastKnownPointRight.x && x.y == lastKnownPointRight.y));
		circuit.Add (rightDownCorner);
		circuit.Add (leftDownCorner);
		addSince (circuit, 
			tops.FindIndex (x => x.x == lastKnownPointLeft.x && x.y == lastKnownPointLeft.y), 
			leftUpCorner);

		drawLines (circuit, 2f);
		drawer.SetTopsText(circuit.Count);
		drawer.SetCircuitText (calculateCircuit (circuit));
	}

	//FIXME : Better code for 4 of this : 

	protected Dot findMAXPointsRIGHT(){
		Dot up = new Dot(999,999);
		Dot down = new Dot(999,999);
		for (int i = 0; i < tops.Count; i++) {
			if (maxs.Contains (tops [i]) || mins.Contains (tops [i])) {
				continue;
			}

			if (tops [i].y >= MAX.y && Geometry.distance(tops[i].y, MAX.y) < Geometry.distance(up.y, MAX.y) && tops[i].x >= MAX.x) {
				up = tops [i];
			}

			if (tops [i].y <= MAX.y && Geometry.distance(tops[i].y, MAX.y) < Geometry.distance(down.y, MAX.y) && tops[i].x >= MAX.x) {
				down = tops [i];
			}
		}

		rightDownCorner = Geometry.pointofIntersection (new Block (up, down), new Block (MAX, new Dot (MAX.x + 1, MAX.y)));
		drawer.DrawImportantObject (rightDownCorner.x, rightDownCorner.y);
		return up;
	}

	protected Dot findMAXPointsLEFT(){
		Dot up = new Dot(999,999);
		Dot down = new Dot(999,999);
		for (int i = 0; i < tops.Count; i++) {
			if (maxs.Contains (tops [i]) || mins.Contains (tops [i])) {
				continue;
			}

			if (tops [i].y >= MAX.y && Geometry.distance(tops[i].y, MAX.y) < Geometry.distance(up.y, MAX.y) && tops[i].x <= MAX.x) {
				up = tops [i];
			}

			if (tops [i].y <= MAX.y && Geometry.distance(tops[i].y, MAX.y) < Geometry.distance(down.y, MAX.y) && tops[i].x <= MAX.x) {
				down = tops [i];
			}
		}
		leftDownCorner = Geometry.pointofIntersection (new Block (up, down), new Block (MAX, new Dot (MAX.x + 1, MAX.y)));
		drawer.DrawImportantObject (leftDownCorner.x, leftDownCorner.y);
		return up;
	}

	protected Dot findMINPointsRIGHT(){
		Dot up = new Dot(999,999);
		Dot down = new Dot(999,999);
		for (int i = 0; i < tops.Count; i++) {
			if (maxs.Contains (tops [i]) || mins.Contains (tops [i])) {
				continue;
			}

			if (tops [i].y >= MIN.y && Geometry.distance(tops[i].y, MIN.y) < Geometry.distance(up.y, MIN.y) && tops[i].x >= MIN.x) {
				up = tops [i];
			}

			if (tops [i].y <= MIN.y && Geometry.distance(tops[i].y, MIN.y) < Geometry.distance(down.y, MIN.y) && tops[i].x >= MIN.x) {
				down = tops [i];
			}
		}
		rightUpCorner = Geometry.pointofIntersection (new Block (up, down), new Block (MIN, new Dot (MIN.x + 1, MIN.y)));
		drawer.DrawImportantObject (rightUpCorner.x, rightUpCorner.y);
		return down;
	}

	protected Dot findMINPointsLEFT(){
		Dot up = new Dot(999,999);
		Dot down = new Dot(999,999);
		for (int i = 0; i < tops.Count; i++) {
			if (maxs.Contains (tops [i]) || mins.Contains (tops [i])) {
				continue;
			}

			if (tops [i].y >= MIN.y && Geometry.distance(tops[i].y, MIN.y) < Geometry.distance(up.y, MIN.y) && tops[i].x <= MIN.x) {
				up = tops [i];
			}

			if (tops [i].y <= MIN.y && Geometry.distance(tops[i].y, MIN.y) < Geometry.distance(down.y, MIN.y) && tops[i].x <= MIN.x) {
				down = tops [i];
			}
		}
		leftUpCorner = Geometry.pointofIntersection (new Block (up, down), new Block (MIN, new Dot (MIN.x + 1, MIN.y)));
		drawer.DrawImportantObject (leftUpCorner.x, leftUpCorner.y);
		return down;
	}


	protected void fourthVersion_minAndMaxToCalculate (){

	}


	protected void findMinPointsOnPolygon(){
		List<Dot> search = new List<Dot> ();
		for (int i = 0; i < tops.Count; i++) {
			if (tops [i].y == MIN.y) {
				search.Add (tops [i]);
			}
		}

		if (search.Count == 1) {
			rightUpCorner = search [0];
			drawer.DrawImportantObject (rightUpCorner.x, rightUpCorner.y);
			minIsOnePoint = true;
			return;
		}


		Dot q;
		Dot w;
		float prev_count = 0;

		for (int i = 0; i < search.Count; i++) {
			for (int j = 0; j < search.Count; j++) {
				q = search [i];
				w = search [j];
				if (Geometry.distance (q, w) > prev_count) {
					rightUpCorner = q;
					leftUpCorner = w;
					prev_count = Geometry.distance (q, w);
				}
			}
		}

		if (rightUpCorner.x < leftUpCorner.x) {
			Dot temp = rightUpCorner;
			rightUpCorner = leftUpCorner;
			leftUpCorner = temp;
		}

		drawer.DrawImportantObject (rightUpCorner.x, rightUpCorner.y);
		drawer.DrawImportantObject (leftUpCorner.x, leftUpCorner.y);
	}

	protected void findMaxPointsOnPolygon(){
		List<Dot> search = new List<Dot> ();

		for (int i = 0; i < tops.Count; i++) {
			if (tops [i].y == MAX.y) {
				search.Add (tops [i]);
			}
		}

		if (search.Count == 1) {
			maxIsOnePoint = true;
			leftDownCorner = search [0];
			drawer.DrawImportantObject (leftDownCorner.x, leftDownCorner.y);
			return;
		}

		Dot q;
		Dot w;
		float prev_count = 0;

		for (int i = 0; i < search.Count; i++) {
			for (int j = 0; j < search.Count; j++) {
				q = search [i];
				w = search [j];
				if (Geometry.distance (q, w) > prev_count) {
					leftDownCorner = q;
					rightDownCorner = w;
					prev_count = Geometry.distance (q, w);
				}
			}
		}

		if (leftDownCorner.x > rightDownCorner.x) {
			Dot temp = leftDownCorner;
			leftDownCorner = rightDownCorner;
			rightDownCorner = temp;
		}

		drawer.DrawImportantObject (leftDownCorner.x, leftDownCorner.y);
		drawer.DrawImportantObject (rightDownCorner.x, rightDownCorner.y);
	}

	protected float calculateCircuit(List<Dot> points){
		float output = 0;
		for (int i = 0; i < points.Count; i++) {
			output += Geometry.distance (points[(i % points.Count + points.Count)%points.Count], points[(i+1 % points.Count + points.Count)%points.Count]);
		}
		return output;
	}

	protected float calculateCircuit(List<UnityDot> points){
		float output = 0;
		for (int i = 0; i < points.Count; i++) {
			output += Geometry.distance (points[(i % points.Count + points.Count)%points.Count], points[(i+1 % points.Count + points.Count)%points.Count]);
		}
		return output;
	}

	#endregion
}
