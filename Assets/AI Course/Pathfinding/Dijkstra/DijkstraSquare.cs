using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DijkstraSquare : MonoBehaviour {

	public int x = 10;
	public int y = 10;
	[Range(0f, 1f)] public float edgeProbability = 0.75f;
	public int RandomSeed = 0;
	public Color edgeColor = Color.red;
	public float gap = 3f;
    public Material startMaterial = null;
	public Material trackMaterial = null;
    public Material endMaterial = null;

	// what to put on the scene, not really meaningful
	public GameObject sceneObject;

	protected Node[,] matrix;
	protected Graph g;
	
	void Start () {
		if (sceneObject != null) {

			// initialize randomness, so experiments can be repeated
			if (RandomSeed == 0) RandomSeed = (int)System.DateTime.Now.Ticks;
			Random.InitState (RandomSeed);

			// create a x * y matrix of nodes (and scene objects)
			matrix = CreateGrid(sceneObject, x, y, gap);

			// create a graph and put random edges inside
			g = new Graph();
			CreateLabyrinth(g, matrix, edgeProbability);

			// ask dijkstra to solve the problem
			Edge[] path = DijkstraSolver.Solve(g, matrix[0, 0], matrix [x - 1, y - 1]);

			// check if there is a solution
			if (path.Length == 0) {
				UnityEditor.EditorUtility.DisplayDialog ("Sorry", "No solution", "OK");
			} else {
				// if yes, outline it
                OutlinePath(path, startMaterial, trackMaterial, endMaterial);
			}
		}
	}

	protected virtual Node[,] CreateGrid(GameObject o, int x, int y, float gap) {
		Node[,] matrix = new Node[x,y];
		for (int i = 0; i < x; i += 1) {
			for (int j = 0; j < y; j += 1) {
				matrix[i, j] = new Node("" + i + "," +j, Instantiate(o));
				matrix[i, j].sceneObject.name = o.name;
				matrix[i, j].sceneObject.transform.position = 
					transform.position + 
                    transform.right * gap * (i - ((x - 1) / 2f)) + 
                    transform.forward * gap * (j - ((y - 1) / 2f));
				matrix[i, j].sceneObject.transform.rotation = transform.rotation;
			}
		}
		return matrix;
	}
	
	protected void CreateLabyrinth(Graph g, Node[,] crossings, float threshold) {
		for (int i = 0; i < crossings.GetLength(0); i += 1) {
			for (int j = 0; j < crossings.GetLength(1); j += 1) {
				g.AddNode(crossings[i, j]);
				foreach (Edge e in RandomEdges(crossings, i, j, threshold)) {
					g.AddEdge(e);
				}
			}
		}
	}

	protected void OutlinePath(Edge[] path, Material sm, Material tm, Material em) {
		if (path.Length == 0) return;
		foreach (Edge e in path) {
			e.to.sceneObject.GetComponent<MeshRenderer>().material = tm;
		}
		path[0].from.sceneObject.GetComponent<MeshRenderer>().material = sm;
        path[path.Length - 1].to.sceneObject.GetComponent<MeshRenderer>().material = em;
	}

    protected Edge[] RandomEdges(Node[,] matrix, int x, int y, float threshold) {
        List<Edge> result = new List<Edge>();
        if (x != 0 && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x - 1, y], Distance(matrix[x, y], matrix[x - 1, y])));

        if (y != 0 && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x, y - 1], Distance(matrix[x, y], matrix[x, y - 1])));

        if (x != (matrix.GetLength(0) - 1) && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x + 1, y], Distance(matrix[x, y], matrix[x + 1, y])));

        if (y != (matrix.GetLength(1) - 1) && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x, y + 1], Distance(matrix[x, y], matrix[x, y + 1])));

        return result.ToArray();
    }

    protected virtual float Distance(Node from, Node to) {
        return 1f;
    }
	
	void OnDrawGizmos() {
		if (matrix != null) {
			Gizmos.color = edgeColor;
			for (int i = 0; i < x; i += 1) {
				for (int j = 0; j < y; j += 1) {
					foreach (Edge e in g.getConnections(matrix[i, j])) {
						Vector3 from = e.from.sceneObject.transform.position;
						Vector3 to = e.to.sceneObject.transform.position;
						Gizmos.DrawSphere(from + ((to - from) * .2f), .2f);
						Gizmos.DrawLine (from, to); 
					}
				}
			}
		}
	}

}
