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
	public GameObject impassiblePrefab;
	public Vector2 minMaxImpassableTiles;

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
		for (int i = 0; i <= numImpassableTiles; i++) {
			checkCoordinates = new Vector3 (Random.Range (-36, 36), 0, Random.Range (-36, 36));
			if (checkCoordinates.x % 5 != 0) {
				checkCoordinates.x = RoundToNearestFive (Mathf.RoundToInt(checkCoordinates.x));
			} if (checkCoordinates.z % 5 != 0) {
				checkCoordinates.z = RoundToNearestFive (Mathf.RoundToInt(checkCoordinates.z));
			}

			GameTile foundTile = GetTileFromPos (checkCoordinates);
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

	public int RoundToNearestFive(int preNum){
		return preNum += (5 - Mathf.RoundToInt (preNum % 5));
	}
	public int RandomRotationAngle() {
		return rotations [Random.Range (0, rotations.Length)];
	}
}
