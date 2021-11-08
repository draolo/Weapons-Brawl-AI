using UnityEngine;
using System.Collections;

using MonoBT;

[RequireComponent (typeof (SeekBehaviour))]
[RequireComponent (typeof (FleeBehaviour))]

public class MonoBTSentinel : MonoBehaviour {

	[Range (0f, 20f)] public float sensingRange = 10f;
	[Range (0f, 20f)] public float fearRange = 4f;
	public string targetTag = "Player";

	void Start () {
		BTAction a1 = new BTAction (Hide);
		BTAction a2 = new BTAction (Show);
		BTAction a3 = new BTAction (Chase);
		BTAction a4 = new BTAction (Flee);

		BTCondition c1 = new BTCondition (EnemyOutsideRange);
		BTCondition c2 = new BTCondition (EnemyNotTooClose);

		BTDecorator d1 = new BTDecoratorUntilFail (c1);

		BTSequence s2 = new BTSequence (new IBTTask[] { c2, a3 });
		BTDecorator d2 = new BTDecoratorUntilFail (s2);

		BTSequence s1 = new BTSequence (new IBTTask[] { a1, d1, a2, d2, a4 });

		BehaviorTree AI = new BehaviorTree(s1);

		AI.Run (); // System stops HERE!
	}

	// GIMMICS

	private void OnValidate () {
		Transform t = transform.Find ("Long Range");
		if (t != null) { // we assume it is a plane 
			t.localScale = new Vector3 (sensingRange / transform.localScale.x, 1f, sensingRange / transform.localScale.z) / 5f;
		}
		t = transform.Find ("Short Range");
		if (t != null) {
			t.localScale = new Vector3 (fearRange / transform.localScale.x, 1f, fearRange / transform.localScale.z) / 5f;
		}
	}
	// CONDITIONS

	private Transform target;
	
	public bool EnemyOutsideRange() {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag(targetTag)) {
			if ((go.transform.position - transform.position).magnitude < sensingRange) {
				target = go.transform;
				return false;
			}
		}
		return true;
	}

	public bool EnemyNotTooClose() {
		return (target.transform.position - transform.position).magnitude > fearRange;
	}

	// ACTIONS

	public bool Hide() {
		GetComponent<MeshRenderer> ().enabled = false;
		return true;
	}


	public bool Show() {
		GetComponent<MeshRenderer> ().enabled = true;
		return true;
	}

	public bool Chase() {
		GetComponent<SeekBehaviour> ().destination = target;
		GetComponent<FleeBehaviour> ().destination = null;
		return true;
	}

	public bool Flee() {
		GetComponent<SeekBehaviour> ().destination = null;
		GetComponent<FleeBehaviour> ().destination = target;
		return true;
	}
}
