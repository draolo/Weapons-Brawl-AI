using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCohesion : BoidComponent {

	override public Vector3 GetDirection (Collider [] neighbors, int size) {
		Vector3 cohesion = Vector3.zero;
		float counter = 0f;
		for (int i = 0; i < size; i += 1) {
			if (neighbors [i].gameObject.layer == gameObject.layer) {
				cohesion += neighbors [i].transform.position;
				counter += 1f;
			}
		}
		cohesion /= (float) counter;
		cohesion -= transform.position;
		return cohesion.normalized * BoidShared.CohesionComponent;
	}
}