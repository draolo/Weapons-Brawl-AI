using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBehaviour : MovementBehaviour {

	public Transform destination;

	public float gas = 3f;
	public float steer = 30f;
	public float brake = 20f;

	public float brakeAt = 95f;
	public float stopAt = 100f;

	public override Vector3 GetAcceleration (MovementStatus status) {
		if (destination != null) {
			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			Vector3 fromDestination = (transform.position - verticalAdj);

			if (fromDestination.magnitude < stopAt) {
				Vector3 tangentComponent = Vector3.Project (fromDestination.normalized, status.movementDirection);
				Vector3 normalComponent = (fromDestination.normalized - tangentComponent);
				return (tangentComponent * (fromDestination.magnitude < brakeAt ? gas : -brake)) + (normalComponent * steer);
			} else {
				return Vector3.zero;
			}
		} else {
			return Vector3.zero;
		}
	}
}
