
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

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		//Spawn Player
		playersInGame.Add(Instantiate (playerPrefab));
	}

	void Update() {
		GameUnit playerInControl = WhoseTurnIsIt ().GetComponent<GameUnit>();
	//	cameraMoveText.GetComponent<Text>().text = playerInControl.GetMovesRemaining() + " / " + playerInControl.GetMaxMoves();


	}

	void FixedUpdate() {
	}
		
	public GameObject WhoseTurnIsIt() {
		return playersInGame[playerTurn-1];
	}

	public void NextTurn(){
		playerTurn++;
		if (playerTurn > 4)
			playerTurn = 1;
	}
}
