using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DGripSteering : MonoBehaviour {

	public Transform destination;

	public float gas = 3f;
	public float steer = 3f;
	public float brake = 3f;

	[Range (0f, 1f)] public float linearDrag = .1f;
	[Range (0f, 1f)] public float angularDrag = .1f;

	public float minLinearSpeed = 0.5f;
	public float maxLinearSpeed = 5f;
	public float maxAngularSpeed = 5f;

	public float brakeAt = 5f;
	public float stopAt = 0.01f;

	private float currentLinearSpeed = 0f;
	private float currentAngularSpeed = 0f;

	void FixedUpdate () {
		if (destination) {

			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			if (toDestination.magnitude > stopAt) {

				Vector3 accVector = toDestination.normalized;

				Vector3 tangentComponent = Vector3.Project (accVector, transform.forward);
				Vector3 normalComponent = accVector - tangentComponent;

				float currentGas = toDestination.magnitude > brakeAt ? gas : -brake;
				// steering is not affected by distance

				float tangentAcc = tangentComponent.magnitude * currentGas;
				float rotationAcc = normalComponent.magnitude * Vector3.Dot (normalComponent, transform.right) * steer;

				float t = Time.deltaTime;

				float tangentDelta = currentLinearSpeed * t + 0.5f * tangentAcc * t * t;
				float rotationDelta = currentAngularSpeed * t + 0.5f * rotationAcc * t * t;

				currentLinearSpeed = (currentLinearSpeed * (1f - linearDrag * t)) + tangentAcc * t;
				currentAngularSpeed = (currentAngularSpeed * (1f - angularDrag * t)) + rotationAcc * t;

				currentLinearSpeed = Mathf.Clamp (currentLinearSpeed, minLinearSpeed, maxLinearSpeed);
				currentAngularSpeed = Mathf.Clamp (currentAngularSpeed, -maxAngularSpeed, maxAngularSpeed);

				Rigidbody rb = GetComponent<Rigidbody> ();
				rb.MovePosition (rb.position + transform.forward * tangentDelta);
				rb.MoveRotation (rb.rotation * Quaternion.Euler (0f, rotationDelta, 0f));
			}
		}
	}

	private void OnDrawGizmos () {
		UnityEditor.Handles.Label (transform.position + 2f * transform.up, currentLinearSpeed.ToString () + "\n" + currentAngularSpeed.ToString ());
	}

}
