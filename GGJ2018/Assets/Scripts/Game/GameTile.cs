using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour {
	public enum TileType { SpawnTile, Impassible, Transmitter, Passable }

	public TileType type = TileType.Passable;
	public GameTile left;
	public GameTile right;
	public GameTile up;
	public GameTile down;

	public void Initialize() {
		RaycastHit outHit = new RaycastHit ();

		if (Physics.Raycast (transform.position, Vector3.right, out outHit, 3.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				right = outHit.collider.GetComponent<GameTile> ();
			}
		}

		if (Physics.Raycast (transform.position, Vector3.left, out outHit, 3.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				left = outHit.collider.GetComponent<GameTile> ();
			}
		}

		if (Physics.Raycast (transform.position, Vector3.forward, out outHit, 3.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				up = outHit.collider.GetComponent<GameTile> ();
			}
		}

		if (Physics.Raycast (transform.position, Vector3.back, out outHit, 3.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				down = outHit.collider.GetComponent<GameTile> ();
			}
		}
	}
}
