
using UnityEngine;

public class Node {

	public string description;
	public GameObject sceneObject;

	public Node(string description, GameObject o = null) {
		this.description = description;
		this.sceneObject = o;
	}
}