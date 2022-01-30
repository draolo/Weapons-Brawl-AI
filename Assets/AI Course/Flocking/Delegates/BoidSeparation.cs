using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSeparation : BoidComponent {

	override public Vector3 GetDirection (Collider [] neighbors, int size) {

		Vector3 separation = Vector3.zero;
		Vector3 tmp;
		for (int i = 0; i < size; i += 1) {
			tmp = (transform.position - neighbors [i].ClosestPointOnBounds (transform.position));
			separation += tmp.normalized / (tmp.magnitude + 0.0001f);
		}
		return separation.normalized * BoidShared.SeparationComponent;
	}
}
