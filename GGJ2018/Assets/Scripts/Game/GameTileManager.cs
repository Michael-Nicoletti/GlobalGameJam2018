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
	public Vector2 minMaxImpassableTiles;


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
			checkCoordinates = new Vector3 (Random.Range (-25, 25), 0, Random.Range (-25, 25));
			if (checkCoordinates.x % 3 != 0) {
				checkCoordinates.x = RoundToNearestThree (Mathf.RoundToInt(checkCoordinates.x));
			} if (checkCoordinates.z % 3 != 0) {
				checkCoordinates.z = RoundToNearestThree (Mathf.RoundToInt(checkCoordinates.z));
			}

			Debug.Log (checkCoordinates);
			GameTile foundTile = GetTileFromPos (checkCoordinates);
			if (foundTile != null) {
				if (!foundTile.CheckSides ()) {
					i -= 1;
					continue;
				} else {
					
				}
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
				returnObject = baseTilePrefabs [i].basePrefabs [Random.Range (0, baseTilePrefabs [i].basePrefabs.Count - 1)];
			}
		}
		return returnObject;
	}

	public int RoundToNearestThree(int preNum){
		return preNum += (3 - Mathf.RoundToInt (preNum % 3));
	}
}
