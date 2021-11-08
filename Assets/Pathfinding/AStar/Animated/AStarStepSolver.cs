using System;
using System.Collections;
using System.Collections.Generic;

public class AStarStepSolver : AStarSolver {

	public static Edge[] solution;
	public static Node current;
	
	static Graph graph;
	static Node startNode;
	static Node goalNode;
	static HeuristicFunction estimator;

	public static void Init(Graph g, Node start, Node goal, HeuristicFunction heuristic) {
		
		graph = g;
		startNode = start;
		goalNode = goal;
		estimator = heuristic;
		
		// setup sets (1)
		visited = new List<Node>();
		unvisited = new List<Node> (g.getNodes ());
		
		// set all node tentative distance (2)
		status = new Dictionary<Node, NodeExtension> ();
		foreach (Node n in unvisited) {
			NodeExtension ne = new NodeExtension();
			ne.distance = ( n == start ? 0f : float.MaxValue ); // infinite
			ne.estimate = ( n == start ? heuristic(start, goal) : float.MaxValue );
			status [n] = ne;
		}
		solution = null;
	}

	public static bool Step() {

		// if we are not done yet
		if (!CheckSearchComplete(goalNode, unvisited)) {

			// select net current node (3)
			current = GetNextNode();

			// if graph is not partitioned
			if (status [current].distance != float.MaxValue) {

				// assign weight and predecessor to all neighbors (4)
				foreach (Edge e in graph.getConnections(current)) {
					if (status[current].distance + e.weight < status[e.to].distance) {
						NodeExtension ne = new NodeExtension();
						ne.distance = status[current].distance + e.weight;
						ne.estimate = ne.distance + estimator(current, goalNode);
						ne.predecessor = e;
						status[e.to] = ne;
						// unlike Dijkstra's, we can discover better paths here
						if (visited.Contains(e.to)) {
							unvisited.Add(e.to);
							visited.Remove(e.to);
						}
					}
				}

				// mark current node as visited (5)
				visited.Add(current);
				unvisited.Remove(current);
				return true;
			}
		}
		
		if (status [goalNode].distance == float.MaxValue) {
			// goal is unreachable
			solution = new Edge[0]; 
		} else {
			// walk back and build the shortest path (7)
			List<Edge> result = new List<Edge> ();
			Node walker = goalNode;

			while (walker != startNode) {
				result.Add (status [walker].predecessor);
				walker = status [walker].predecessor.from;
			}
			result.Reverse ();
			solution = result.ToArray ();
		}
		return false;
	}
}
















