using UnityEngine;

public class DragBehaviour : MovementBehaviour {

	// how long does it take to stop moving by dragging
	public float linearDrag = 3f;
	public float angularDrag = 3f;

	public override Vector3 GetAcceleration (MovementStatus status) {
		return - (status.movementDirection.normalized * status.linearSpeed / linearDrag) 
			   - ((Quaternion.Euler (0f, 90f, 0f) * status.movementDirection.normalized) * status.angularSpeed / angularDrag);
	}
}

