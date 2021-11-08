
using System;
using System.Collections;
using System.Collections.Generic;

public class DijkstraSolver {
	
	// two set of nodes (1)

	public static List<Node> visited;
	public static List<Node> unvisited;

	// data structures to extend nodes (2)

	protected struct NodeExtension {
		public float distance;
		public Edge predecessor;
	}

	protected static Dictionary<Node, NodeExtension> status;

	public static Edge[] Solve(Graph g, Node start, Node goal) {

		// setup sets (1)
		visited = new List<Node>();
		unvisited = new List<Node> (g.getNodes ());

		// set all node tentative distance (2)
		status = new Dictionary<Node, NodeExtension> ();
		foreach (Node n in unvisited) {
			NodeExtension ne = new NodeExtension();
			ne.distance = ( n == start ? 0f : float.MaxValue ); // infinite
            // -1 could be a better choice, but would make code more complex later
			status [n] = ne;
		}

		// iterate until all nodes are visited (6)
		while (unvisited.Count > 0) {
			// select next current node (3)
			Node current = GetNextNode();

			if (status[current].distance == float.MaxValue) break; // graph is partitioned

			// assign weight and predecessor to all neighbors (4)
			foreach (Edge e in g.getConnections(current)) {
				if (status[current].distance + e.weight < status[e.to].distance) {
					NodeExtension ne = new NodeExtension();
					ne.distance = status[current].distance + e.weight;
					ne.predecessor = e;
					status[e.to] = ne;
				}
			}
			// mark current node as visited (5)
			visited.Add(current);
			unvisited.Remove(current);
		}

		if (status [goal].distance == float.MaxValue) return new Edge[0]; // goal is unreachable

		// walk back and build the shortest path (7)
		List<Edge> result = new List<Edge> ();
		Node walker = goal;

		while (walker != start) {
			result.Add(status[walker].predecessor);
			walker = status[walker].predecessor.from;
		}
        result.Reverse();
        return result.ToArray();
	}

	// iterate on the unvisited set and get the lowest weight
	protected static Node GetNextNode() {
		Node candidate = null;
		float cDistance = float.MaxValue;
		foreach (Node n in unvisited) {
			if (candidate == null || cDistance > status[n].distance) {
				candidate = n;
				cDistance = status[n].distance;
			}
		}
		return candidate;
	}

}