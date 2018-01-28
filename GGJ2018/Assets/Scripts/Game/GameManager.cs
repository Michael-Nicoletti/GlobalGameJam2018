
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
		//Spawn Player One
		playersInGame.Add(Instantiate (playerPrefab));
		WhoseTurnIsIt ().GetComponent<GameUnit> ().WakeUp ();
		CameraMan.instance.SetTarget (WhoseTurnIsIt().transform);

		//Spawn Other Players
		playersInGame.Add(Instantiate (playerPrefab));
		playersInGame.Add(Instantiate (playerPrefab));
		playersInGame.Add(Instantiate (playerPrefab));
	}

	void Update() {
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
		yield return new WaitForSeconds(2);
	}
}
