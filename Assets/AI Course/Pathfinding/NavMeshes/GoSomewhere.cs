using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoSomewhere : MonoBehaviour {

	public Transform destination;

	void Start () {
		GetComponent<NavMeshAgent>().destination = destination.position;
	}
	
}
