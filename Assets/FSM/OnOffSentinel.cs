using UnityEngine;
using System.Collections;

public class OnOffSentinel : MonoBehaviour {

	[Range(0f, 20f)] public float range = 5f;
	public float reactionTime = 3f;
	public string targetTag = "Player";
	public Light ambientLight = null;

	private FSM fsm;

	void Start () {
		if (!ambientLight) return;	// Sanity

		// Define states and add actions when enter/exit/stay
		FSMState off = new FSMState();
		off.enterActions.Add(TurnOff);

		FSMState on = new FSMState();
		on.enterActions.Add(TurnOn);

		// Define transitions
		FSMTransition t1 = new FSMTransition (FriendsInRange);
		FSMTransition t2 = new FSMTransition (NoFriendsInRange);

		// Link states with transitions
		off.AddTransition(t1, on);
		on.AddTransition(t2, off);

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

	// ACTIONS

	public void TurnOn() {
		ambientLight.intensity = 1f;
	}

	public void TurnOff() {
		ambientLight.intensity = 0f;
	}

}