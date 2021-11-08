﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class DPushTo : MonoBehaviour {

	public Transform destination;
	public float force = 200f;

	void FixedUpdate () {
		if (destination) {
			Vector3 verticalAdj = new Vector3 (destination.position.x, transform.position.y, destination.position.z);
			transform.LookAt (verticalAdj);
			Rigidbody rb = GetComponent<Rigidbody> ();
			rb.AddForce (transform.forward * force);
		}
	}
}