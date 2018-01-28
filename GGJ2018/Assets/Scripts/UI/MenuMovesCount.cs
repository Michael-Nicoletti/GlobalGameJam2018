using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMovesCount : MonoBehaviour {

	public Text movesCount;

	public void UpdateMovesText(GameUnit inUnit) {
		if (inUnit is Player) {
			movesCount.text = "" + inUnit.GetMovesRemaining () + "/" + inUnit.GetMaxMoves () + " Player " + (GameManager.playerTurn - 1);
		} else if (inUnit is Boss) {
			movesCount.text = "" + inUnit.GetMovesRemaining () + "/" + inUnit.GetMaxMoves () + " Boss "	;
		}

		if (inUnit.GetMovesRemaining() <= 0) {
			movesCount.text = "Space to Continue";
		}
	}
}
