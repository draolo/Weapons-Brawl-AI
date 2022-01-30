using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DijkstraMountain : DijkstraSquare {

	protected override Node[,] CreateGrid(GameObject o, int x, int y, float gap) {
		Node[,] matrix = new Node[x,y];
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				matrix[i, j] = new Node("" + i + "," +j, Instantiate(o));
				matrix[i, j].sceneObject.name = o.name;
				matrix[i, j].sceneObject.transform.position = 
					transform.position + 
						Vector3.right * gap * (i - ((x - 1) / 2f)) + 
						Vector3.forward * gap * (j - ((y - 1) / 2f)) +
						Vector3.up * gap * (((i <= x / 2f ? i : x - i) + (j <= y / 2f ? j : y - j) ) / 2f) ;
				matrix[i, j].sceneObject.transform.rotation = transform.rotation;
			}
		}
		return matrix;
	}

	protected override float Distance(Node from, Node to) {
		float h = to.sceneObject.transform.position.y - from.sceneObject.transform.position.y;
		if (h <= 0) return 1;
		return 3;
	}

}
