using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class DDelegatedSteering : MonoBehaviour {

	public float minLinearSpeed = 0.5f;
	public float maxLinearSpeed = 5f;
	public float maxAngularSpeed = 5f;

	private MovementStatus status;

	private void Start () {
		status = new MovementStatus ();
	}

	void FixedUpdate () {

		status.movementDirection = transform.forward;

		// Contact al behaviours and build a list of directions
		List<Vector3> components = new List<Vector3> ();
		foreach (MovementBehaviour mb in GetComponents<MovementBehaviour> ())
			components.Add (mb.GetAcceleration (status));

		// Blend the list to obtain a single acceleration to apply
		Vector3 blendedAcceleration = Blender.Blend (components);

		// if we have an acceleration, apply it
		if (blendedAcceleration.magnitude != 0f) {
			Driver.Steer (GetComponent<Rigidbody> (), status, blendedAcceleration,
				          minLinearSpeed, maxLinearSpeed, maxAngularSpeed);
		}
	}

	private void OnDrawGizmos () {
		if (status != null) {
			UnityEditor.Handles.Label (transform.position + 2f * transform.up, status.linearSpeed.ToString () + "\n" + status.angularSpeed.ToString ());
		}
	}

}
