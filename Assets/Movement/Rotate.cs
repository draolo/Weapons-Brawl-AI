using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public bool active = false;
	public float dps = 10f;

	void Update () {
		if (active) {
			transform.Rotate (transform.up * dps * Time.deltaTime);
		}
	}
}
