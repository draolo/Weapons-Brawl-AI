using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquare : DijkstraSquare {

	public bool stopAtFirstHit = false;
	public Material visitedMaterial = null;

	public enum Heuristics { Euclidean, Manhattan, Bisector, FullBisector, Zero };
	public HeuristicFunction [] myHeuristics = { EuclideanEstimator, ManhattanEstimator, BisectorEstimator,
												 FullBisectorEstimator, ZeroEstimator };
	public Heuristics heuristicToUse = Heuristics.Euclidean;

	void Start () {
		if (sceneObject != null) {

			// initialize randomness, so experiments can be repeated
			if (RandomSeed == 0) RandomSeed = (int)System.DateTime.Now.Ticks;
			Random.InitState (RandomSeed);

			// create a x * y matrix of nodes (and scene objects)
			// edge weight is now the geometric distance (gap)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask A* to solve the problem
			AStarSolver.immediateStop = stopAtFirstHit;
			Edge [] path = AStarSolver.Solve (g, matrix [0, 0], matrix [x - 1, y - 1], myHeuristics [(int) heuristicToUse]);

			// Outline visited nodes
			OutlineSet(AStarSolver.visited, visitedMaterial);

			// check if there is a solution
			if (path.Length == 0) {
				UnityEditor.EditorUtility.DisplayDialog ("Sorry", "No solution", "OK");
			} else {
				// if yes, outline it
                OutlinePath(path, startMaterial, trackMaterial, endMaterial);
			}
		}
	}

	protected void OutlineSet(List<Node> set, Material m) {
		if (m == null) return;
		foreach (Node n in set) {
			n.sceneObject.GetComponent<MeshRenderer>().material = m;
		}
	}

	protected static float EuclideanEstimator(Node from, Node to) {
		return (from.sceneObject.transform.position - to.sceneObject.transform.position).magnitude;
	}

	protected static float ManhattanEstimator(Node from, Node to) {
		return (
				Mathf.Abs(from.sceneObject.transform.position.x - to.sceneObject.transform.position.x) +
				Mathf.Abs(from.sceneObject.transform.position.z - to.sceneObject.transform.position.z)
			);
	}

	protected static float BisectorEstimator(Node from, Node to) {
		Ray r = new Ray (Vector3.zero, to.sceneObject.transform.position);
		return Vector3.Cross(r.direction, from.sceneObject.transform.position - r.origin).magnitude;
	}

	protected static float FullBisectorEstimator(Node from, Node to) {
		Ray r = new Ray (Vector3.zero, to.sceneObject.transform.position);
		Vector3 toBisector = Vector3.Cross (r.direction, from.sceneObject.transform.position - r.origin);
		return toBisector.magnitude + (to.sceneObject.transform.position - ( from.sceneObject.transform.position + toBisector ) ).magnitude ;
	}

	protected static float ZeroEstimator (Node from, Node to) { return 0f; }

}
