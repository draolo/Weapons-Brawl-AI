using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidBehaviourVolume : MovementBehaviour {

	public float sightRange = 5f;
	public float sightAngle = 45f;

	public float steer = 15f;
	public float backpedal = 10f;

	public override Vector3 GetAcceleration (MovementStatus status) {

		Collider collider = GetComponent<Collider> ();

		bool leftHit = Physics.BoxCast (transform.position, 
		                                collider.bounds.extents, 
		                                Quaternion.Euler (0f, - sightAngle, 0f) * status.movementDirection, 
		                                transform.rotation, 
		                                sightRange);
		
		bool centerHit = Physics.BoxCast (transform.position, 
		                                  collider.bounds.extents, 
		                                  status.movementDirection, 
		                                  transform.rotation, 
		                                  sightRange);
		
		bool rightHit = Physics.BoxCast (transform.position, 
		                                 collider.bounds.extents, 
		                                 Quaternion.Euler (0f, sightAngle, 0f) * status.movementDirection, 
		                                 transform.rotation, 
		                                 sightRange);

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
}
