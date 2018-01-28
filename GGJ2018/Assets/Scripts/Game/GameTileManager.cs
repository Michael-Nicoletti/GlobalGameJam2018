using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileManager : MonoBehaviour {

	[System.Serializable]
	public class BaseTiles { 
		public GameTile.FoliageType type;
		public List<GameObject> basePrefabs;
		public List<GameObject> foliagePrefabs;
	}		

	public static GameTileManager instance;
	public GameObject[] tileModelPrefabs = new GameObject[2];
	public List<GameTile> gameTiles = new List<GameTile>();
	public List<BaseTiles> baseTilePrefabs = new List<BaseTiles>();
	[Header("")]
	public GameObject impassiblePrefab;
	public GameObject transmitterPrefab;
	public Vector2 minMaxImpassableTiles;
	public int numTransmitterPos;
	public int minSpaceBetweenTransmitters = 3;
	public int minSpaceBetweenSpawnpoints = 10;
	public int minSpaceBetweenSpawnpointsAndTransmitters = 5;

	private int[] rotations = new int[] { 0, 90, 180, 270 };

	void Awake() {
		instance = this;
	}

	void Start() {
		GameTile[] gameTileHolder = GetComponentsInChildren<GameTile> ();
		for (int i = 0; i < gameTileHolder.Length; i++) {
			gameTiles.Add (gameTileHolder [i]);
			gameTileHolder [i].Initialize ();
		}

		//Time to randomize and make some impassable tiles
		int numImpassableTiles = Mathf.RoundToInt(Random.Range(minMaxImpassableTiles.x, minMaxImpassableTiles.y));
		Vector3 checkCoordinates = Vector3.zero;
		for (int i = 0; i < numImpassableTiles; i++) {
			GameTile foundTile = GetTileFromPos (GetRandomPositionOnBoard());
			if (foundTile != null) {
				if (!foundTile.CheckSides ()) {
					i -= 1;
					continue;
				} else {
					foundTile.SetImpassible ();
				}
			} else {
				Debug.Log ("We didnt find anything "+checkCoordinates);
			}
		}

		for (int i = 0; i < numTransmitterPos; i++) {
			GameTile foundTile = GetTileFromPos (GetRandomPositionOnBoard ());
			if (foundTile.type == GameTile.TileType.Transmitter || foundTile.type == GameTile.TileType.Impassible) {
				i -= 1;
				continue;
			}
			if (!foundTile.CheckTransmitterPlacement (minSpaceBetweenTransmitters)) {
				i -= 1;
				continue;
			} else { foundTile.SetTransmitter(); }
		}

		for (int i = 0; i < 4; i++) {
			GameTile foundTile = GetTileFromPos (GetRandomPositionOnBoard ());
			if (foundTile.type != GameTile.TileType.Passable) {
				i -= 1;
				continue;
			}
			if (!foundTile.CheckSpawnPlacement (minSpaceBetweenSpawnpointsAndTransmitters, minSpaceBetweenSpawnpoints)) {
				i -= 1;
				continue;
			} else { foundTile.SetSpawnTile (); }
		}

	}

	public GameTile GetTileFromPos(Vector3 pos) {
		for (int i = 0; i < gameTiles.Count; i++) {
			if (gameTiles [i].transform.position == pos) {
				return gameTiles [i];
			}
		}
		return null;
	}

	public GameObject GetRandomTilePrefabFromType(GameTile.FoliageType inType) {
		GameObject returnObject = null;
		for (int i = 0; i < baseTilePrefabs.Count; i++) {
			if (baseTilePrefabs [i].type == inType) {
				returnObject = baseTilePrefabs [i].basePrefabs [Random.Range (0, baseTilePrefabs [i].basePrefabs.Count)];
			}
		}
		return returnObject;
	}

	public Vector3 GetRandomPositionOnBoard() {
		Vector3 randomCoords = new Vector3 (Random.Range (-41, 41), 0, Random.Range (-41, 41));
		if (randomCoords.x % 5 != 0) {
			randomCoords.x = RoundToNearestFive (Mathf.RoundToInt(randomCoords.x));
		} if (randomCoords.z % 5 != 0) {
			randomCoords.z = RoundToNearestFive (Mathf.RoundToInt(randomCoords.z));
		}
		return randomCoords;
	}

	public int RoundToNearestFive(int preNum){
		return preNum += (5 - Mathf.RoundToInt (preNum % 5));
	}
	public int RandomRotationAngle() {
		return rotations [Random.Range (0, rotations.Length)];
	}
}
