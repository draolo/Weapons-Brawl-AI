using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidBehaviour : MovementBehaviour {

	public float sightRange = 5f;
	public float sightAngle = 45f;

	public float steer = 15f;
	public float backpedal = 10f;

	public override Vector3 GetAcceleration (MovementStatus status) {

		bool leftHit = Physics.Raycast (transform.position, Quaternion.Euler (0f, -sightAngle, 0f) * status.movementDirection, sightRange);
		bool centerHit = Physics.Raycast (transform.position, status.movementDirection, sightRange);
		bool rightHit = Physics.Raycast (transform.position, Quaternion.Euler (0f, sightAngle, 0f) * status.movementDirection, sightRange);

		Vector3 right = Quaternion.Euler (0f, 90f, 0f) * status.movementDirection.normalized;

		if (leftHit && !centerHit && !rightHit) {
			return right * steer;
		} else if (leftHit && centerHit && !rightHit) {
			return right * steer * 2f;
		} else if (leftHit && centerHit && rightHit) {
			return -status.movementDirection.normalized * backpedal;
		} else if (!leftHit && centerHit && rightHit) {
			return -right * steer * 2f;
		} else if (!leftHit && !centerHit && rightHit) {
			return -right * steer;
		} else if (!leftHit && centerHit && !rightHit) {
			return right * steer;
		}

		return Vector3.zero;
	}


	private Vector3 ObjectSize (GameObject go) {
		Bounds b = new Bounds (go.transform.position, Vector3.zero);
		foreach (Collider c in go.GetComponentsInChildren<Collider> ()) {
			b.Encapsulate (c.bounds);
		}
		return b.size;
	}

}
