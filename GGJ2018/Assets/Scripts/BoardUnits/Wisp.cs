using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : GameUnit {

	GameObject owner;
	GameObject target;
	bool targetAchieved = false;
	bool homeReached = false;

	public void Prime(GameObject o, GameObject gt){
		owner = o;
		target = gt;
	}

	public void ScoutOrReturn(){
		if (targetAchieved) Return ();
		else Scout ();
	}

	void Scout(){
		pathfindFromTo (transform.position, target.transform.position);
	}

	void Return(){
		pathfindFromTo (transform.position, owner.transform.position);
	}

	//We're overriding this because wisps can fly over impassable terrain.
	protected override void pathfindFromTo(Vector3 start, Vector3 target){
		if (!asleep && !travelling) {//GAME UNITS WILL NOT PATHFIND WHILE ASLEEP
			if (start == target) return;
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
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Up], closedNodes)) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Up], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Right] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Right], closedNodes)) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Right], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Down] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Down], closedNodes)) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Down], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Left] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Left], closedNodes)) {
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
					//Debug.Log ("We're dead Jim! (No paths found.)");
					pathExists = false;
					break;
					//If the node is the goal, we're good to go.
				} else if (currentNode.me.transform.position == target) {
					//Debug.Log ("Found it! (Path Route Found)");
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

	protected override void TravelAlongTiles(){
		if (movesRemaining == 0) {
			pathing.Clear();
		}

		if (pathing.Count >= 1){
			transform.position = Vector3.Lerp (transform.position, pathing[pathing.Count-1].transform.position + (pathing[pathing.Count-1].transform.position - transform.position).normalized*1.3f, Time.deltaTime * tileTransitionSpeed);
			Debug.DrawRay (transform.position, Vector3.up * 5, Color.yellow, 1.0f);
			GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (this);
			travelling = true;
			if (GameManager.instance.WhoseTurnIsIt () == gameObject) {
				GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (this);
			}

			if (pathing [pathing.Count - 1].gameObject == target)
				targetAchieved = true;

			if (pathing [pathing.Count - 1].transform.position == RoundVectorToFives (owner.transform.position))
				homeReached = true;
			
			if (Vector3.Distance (transform.position, pathing[pathing.Count-1].transform.position) < tileSnapDistance) {
				transform.position = pathing[pathing.Count-1].transform.position;
				pathing.RemoveAt (pathing.Count-1);
				movesRemaining--;
				travelling = false;
			}

			if (pathing.Count >= 1) Debug.DrawRay (pathing[pathing.Count-1].transform.position, Vector3.up * 8f, Color.white);

			if (homeReached) {
				owner.GetComponent<Player> ().WispReturn(this.gameObject);
				GameManager.instance.RemoveWispFromList (this.gameObject);
				Destroy (gameObject);
			}
		}
	}
}
