using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit : MonoBehaviour {

	[SerializeField] protected int maxMoves = 2;
	[SerializeField] protected int tileTransitionSpeed = 3;

	protected int health = 2;
	protected int movesRemaining = 0;
	protected float tileSnapDistance = 0.1f;
	protected bool asleep = true;
	protected bool travelling = false;
	protected bool alive = true;

	protected List<GameTile> pathing = new List<GameTile>();

	protected class Node{
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

	protected virtual void Awake() {
		
	}

	// Use this for initialization
	protected virtual void Start () {
		//GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (transform.position);
		RefreshMoves ();
	}
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {
		if (!asleep) TravelAlongTiles ();


	}

	//HOW TO USE PATHFINDING
	//Run "pathfindFromTo," and feed in the start and target tiles by their vector.
	//The unit will automatically attempt to travel along those tiles.
	//Modify units snapping to tiles via "tileSnapDistance
	protected virtual void pathfindFromTo(Vector3 start, Vector3 target)
	{
		if (!asleep && !travelling) {//GAME UNITS WILL NOT PATHFIND WHILE ASLEEP
			if (start == target || GameTileManager.instance.GetTileFromPos (RoundVectorToFives (target)).type == GameTile.TileType.Impassible
			   || GameTileManager.instance.GetTileFromPos (RoundVectorToFives (target)).type == GameTile.TileType.Transmitter)
				return;
			List <Node> foundNodes = new List<Node> ();
			List <Node> closedNodes = new List<Node> ();
			bool keepSearching = true;
			bool pathExists = true;
			Node currentNode;

			foundNodes.Add (new Node (GameTileManager.instance.GetTileFromPos (RoundVectorToFives (transform.position)), null));
			currentNode = foundNodes [0];

			//Check all tiles that are neighbouring the one the player is currently on. 
			//Add them to the list of nodes to scan. (The start node is included by default).
			while (keepSearching && pathExists) {
				if (currentNode.me.tiles [(int)GameTile.Direction.Up] != null
				   && !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Up], closedNodes)
				   && currentNode.me.tiles [(int)GameTile.Direction.Up].type != GameTile.TileType.Impassible
				   && currentNode.me.tiles [(int)GameTile.Direction.Up].type != GameTile.TileType.Transmitter) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Up], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Right] != null
				   && !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Right], closedNodes)
				   && currentNode.me.tiles [(int)GameTile.Direction.Right].type != GameTile.TileType.Impassible
				   && currentNode.me.tiles [(int)GameTile.Direction.Right].type != GameTile.TileType.Transmitter) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Right], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Down] != null
				   && !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Down], closedNodes)
				   && currentNode.me.tiles [(int)GameTile.Direction.Down].type != GameTile.TileType.Impassible
				   && currentNode.me.tiles [(int)GameTile.Direction.Down].type != GameTile.TileType.Transmitter) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Down], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Left] != null
				   && !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Left], closedNodes)
				   && currentNode.me.tiles [(int)GameTile.Direction.Left].type != GameTile.TileType.Impassible
				   && currentNode.me.tiles [(int)GameTile.Direction.Left].type != GameTile.TileType.Transmitter) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Left], currentNode));
				}

				//Scan all found nodes for fitness.
				Node mostFit = new Node ();
				for (int i = 0; i <= foundNodes.Count - 1; i++) {
					if (mostFit.me == null || (Vector3.Distance (foundNodes [i].me.transform.position, target) < Vector3.Distance (mostFit.me.transform.position, target)) && !closedNodes.Contains (foundNodes [i])) {
						mostFit = foundNodes [i];
					}
				}

				//If we found a better node, it becomes the current node.
				if (mostFit.me != null)
					foundNodes.Remove (mostFit);
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
					Debug.DrawRay (n.me.transform.position, Vector3.up * 8f, Color.yellow, 0.5f);
				}

				foreach (Node n in closedNodes) {
					Debug.DrawRay (n.me.transform.position, Vector3.up * 8f, Color.red, 0.5f);
				}
			}

			//If we've reached this point, we have the solution. Trace back the solution pathfound. 
			//Cycle through all nodes in the closed list via their parents.
			List<GameTile> finalPath = new List<GameTile> ();
			Node w = currentNode;

			while (w != null) {
				finalPath.Add (w.me);
				w = w.parent;
			}
			finalPath.RemoveAt (finalPath.Count - 1);
			pathing = finalPath;
		}
	}

	//Travel along the tiles set in pathfindFromTo, EXPENDS GAME UNIT ENERGY.
	protected virtual void TravelAlongTiles(){
		if (movesRemaining == 0) {
			pathing.Clear();
		}

		if (pathing.Count >= 1){
			transform.position = Vector3.Lerp (transform.position, pathing[pathing.Count-1].transform.position + (pathing[pathing.Count-1].transform.position - transform.position).normalized*1.3f, Time.deltaTime * tileTransitionSpeed);
			Debug.DrawRay (transform.position, Vector3.up * 5, Color.yellow, 1.0f);
			GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (transform.position);
			travelling = true;
			if (GameManager.instance.WhoseTurnIsIt () == gameObject) {
				GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (transform.position);
			}
			if (Vector3.Distance (transform.position, pathing[pathing.Count-1].transform.position) < tileSnapDistance) {
				transform.position = pathing[pathing.Count-1].transform.position;
				pathing.RemoveAt (pathing.Count-1);
				movesRemaining--;
				travelling = false;
			}

			if (pathing.Count >= 1) Debug.DrawRay (pathing[pathing.Count-1].transform.position, Vector3.up * 8f, Color.white);
		}
	}

	public void RefreshMoves(){
		movesRemaining = maxMoves;
	}


	protected bool DoesNodeListContainGameTile(GameTile GT, List<Node> cl){
		foreach (Node n in cl) {
			if (n.me == GT)
				return true;
		}

		return false;
	}

	//Rounds all vector numbers to multiples of three.
	protected Vector3 RoundVectorToFives(Vector3 n) {
		return new Vector3 ( Mathf.Round (n.x) / 5 * 5, Mathf.Round (n.y) / 5 * 5, Mathf.Round (n.z) / 5 * 5);
	}

	public int GetMovesRemaining(){
		return movesRemaining;
	}

	public int GetMaxMoves(){
		return maxMoves;
	}

	public void WakeUp(){
		asleep = false;
	}

	public void Sleep(){
		asleep = true;
	}

	public bool isAlive(){
		return alive;
	}

	protected virtual void Kill(){
		alive = false;
	}

	protected void Damage(){
		health--;
		if (health <= 0) Kill ();
	}
}
