using UnityEngine;
using System.Collections;

public class DTSentinel : MonoBehaviour {

	[Range(0f, 20f)] public float range = 5f;
	public float reactionTime = 3f;
	public string targetTag = "Player";

	public Light alarmLight = null;
	public Color alarmColor = Color.red;

	private DecisionTree dt;
	private bool hidden; // this is our initial status
	private Color baseColor;	 // to reset the light correctly

	void Start () {

		// Define actions
		DTAction a1 = new DTAction(Show);
		DTAction a2 = new DTAction(Hide);
		DTAction a3 = new DTAction(Alarm);

		// Define decisions
		DTDecision d1 = new DTDecision(GetStatus);
		DTDecision d2 = new DTDecision(ScanField);
		DTDecision d3 = new DTDecision(ScanField);

		// Link action with decisions
		d1.AddLink(true, d2);
		d1.AddLink(false, d3);

		d2.AddLink(true, a1);

		d3.AddLink(true, a3);
		d3.AddLink(false, a2);

		// Setup my DecisionTree at the root node
		dt = new DecisionTree(d1);

		// Set to hidden status
		baseColor = alarmLight.color;
		Hide(null);
		// same as - a2.Action();

		// Start patroling
		StartCoroutine(Patrol());
	}

	// GIMMICS

	private void OnValidate () {
		Transform t = transform.Find ("Range");
		if (t != null) { // we assume it is a plane 
			t.localScale = new Vector3 (range / transform.localScale.x, 1f, range / transform.localScale.z) / 5f;
		}
	}

	// Take decision every interval, run forever
	public IEnumerator Patrol() {
		while(true) {
			dt.walk();
			yield return new WaitForSeconds(reactionTime);
		}
	}

	// ACTIONS

	public object Hide(object o) {
		alarmLight.color = baseColor;
		GetComponent<MeshRenderer> ().enabled = false;
		hidden = true;
		return null;
	}

	public object Show(object o) {
		GetComponent<MeshRenderer> ().enabled = true;
		hidden = false;
		return null;
	}

	public object Alarm(object o) {
		alarmLight.color = alarmColor;
		return null;
	}

	// DECISIONS

	// Check if we are hidden or not
	public object GetStatus(object o) {
		return hidden;
	}

	// Check if there are enemies in range
	public object ScanField(object o) {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag(targetTag)) {
			if ((go.transform.position - transform.position).magnitude <= range) return true;
		}
		return false;
	}



}