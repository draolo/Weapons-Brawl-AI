using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AStarSquareBasic : DijkstraSquare {

	void Start () {
		if (sceneObject != null) {

			// create a x * y matrix of nodes (and scene objects)
			// edge weight is now the geometric distance (gap)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask A* to solve the problem
			Edge[] path = AStarSolver.Solve(g, matrix[0, 0], matrix[x - 1, y - 1], EuclideanEstimator);

			// check if there is a solution
			if (path.Length == 0) {
				UnityEditor.EditorUtility.DisplayDialog ("Sorry", "No solution", "OK");
			} else {
				// if yes, outline it
                OutlinePath(path, startMaterial, trackMaterial, endMaterial);
			}
		}
	}

	private float EuclideanEstimator(Node from, Node to) {
		return (from.sceneObject.transform.position - to.sceneObject.transform.position).magnitude;
	}
}

