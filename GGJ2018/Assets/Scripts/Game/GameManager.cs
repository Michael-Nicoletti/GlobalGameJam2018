
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject tileObjectTest;
	int playerTurn = 0; //The "Nobody's Turn" state.

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate() {
		if (playerTurn == 1) {
			
		} 
		else if (playerTurn >= 2) 
		{
			
		}
	}
		
	public void nextTurn (){
		//Cycle the number up to 4-
		if (playerTurn < 4)
			playerTurn++;
		else //and back to one.
			playerTurn = 1;
	}
}
