using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour {
	public enum TileType { SpawnTile, Impassible, Transmitter, Passable }
	public enum FoliageType { Grass, Rock, Sand }
	public enum Direction { UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, Left } 

	public TileType type = TileType.Passable;
	public FoliageType foliageType;

	public GameTile[] tiles = new GameTile[8];

	public void Initialize() {
		RaycastHit outHit = new RaycastHit ();

		//UP LEFT
		if (Physics.Raycast (transform.position, Vector3.forward+Vector3.left, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.UpLeft] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//UP
		if (Physics.Raycast (transform.position, Vector3.forward, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.Up] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//UP Right
		if (Physics.Raycast (transform.position, Vector3.forward+Vector3.right, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.UpRight] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//RIGHT
		if (Physics.Raycast (transform.position, Vector3.right, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.Right] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//DOWN RIGHT
		if (Physics.Raycast (transform.position, Vector3.back+Vector3.right, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.DownRight] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//DOWN
		if (Physics.Raycast (transform.position, Vector3.back, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.Down] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//DOWN LEFT
		if (Physics.Raycast (transform.position, Vector3.back+Vector3.left, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.DownLeft] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		//LEFT
		if (Physics.Raycast (transform.position, Vector3.left, out outHit, 5.0f)) {
			if (outHit.collider.GetComponent<GameTile> () != null) {
				tiles[(int)Direction.Left] = outHit.collider.GetComponent<GameTile> ();
			}
		}

		if (tiles[(int)Direction.Left] == null || tiles[(int)Direction.Right] == null || tiles[(int)Direction.Up] == null || tiles[(int)Direction.Down] == null) {
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

		spawnTile = Instantiate (spawnTile, transform);
		spawnTile.transform.rotation = Quaternion.Euler(new Vector3 (0, (float)GameTileManager.instance.RandomRotationAngle(), 0));
	}

	public bool CheckSides() {
		for (int i = 0; i < tiles.Length; i++) {
			if (i % 2 == 0) {
				if (tiles [i].type == TileType.Impassible || tiles [i] == null) {
					return false;
				}
			}
		}
		return true;
	}

	public bool CheckSidesAndDiags(int depth) {														//How far down the hole do we go?
		int depthCount = 0;
		GameTile currentCheckTile = null;
		while (depthCount < depth) {
			if(currentCheckTile!=null) {
				if(currentCheckTile.type!=TileType.Transmitter) {
					//currentCheckTile = left;
				}
			}
			depthCount++;
		}
		return false;
	}

	public void SetImpassible() {
		type = TileType.Impassible;
		GameObject tempGo = Instantiate(GameTileManager.instance.impassiblePrefab, transform);
		tempGo.transform.position += Vector3.up;
		//Spawn on a rock or something
	}
}
