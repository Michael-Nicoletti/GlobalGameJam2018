
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[SerializeField] private GameObject playerPrefab;
    public GameObject tileObjectTest;

	List<GameObject> playersInGame = new List<GameObject>(); //Reference to all players.

	// Use this for initialization
	void Start () {
		//Spawn Player
		playersInGame.Add(Instantiate (playerPrefab));
	}

	void Update() {
		
	}

	void FixedUpdate() {
	}
		
}
