using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour {

	public float radius = 10f;
	public int count = 100;
	public GameObject boid = null;

	void Start () {
		if (boid != null) {
			for (int i = 0; i < count; i += 1) {
				GameObject go = Instantiate (boid, transform.position + Random.insideUnitSphere * radius, transform.rotation);
				go.transform.LookAt (transform.position + Random.insideUnitSphere * radius);
				go.name = boid.name + " " + i;
			}
		}
	}
}
