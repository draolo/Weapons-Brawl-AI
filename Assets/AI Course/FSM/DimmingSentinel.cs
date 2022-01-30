using UnityEngine;
using System.Collections;

public class DimmingSentinel : MonoBehaviour {

	[Range (0f, 20f)] public float range = 5f;
	public float reactionTime = 3f;
	public string targetTag = "Player";
	public Light ambientLight = null;
	public float dimmingTime = 5f;
	
	private FSM fsm;
	private float dimmingStart;

	void Start () {
		if (!ambientLight) return;	// Sanity

		// Define states and link actions when enter/exit/stay
		FSMState off = new FSMState ();
		off.enterActions.Add (TurnOff);

		FSMState on = new FSMState();
		on.enterActions.Add (TurnOn);

		FSMState dimming = new FSMState();
		dimming.enterActions.Add (StartDimmer);
		dimming.stayActions.Add (Dimmer);

		// Define transitions
		FSMTransition t1 = new FSMTransition (FriendsInRange);
		FSMTransition t2 = new FSMTransition (NoFriendsInRange);
		FSMTransition t3 = new FSMTransition (FriendsInRange); // different from t1
		FSMTransition t4 = new FSMTransition (LightIsOff);

		// Link states with transitions
		off.AddTransition (t1, on);
		on.AddTransition (t2, dimming);
		dimming.AddTransition (t3, on);
		dimming.AddTransition (t4, off);

		// Setup a FSA at initial state
		fsm = new FSM(off);

		// Start monitoring
		StartCoroutine(Patrol());
	}

	// GIMMICS

	private void OnValidate () {
		Transform t = transform.Find ("Range");
		if (t != null) {
			t.localScale = new Vector3 (range / transform.localScale.x, 1f, range / transform.localScale.z) / 5f;
		}
	}

	// Periodic update, run forever
	public IEnumerator Patrol() {
		while(true) {
			fsm.Update();
			yield return new WaitForSeconds(reactionTime);
		}
	}
		
	// CONDITIONS

	public bool FriendsInRange() {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag(targetTag)) {
			if ((go.transform.position - transform.position).magnitude <= range) return true;
		}
		return false;
	}

	public bool NoFriendsInRange() {
		return !FriendsInRange();
	}

	public bool LightIsOff() {
		return (ambientLight.intensity == 0);
	}

	// ACTIONS

	public void TurnOn() {
		ambientLight.intensity = 1f;
	}

	public void TurnOff() {
		ambientLight.intensity = 0f;
	}

	public void StartDimmer () {
		dimmingStart = Time.realtimeSinceStartup;
	}

	public void Dimmer() {
		ambientLight.intensity = 1f - Mathf.Clamp01 ((Time.realtimeSinceStartup - dimmingStart) / dimmingTime);
	}
}