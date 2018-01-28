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

	[Header("Animator Info"), Range(0,1)]
	public float rotationProgress = 1;
	public int[] rotationStops = new int[2];
	public bool inRange = false;

	private Animator anim;
	private Vector3 startRot;
	private Vector3 endRot;
	private float prevRotationProgress;

	public void Initialize() {
		anim = GetComponent<Animator>();
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

		if (GameTileManager.instance.baseTilePrefabs [(int)foliageType].foliagePrefabs.Count > 0) {
			Instantiate (GameTileManager.instance.baseTilePrefabs [(int)foliageType].foliagePrefabs [Random.Range (0, GameTileManager.instance.baseTilePrefabs [(int)foliageType].foliagePrefabs.Count)], transform);
		}
		endRot = new Vector3(rotationStops[1], transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
		startRot = new Vector3(rotationStops[0], transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
	}

	public bool CheckSides() {
		for (int i = 0; i < tiles.Length; i++) {
			if (i % 2 == 0) {
				if (tiles [i] == null || tiles [i].type == TileType.Impassible) {
					return false;
				}
			}
		}
		return true;
	}

	void Update() {
		UpdateAnim ();
		if (rotationProgress != prevRotationProgress) {
			transform.rotation = Quaternion.Lerp (Quaternion.Euler (startRot), Quaternion.Euler (endRot), rotationProgress);	
			prevRotationProgress = rotationProgress;
		}

	}

	public void UpdateAnim() {
		if (anim != null) {
			//anim.SetBool ("active", );
		}
	}

	public bool CheckTransmitterPlacement(int depth) {														//How far down the hole do we go?
		int depthCount = 0;
		GameTile currentCheckTile = null;
		for (int i = 0; i < tiles.Length; i++) {
			depthCount = 0;
			currentCheckTile = tiles [i];
			while (depthCount < depth) {
				if (currentCheckTile == null) {
					break;
				}
				if (currentCheckTile.type == TileType.Transmitter) {
					return false;
				} else {
					Debug.DrawRay (currentCheckTile.transform.position, Vector3.up, Color.red, 10.0f);
					currentCheckTile = currentCheckTile.tiles [i]; 
				}
				depthCount++;
			}
		}
		return true;
	}

	public bool CheckSpawnPlacement(int depthTransmitter, int depthSpawnpoints) {
		int depthCount = 0;
		GameTile currentCheckTile = null;
		for (int i = 0; i < tiles.Length; i++) {
			depthCount = 0;
			currentCheckTile = tiles [i];
			while (depthCount < depthSpawnpoints) {
				if (currentCheckTile == null) {
					break;
				}
				if (depthCount < depthTransmitter) {
					if (currentCheckTile.type == TileType.Transmitter) {
						return false;
					}
				}
				if (currentCheckTile.type == TileType.SpawnTile) {
					return false;
				} else {
					Debug.DrawRay (currentCheckTile.transform.position, Vector3.up, Color.red, 10.0f);
					currentCheckTile = currentCheckTile.tiles [i]; 
				}
				depthCount++;
			}
		}
		return true;

	}

	public void SetImpassible() {
		type = TileType.Impassible;
		GameObject tempGo = Instantiate(GameTileManager.instance.impassiblePrefab, transform);
	}

	public void SetTransmitter() { 
		type = TileType.Transmitter;
		Debug.DrawRay (transform.position, Vector3.up * 10, Color.green, 10.0f);
		GameObject tempGo = Instantiate(GameTileManager.instance.transmitterPrefab, transform);
	}

	public void SetSpawnTile() {
		type = TileType.SpawnTile;
		Debug.DrawRay (transform.position, Vector3.up * 10, Color.magenta, 10.0f);
	}
}
