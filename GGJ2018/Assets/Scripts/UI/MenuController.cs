using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	public static MenuController instance;

	public MenuRoundStart roundStart;
	public MenuMovesCount moveCount;

	void Awake() {
		instance = this;
	}

	public MenuRoundStart GetRoundStart() { return roundStart; }
	public MenuMovesCount GetMovesCount() { return moveCount;  }
}
