using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class ChaseSomething : MonoBehaviour {

	public Transform destination;
	public float resampleTime = 5f;

	void Start () {
		StartCoroutine (GoChasing());
	}

	private IEnumerator GoChasing() {
		while (true) {
			GetComponent<NavMeshAgent> ().destination = destination.position;
			yield return new WaitForSeconds (resampleTime);
		}
	}
	
}
