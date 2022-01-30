
// Transition between two nodes
public class Edge {
	
	public Node from;
	public Node to;
	public float weight;
	
    // Default weight could also be 0, but 1 will give a better animation effect
	public Edge(Node from, Node to, float weight = 1f) {
		this.from = from;
		this.to = to;
		this.weight = weight;
	}

}