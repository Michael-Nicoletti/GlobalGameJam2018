using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour {
	public enum TileType { SpawnTile, Impassible, Transmitter, Passable }
	public enum FoliageType { Grass, Rock, Sand }

	public TileType type = TileType.Passable;
	public FoliageType foliageType;
	public GameTile left;
	public GameTile right;
	public GameTile up;
	public GameTile down;
	public int moveCost = 1;

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

		if (left == null || right == null || up == null || down == null) {
			foliageType = FoliageType.Sand;
		} else {
			int random = Random.Range (0, 3);
			if (random == 0) {
				foliageType = FoliageType.Grass;
			} else if (random == 1) {
				foliageType = FoliageType.Rock;
			} else {
				foliageType = FoliageType.Sand;
			}
		}

		GameObject spawnTile = GameTileManager.instance.GetRandomTilePrefabFromType (foliageType);

		Instantiate (spawnTile, transform);
	}
}
