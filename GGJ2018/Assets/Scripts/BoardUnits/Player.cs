using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : GameUnit {

	protected override void FixedUpdate(){
		base.FixedUpdate ();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
	}

	public void TryMovement(GameObject target){
		pathfindFromTo (transform.position, target.transform.position);
	}

	//An Unsafe Pathfind specifically for transmitters.
	void TryActivateTransmitter(Vector3 target)
	{
		//GAME UNITS WILL NOT PATHFIND WHILE ASLEEP (also because this function is special: THE TARGET MUST BE A TRANSMITTER.)
		if (!asleep && !travelling && GameTileManager.instance.GetTileFromPos(target).type == GameTile.TileType.Transmitter) {
			if (RoundVectorToFives(transform.position) == target || GameTileManager.instance.GetTileFromPos (RoundVectorToFives (target)).type == GameTile.TileType.Impassible)
				return;
			List <Node> foundNodes = new List<Node> ();
			List <Node> closedNodes = new List<Node> ();
			bool keepSearching = true;
			bool pathExists = true;
			Node currentNode;

			foundNodes.Add (new Node (GameTileManager.instance.GetTileFromPos (RoundVectorToFives (transform.position)), null));
			currentNode = foundNodes [0];

			//ONE TIME CHECK FOR TRANSMITTERS NEARBY
			if(movesRemaining > 0){
				if (currentNode.me.tiles [(int)GameTile.Direction.Up] != null && currentNode.me.tiles [(int)GameTile.Direction.Up].type == GameTile.TileType.Transmitter) {
					GameManager.instance.ActivateTransmitter (currentNode.me.tiles [(int)GameTile.Direction.Up]);
					movesRemaining--;
				} else if (currentNode.me.tiles [(int)GameTile.Direction.Right] != null && currentNode.me.tiles [(int)GameTile.Direction.Right].type == GameTile.TileType.Transmitter) {
					GameManager.instance.ActivateTransmitter (currentNode.me.tiles [(int)GameTile.Direction.Right]);
					movesRemaining--;
				} else if (currentNode.me.tiles [(int)GameTile.Direction.Down] != null && currentNode.me.tiles [(int)GameTile.Direction.Down].type == GameTile.TileType.Transmitter) {
					GameManager.instance.ActivateTransmitter (currentNode.me.tiles [(int)GameTile.Direction.Down]);
					movesRemaining--;
				} else if (currentNode.me.tiles [(int)GameTile.Direction.Left] != null && currentNode.me.tiles [(int)GameTile.Direction.Left].type == GameTile.TileType.Transmitter) {
					GameManager.instance.ActivateTransmitter (currentNode.me.tiles [(int)GameTile.Direction.Left]);
					movesRemaining--;
				}
			}

			//Check all tiles that are neighbouring the one the player is currently on. 
			//Add them to the list of nodes to scan. (The start node is included by default).
			while (keepSearching && pathExists) {
				if (currentNode.me.tiles [(int)GameTile.Direction.Up] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Up], closedNodes)
					&& currentNode.me.tiles [(int)GameTile.Direction.Up].type != GameTile.TileType.Impassible) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Up], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Right] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Right], closedNodes)
					&& currentNode.me.tiles [(int)GameTile.Direction.Right].type != GameTile.TileType.Impassible) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Right], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Down] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Down], closedNodes)
					&& currentNode.me.tiles [(int)GameTile.Direction.Down].type != GameTile.TileType.Impassible) {
					foundNodes.Add (new Node (currentNode.me.tiles [(int)GameTile.Direction.Down], currentNode));
				}
				if (currentNode.me.tiles [(int)GameTile.Direction.Left] != null
					&& !DoesNodeListContainGameTile (currentNode.me.tiles [(int)GameTile.Direction.Left], closedNodes)
					&& currentNode.me.tiles [(int)GameTile.Direction.Left].type != GameTile.TileType.Impassible) {
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
			finalPath.RemoveAt (0);//THE KEY (we remove the transmitter tile from the pathing because it's supposed to be impassable, thus we move to the closest available space.
			pathing = finalPath;
		}
	}

	protected override void Kill(){
		if (isAlive()) transform.Rotate (90,0,0);
		base.Kill ();
		GameManager.instance.CheckIfAllDead ();
	}
}
