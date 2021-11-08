using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DMoveTo : MonoBehaviour {

	public Transform destination;
	public float speed = 2f;
	public float stopAt = 0.01f;

	void FixedUpdate () {
		if (destination) {

			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			Vector3 toDestination = (verticalAdj - transform.position);

			if (toDestination.magnitude > stopAt) {
				// we keep only option a
				transform.LookAt (verticalAdj);
				Rigidbody rb = GetComponent<Rigidbody> ();
				rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
			}
		}
	}
}
