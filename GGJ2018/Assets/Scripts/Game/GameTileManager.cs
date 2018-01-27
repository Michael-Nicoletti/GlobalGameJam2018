using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileManager : MonoBehaviour {

	public static GameTileManager instance;
	public List<GameTile> gameTiles = new List<GameTile>();

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
}
