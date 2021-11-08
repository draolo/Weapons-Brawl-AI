using UnityEngine;
using System.Collections;

public class AlarmSentinelCR : MonoBehaviour {

	[Range (0f, 20f)] public float range = 5f;
	public float reactionTime = 3f;
	public string targetTag = "Player";
	public Light ambientLight = null;
	public Color color1 = Color.red;
	public Color color2 = Color.yellow;
	public float switchTime = .5f;

	private FSM fsm;
	private bool alarmOn;

	void Start () {
		if (!ambientLight) return;	// Sanity

		// Define states and link actions when enter/exit/stay
		FSMState off = new FSMState();

		FSMState alarm = new FSMState();
		alarm.enterActions.Add(StartAlarm);
		alarm.exitActions.Add(ShutAlarm);

		// Define transitions
		FSMTransition t1 = new FSMTransition (EnemiesAround);
		FSMTransition t2 = new FSMTransition (NoEnemiesAround);

		// Link states with transitions
		off.AddTransition (t1, alarm);
		alarm.AddTransition (t2, off);

		// Setup a FSA at initial state
		fsm = new FSM(off);

		// Alarm is off
		alarmOn = false;

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

	public bool EnemiesAround() {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag(targetTag)) {
			if ((go.transform.position - transform.position).magnitude <= range) return true;
		}
		return false;
	}

	public bool NoEnemiesAround() {
		return !EnemiesAround();
	}

	// ACTIONS

	public void StartAlarm () {
		StartCoroutine(RingAlarm());
	}

	public void ShutAlarm() {
		alarmOn = false;
	}

	// ALARM COROUTINE

	public IEnumerator RingAlarm() {
		alarmOn = true;
		Color initialColor = ambientLight.color;
		bool firstColor = true;
		while (alarmOn) {
			ambientLight.color = firstColor ? color1 : color2;
			firstColor = !firstColor;
			yield return new WaitForSeconds(switchTime);
		}
		ambientLight.color = initialColor;
	}

}