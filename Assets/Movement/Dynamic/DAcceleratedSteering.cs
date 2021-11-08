using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DAcceleratedSteering : MonoBehaviour {

	public Transform destination;

	public float gas = 3f;
	public float steer = 3f;

	private float currentLinearSpeed = 0f;
	private float currentAngularSpeed = 0f;

	void FixedUpdate () {
		if (destination) {

			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			Vector3 accVector = toDestination.normalized;

			Vector3 tangentComponent = Vector3.Project (accVector, transform.forward);
			Vector3 normalComponent = accVector - tangentComponent;

			float tangentAcc = tangentComponent.magnitude * gas;
			float rotationAcc = normalComponent.magnitude * Vector3.Dot (normalComponent.normalized, transform.right) * steer;

			float t = Time.deltaTime;

			float tangentDelta = currentLinearSpeed * t + 0.5f * tangentAcc * t * t;
			float rotationDelta = currentAngularSpeed * t + 0.5f * rotationAcc * t * t;

			currentLinearSpeed += tangentAcc * t;
			currentAngularSpeed += rotationAcc * t;

			Rigidbody rb = GetComponent<Rigidbody> ();
			rb.MovePosition (rb.position + transform.forward * tangentDelta);
			rb.MoveRotation (rb.rotation * Quaternion.Euler (0f, rotationDelta, 0f));
		}
	}

}
