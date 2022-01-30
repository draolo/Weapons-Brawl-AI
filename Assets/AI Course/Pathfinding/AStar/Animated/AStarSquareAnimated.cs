using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquareAnimated : AStarSquare {

	public Material currentMaterial = null;
	public float delay = 0.2f;

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

			// ask A* to setup the problem
			AStarStepSolver.immediateStop = stopAtFirstHit;
			AStarStepSolver.Init (g, matrix [0, 0], matrix [x - 1, y - 1], myHeuristics [(int)heuristicToUse]);

			// put the solver in a coroutine
			StartCoroutine(AnimateSolution(delay));
		}
	}

	private IEnumerator AnimateSolution (float pause) {
		while (AStarStepSolver.Step ()) {
			OutlineSet (AStarStepSolver.visited, visitedMaterial);
			OutlineNode (AStarStepSolver.current, currentMaterial);
			yield return new WaitForSeconds (pause);
		}
		Edge [] path = AStarStepSolver.solution;
		// check if there is a solution
		if (path.Length == 0) {
			UnityEditor.EditorUtility.DisplayDialog ("Sorry", "No solution", "OK");
		} else {
			// if yes, outline it
			OutlinePath (path, startMaterial, trackMaterial, endMaterial);
		}
	}

	protected void OutlineNode (Node n, Material m) {
		if (n != null)
			n.sceneObject.GetComponent<MeshRenderer> ().material = m;
	}

}