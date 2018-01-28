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
	public List<GameTile> spawnPoints = new List<GameTile>();
	public List<GameTile> transmitterPoints = new List<GameTile>();

	private int[] rotations = new int[] { 0, 90, 180, 270 };

	private List<GameTile> activeTiles = new List<GameTile>();
	private bool initialized = false;

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
				Debug.Log ("We didnt find anything "+ checkCoordinates);
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
			} else { 
				foundTile.SetTransmitter(); 
				transmitterPoints.Add (foundTile);
			}
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
			} else {
				foundTile.SetSpawnTile (); 
				spawnPoints.Add (foundTile);
			}
		}
		initialized = true;
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
		return (int)Mathf.Round (preNum / 5) * 5;
	}
	public int RandomRotationAngle() {
		return rotations [Random.Range (0, rotations.Length)];
	}

	public void UpdateActiveListBasedOnPlayerPosition(Vector3 playerPos) {
		List<GameTile> newList = new List<GameTile> ();
		foreach (GameTile tile in activeTiles) {
			tile.inRange = false;
		}
		GameTile checkTile = GetTileFromPos(new Vector3(RoundToNearestFive(Mathf.RoundToInt(playerPos.x)), 0, RoundToNearestFive(Mathf.RoundToInt(playerPos.z))));
		if (checkTile != null) {
			newList.Add (checkTile);
			Debug.DrawRay (checkTile.transform.position, Vector3.up * 10, Color.blue, 10.0f);
			for (int i = 0; i < checkTile.tiles.Length; i++) {
				GameTile addTile = checkTile.tiles [i];
				if (addTile != null) {
					newList.Add (addTile);
					Debug.DrawRay (addTile.transform.position, Vector3.up * 10, Color.blue, 10.0f);
					for (int j = 0; j < addTile.tiles.Length; j++) {
						if (addTile.tiles [j] != null && !newList.Contains (addTile.tiles [j])) {
							Debug.DrawRay (addTile.tiles[j].transform.position, Vector3.up * 10, Color.blue, 10.0f);
							newList.Add(addTile.tiles[j]);
						}
					}
				}
			}
		}

		for (int i = 0; i < activeTiles.Count; i++) {
			Debug.DrawRay (activeTiles [i].transform.position, Vector3.up * 10, Color.black, 10.0f);	
			if(!newList.Contains(activeTiles[i])) {
				activeTiles.Remove(activeTiles[i]);
			}
		}

		for (int i = 0; i < newList.Count; i++) {
			if (!activeTiles.Contains (newList [i])) {
				activeTiles.Add (newList [i]);
			}
		}
		foreach (GameTile tile in activeTiles) {
			tile.inRange = true;
		}
	}

	public bool GetInit() { return initialized; }
}
