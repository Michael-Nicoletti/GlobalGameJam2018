using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRoundStart : MonoBehaviour {

	public Animator anim;
	public Text playerInformation;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();

	}

	public void StartNewRound(GameUnit inUnit) {
		if (inUnit is Player) {
			playerInformation.text = "Player " + (GameManager.playerTurn-1) + "'s Turn!";
			anim.SetTrigger ("StartRound");
		} else if (inUnit is Boss) {
			playerInformation.text = "boss Turn!";
			anim.SetTrigger ("StartRound");
		}
	}
}
