using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node{
	public GameTile me;
	public unsafe Node parent;

	public Node (){
		me = null;
		parent = null;
	}

	public Node (GameTile m, Node p){
		me = m;
		parent = p;
	}

}

public class Player : MonoBehaviour {

	[SerializeField]
	GameObject moveMarkerPrefab;

	int movementSpeed = 2;
	int movesRemaining = 2;

	List<GameTile> pathing = new List<GameTile>();

	// Use this for initialization
	void Start () {
		IEnumerator coroutine = pathfindFromTo (roundVectorToThrees (transform.position), new Vector3 (0, 0, 15));
		StartCoroutine (coroutine);
	}
	
	// Update is called once per frame
	void Update () {
	}

	//Pathfind to the highlighted tile.
	IEnumerator pathfindFromTo(Vector3 start, Vector3 target)
	{
		if (start == target) yield break;

		List <Node> foundNodes = new List<Node>();
		List <Node> closedNodes = new List<Node>();
		bool keepSearching = true;
		bool pathExists = true;
		Node currentNode;

		Ray directPath = new Ray(transform.position, target);
		foundNodes.Add(new Node(GameTileManager.instance.GetTileFromPos (roundVectorToThrees(transform.position)), null));
		currentNode = foundNodes [0];

		//Check all tiles that are neighbouring the one the player is currently on. 
		//Add them to the list of nodes to scan. (The start node is included by default).
		while (keepSearching && pathExists) {
			if (currentNode.me.up != null && !closedNodes.Contains(new Node(currentNode.me.up, currentNode))) {
				foundNodes.Add (new Node(currentNode.me.up, currentNode));
			}
			if (currentNode.me.right != null && !closedNodes.Contains(new Node(currentNode.me.right, currentNode))) {
				foundNodes.Add (new Node(currentNode.me.right, currentNode));
			}
			if (currentNode.me.down != null && !closedNodes.Contains(new Node(currentNode.me.down, currentNode))) {
				foundNodes.Add (new Node(currentNode.me.down, currentNode));
			}
			if (currentNode.me.left != null && !closedNodes.Contains(new Node(currentNode.me.left, currentNode))) {
				foundNodes.Add (new Node(currentNode.me.left, currentNode));
			}

			//Scan all found nodes for fitness.
			Node mostFit = new Node();
			for (int i = 0; i <= foundNodes.Count - 1; i++) {
				if (Vector3.Distance (foundNodes [i].me.transform.position, target) < Vector3.Distance (currentNode.me.transform.position, target)) {
					mostFit = foundNodes [i];
				}
			}
				
			//If we found a better node, it becomes the current node.
			if (mostFit.me != null ) foundNodes.Remove (mostFit);
			currentNode = mostFit;
			closedNodes.Add (currentNode);

			//If we found no better nodes, then abort, we're stuck.
			if (currentNode.me == null) {
				Debug.Log ("We're dead Jim! (No paths found.)");
				pathExists = false;
				break;
				//If the node is the goal, we're good to go.
			} else if (currentNode.me.transform.position == target) {
				Debug.Log ("Found it! (Path Route Found)");
				keepSearching = false;
				break;
			} 

			//If we've reached this point the current node is not the goal, we're not stuck,
			//and we've assigned it as the "most fit." Let's keep looping.

			foreach (Node n in foundNodes) {
				Debug.DrawRay (n.me.transform.position, Vector3.up * 5f, Color.yellow, 0.6f);
			}

			foreach (Node n in closedNodes) {
				Debug.DrawRay (n.me.transform.position, Vector3.up * 5f, Color.red, 0.6f);
			}

			yield return new WaitForSeconds (0.5f);
		}
		if (pathExists){
			
		}
	}

	//Rounds all vector numbers to multiples of three.
	Vector3 roundVectorToThrees(Vector3 n) {
		return new Vector3 ( Mathf.Round (transform.position.x) / 3 * 3, Mathf.Round (transform.position.y) / 3 * 3, Mathf.Round (transform.position.z) / 3 * 3);
	}
}
