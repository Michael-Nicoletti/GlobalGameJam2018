﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct TransmitterNode{
	public bool active;
	public GameTile transmitterTile;
}

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static int playerTurn = 1;
	 
	[SerializeField] private GameObject cameraMoveText;
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private GameObject bossPrefab;
    public GameObject tileObjectTest;
	TransmitterNode[] transmitters = new TransmitterNode[3];

	List<GameObject> playersInGame = new List<GameObject>(); //Reference to all players.
	private bool spawnPlayers = true;

	List<GameObject> minionsInGame = new List<GameObject>();
	ControlHandler controlHandler;

	void Awake() {
		instance = this;
		controlHandler = this.GetComponent<ControlHandler> ();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine ("LateStart");
	}

	IEnumerator LateStart() {
		yield return new WaitForSeconds (0.5f);
		for (int i = 0; i < 3; i++){
			transmitters [i].transmitterTile = GameTileManager.instance.transmitterPoints [i];
			transmitters [i].active = false;
		}
	}

	void Update() {
		if (spawnPlayers && GameTileManager.instance.GetInit ()) {
			spawnPlayers = false;
			GameObject spawnedPlayer = null;
			int randSpawnLoc = 0;
			for (int i = 0; i < 4; i++) {
				if (i == 0) spawnedPlayer = Instantiate (bossPrefab);
				else spawnedPlayer = Instantiate (playerPrefab);

				randSpawnLoc = Random.Range (0, GameTileManager.instance.spawnPoints.Count);
				spawnedPlayer.transform.position = GameTileManager.instance.spawnPoints [randSpawnLoc].transform.position;
				GameTileManager.instance.spawnPoints.RemoveAt (randSpawnLoc);
				playersInGame.Add (spawnedPlayer);
			}
			WhoseTurnIsIt ().GetComponent<GameUnit> ().WakeUp ();
			CameraMan.instance.SetTarget (WhoseTurnIsIt().transform);
			GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (WhoseTurnIsIt ().transform.position);
		}


		GameUnit playerInControl = WhoseTurnIsIt ().GetComponent<GameUnit>();
		cameraMoveText.GetComponent<Text>().text = playerInControl.GetMovesRemaining() + " / " + playerInControl.GetMaxMoves();
	}

	void FixedUpdate() {
		
	}
		
	public GameObject WhoseTurnIsIt() {
		return playersInGame[playerTurn-1];
	}

	public IEnumerator NextTurn(){
		Debug.Log ("Next turn!");
		WhoseTurnIsIt ().GetComponent<GameUnit>().Sleep();
		controlHandler.controlLock = true;

		//If the boss just ended their turn, enter a special minion phase that lasts three seconds (divided amongst all minions).
		if (playerTurn == 1) {
			Debug.Log ("Special Minion Turn! Minion Count: " + minionsInGame.Count);
			for (int i = 0; i < minionsInGame.Count; i++) {
				Minion minion = minionsInGame [i].GetComponent<Minion>();
				minion.WakeUp ();
				minion.RefreshMoves ();
				minion.HuntAndPester ();
				yield return new WaitForSeconds (3 / minionsInGame.Count);
			}

			for (int i = 0; i < minionsInGame.Count; i++) {
				Minion minion = minionsInGame [i].GetComponent<Minion>();
				minion.Sleep ();
			}
		}

		playerTurn++;

		if (playerTurn > 4)
			playerTurn = 1;
		
		while (!WhoseTurnIsIt().GetComponent<GameUnit> ().isAlive ()) {
			playerTurn++;
		}

		if (playerTurn > 4)
			playerTurn = 1;

		GameUnit playerInControl = WhoseTurnIsIt ().GetComponent<GameUnit>();
		playerInControl.RefreshMoves();
		CameraMan.instance.SetTarget (WhoseTurnIsIt().transform);
		playerInControl.WakeUp ();
		GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (playerInControl.transform.position);
		yield return new WaitForSeconds(0.5f);

		controlHandler.controlLock = false;
	}

	public void ActivateTransmitter(GameTile t){

		int w = 0;
		for (int i = 0; i < 3; i++) {
			if (transmitters[i].transmitterTile == t)
				transmitters[i].active = true;
			
			if (transmitters [i].active) {
				w++;
				Debug.DrawRay (transmitters [i].transmitterTile.transform.position, Vector3.up * 100f, Color.white, 5f);
			}
		}

		Debug.Log ("Transmitter activated! " + w + "/3");

		//WIN
		if (w >= 3) {
			Debug.Log ("GAME OVER. PLAYERS WIN.");
			foreach (GameObject p in playersInGame) {
				p.GetComponent<Player>().Sleep ();
			}
		}
		return;
	}

	public void CheckIfAllDead(){
		int deathCount = 0;
		for (int i = 0; i < playersInGame.Count; i++) {
			if (!playersInGame [i].GetComponent<GameUnit> ().isAlive()) deathCount++;
		}

		if (deathCount >= 3) {
			Debug.Log ("GAME OVER. BOSS WIN.");
			foreach (GameObject p in playersInGame) {
				p.GetComponent<GameUnit>().Sleep ();
			}
		}
	}

	public List<GameObject> GetPlayersInGame(){
		return playersInGame;
	}

	public void AddMinionToGame(GameObject m){
		minionsInGame.Add (m);
	}

	public int MinionCount(){
		return minionsInGame.Count;
	}

	public void RemoveMinionFromList(GameObject m){
		for (int i = 0; i < MinionCount () - 1; i++) {
			if (minionsInGame [i] == m) {
				minionsInGame.RemoveAt (i);
			}
		}
	}
}
