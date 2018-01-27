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

public class GameUnit : MonoBehaviour {

	[SerializeField]GameObject moveMarkerPrefab;
	[SerializeField]int moveEnergy = 2;
	[SerializeField]int tileTransitionSpeed = 1;

	protected int movesRemaining = 2;
	protected float tileSnapDistance = 0.3f;

	List<GameTile> pathing = new List<GameTile>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		travelAlongTiles (pathing);
	}

	//HOW TO USE PATHFINDING
	//Run "pathfindFromTo," and feed in the start and target tiles by their vector.
	//The unit will automatically attempt to travel along those tiles.
	//Modify units snapping to tiles via "tileSnapDistance
	protected List<GameTile> pathfindFromTo(Vector3 start, Vector3 target)
	{
		if (start == target) return null;

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
			if (currentNode.me.tiles[(int)GameTile.Direction.Up] != null && !closedNodes.Contains(new Node(currentNode.me.tiles[(int)GameTile.Direction.Up], currentNode)) && currentNode.me.tiles[(int)GameTile.Direction.Up].type != GameTile.TileType.Impassible) {
				Debug.Log (currentNode.me.tiles[(int)GameTile.Direction.Up].type);
				foundNodes.Add (new Node(currentNode.me.tiles[(int)GameTile.Direction.Up], currentNode));
			}
			if (currentNode.me.tiles[(int)GameTile.Direction.Right] != null && !closedNodes.Contains(new Node(currentNode.me.tiles[(int)GameTile.Direction.Right], currentNode)) && currentNode.me.tiles[(int)GameTile.Direction.Right].type != GameTile.TileType.Impassible) {
				foundNodes.Add (new Node(currentNode.me.tiles[(int)GameTile.Direction.Right], currentNode));
			}
			if (currentNode.me.tiles[(int)GameTile.Direction.Down] != null && !closedNodes.Contains(new Node(currentNode.me.tiles[(int)GameTile.Direction.Down], currentNode)) && currentNode.me.tiles[(int)GameTile.Direction.Down].type != GameTile.TileType.Impassible) {
				foundNodes.Add (new Node(currentNode.me.tiles[(int)GameTile.Direction.Down], currentNode));
			}
			if (currentNode.me.tiles[(int)GameTile.Direction.Left] != null && !closedNodes.Contains(new Node(currentNode.me.tiles[(int)GameTile.Direction.Left], currentNode)) && currentNode.me.tiles[(int)GameTile.Direction.Left].type != GameTile.TileType.Impassible) {
				foundNodes.Add (new Node(currentNode.me.tiles[(int)GameTile.Direction.Left], currentNode));
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
				Debug.DrawRay (n.me.transform.position, Vector3.up * 5f, Color.yellow, 1f);
			}

			foreach (Node n in closedNodes) {
				Debug.DrawRay (n.me.transform.position, Vector3.up * 5f, Color.red, 1f);
			}
		}

		//If we've reached this point, we have the solution. Trace back the solution pathfound. 
		//Cycle through all nodes in the closed list via their parents.
		List<GameTile> finalPath = new List<GameTile>();
		Node w = currentNode;

		while ( w != null) {
			finalPath.Add (w.me);
			w = w.parent;
		}

		pathing = finalPath;
		return finalPath;
	}

	//Travel along the
	protected void travelAlongTiles(List<GameTile> p){
		if (pathing[0] != null){
			transform.position = Vector3.Lerp (transform.position, pathing[0].transform.position, Time.deltaTime * tileTransitionSpeed);
			if (Vector3.Distance (transform.position, pathing[0].transform.position) < tileSnapDistance) {
				transform.position = pathing [0].transform.position;
				pathing.RemoveAt (0);
			}

		}
	}

	//Rounds all vector numbers to multiples of three.
	Vector3 roundVectorToThrees(Vector3 n) {
		return new Vector3 ( Mathf.Round (transform.position.x) / 3 * 3, Mathf.Round (transform.position.y) / 3 * 3, Mathf.Round (transform.position.z) / 3 * 3);
	}
}
