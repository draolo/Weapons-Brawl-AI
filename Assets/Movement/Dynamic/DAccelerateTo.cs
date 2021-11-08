using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DAccelerateTo : MonoBehaviour {

	public Transform destination;

	public float gas = 2f;
	public float maxSpeed = 5f;
	public float stopAt = 0.01f;

	private float currentSpeed = 0f;

	public float brake = 3f;
	public float brakeAt = 5f;
	public float minSpeed = 0.5f;

	void FixedUpdate () {
		if (destination) {

			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			if (toDestination.magnitude > stopAt) {
				transform.LookAt (verticalAdj);

				float t = Time.deltaTime;
				float x;

				if (toDestination.magnitude > brakeAt) {
					x = currentSpeed * t + 0.5f * gas * t * t;
					currentSpeed += gas * t;
					currentSpeed = Mathf.Min (currentSpeed, maxSpeed);
				} else {					
					x = currentSpeed * t - 0.5f * brake * t * t;
					currentSpeed -= brake * t;
					currentSpeed = Mathf.Max (currentSpeed, minSpeed);
				}

				Rigidbody rb = GetComponent<Rigidbody> ();
				rb.MovePosition (rb.position + transform.forward * x);
			}
		}
	}

	private void OnDrawGizmos () {
		UnityEditor.Handles.Label (transform.position + 2f * transform.up, currentSpeed.ToString ());
	}

}
