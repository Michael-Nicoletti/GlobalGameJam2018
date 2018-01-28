
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static int playerTurn = 1;
	 
	[SerializeField] private GameObject cameraMoveText;
	[SerializeField] private GameObject playerPrefab;
    public GameObject tileObjectTest;

	List<GameObject> playersInGame = new List<GameObject>(); //Reference to all players.
	private bool spawnPlayers = true;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
	}

	void Update() {
		if (spawnPlayers && GameTileManager.instance.GetInit ()) {
			spawnPlayers = false;
			GameObject spawnedPlayer = null;
			int randSpawnLoc = 0;
			for (int i = 0; i < 4; i++) {
				spawnedPlayer = Instantiate (playerPrefab);
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
		playerTurn++;
		if (playerTurn > 4)
			playerTurn = 1;
		GameUnit playerInControl = WhoseTurnIsIt ().GetComponent<GameUnit>();
		playerInControl.RefreshMoves();
		CameraMan.instance.SetTarget (WhoseTurnIsIt().transform);
		playerInControl.WakeUp ();
		GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (playerInControl.transform.position);
		yield return new WaitForSeconds(2);
	}
}
