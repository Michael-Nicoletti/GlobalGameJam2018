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

	void Awake() {
		instance = this;
	}

	void Start() {
		GameTile[] gameTileHolder = GetComponentsInChildren<GameTile> ();
		for (int i = 0; i < gameTileHolder.Length; i++) {
			gameTiles.Add (gameTileHolder [i]);
			gameTileHolder [i].Initialize ();
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
}
