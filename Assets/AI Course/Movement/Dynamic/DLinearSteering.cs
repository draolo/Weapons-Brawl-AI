using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DLinearSteering : MonoBehaviour {

	public Transform destination;
	public float speed = 3f;
	public float steer = 10f;
	public float stopAt = 0.01f;

	void FixedUpdate () {
		if (destination) {

			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			if (toDestination.magnitude > stopAt) {
				Vector3 accVector = toDestination.normalized;

				Vector3 tangentComponent = Vector3.Project (accVector, transform.forward);
				Vector3 normalComponent = accVector - tangentComponent;

				float tangentSpeed = tangentComponent.magnitude * speed;
				float rotationSpeed = normalComponent.magnitude * Vector3.Dot (normalComponent.normalized, transform.right) * steer;

				float t = Time.deltaTime;

				Rigidbody rb = GetComponent<Rigidbody> ();
				rb.MovePosition (rb.position + transform.forward * tangentSpeed * t);
				rb.MoveRotation (rb.rotation * Quaternion.Euler (0f, rotationSpeed * t, 0f));
			}
		}
	}

}
