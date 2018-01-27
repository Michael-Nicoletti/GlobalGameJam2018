using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileManager : MonoBehaviour {

	public static GameTileManager instance;

	public GameTile startingTilePrefab;
	public int boardSize = 10;
	public List<GameTile> gameTiles = new List<GameTile>();

	void Awake() {
		instance = this;
	}

	public void Initialize() { 
		GameTile startingTile = Instantiate (startingTilePrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<GameTile>();

		startingTile.Initialize ();

	}
}
