using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : GameUnit {

	public void HuntAndPester(){
		List<GameObject> playerList = GameManager.instance.GetPlayersInGame ();

		GameObject closestPlayer = null;
		float closestDist = Mathf.Infinity;

		foreach (GameObject obj in playerList) {
			if (Vector3.Distance (transform.position, obj.transform.position) < closestDist && obj.GetComponent<Player>() != null) {
				closestDist = Vector3.Distance (transform.position, obj.transform.position);
				closestPlayer = obj;
			}
		}

		if (closestPlayer != null) {
			GameTile currentTile = GameTileManager.instance.GetTileFromPos (RoundVectorToFives (transform.position));

			GameObject potentialWeaklingU = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Up]);
			GameObject potentialWeaklingL = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Left]);
			GameObject potentialWeaklingD = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Down]);
			GameObject potentialWeaklingR = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Right]);
			if (potentialWeaklingU != null) {
				potentialWeaklingU.SendMessage ("Damage"); //MESSAGE OF PAIN
				movesRemaining--;
			} else if (potentialWeaklingL != null) {
				potentialWeaklingL.SendMessage ("Damage"); //MESSAGE OF PAIN
				movesRemaining--;
			} else if (potentialWeaklingD != null) {
				potentialWeaklingD.SendMessage ("Damage"); //MESSAGE OF PAIN
				movesRemaining--;
			} else if (potentialWeaklingR != null) {
				potentialWeaklingR.SendMessage ("Damage"); //MESSAGE OF PAIN
				movesRemaining--;
			} else {
				pathfindFromTo (transform.position, closestPlayer.transform.position);
			}

		} else {
			GameManager.instance.CheckIfAllDead ();
		}
	}
	

	protected override void TravelAlongTiles(){
		base.TravelAlongTiles ();
	}

	protected override void Kill ()
	{
		base.Kill ();
		GameManager.instance.RemoveMinionFromList (this.gameObject);
		Destroy (this.gameObject);
	}

	GameObject tileContainsWEAKLING(GameTile gt){
		if (gt != null) {
			List<GameObject> MEATS = GameManager.instance.GetPlayersInGame ();
			for (int i = 0; i < MEATS.Count; i++) {
				if (MEATS [i].GetComponent<Player> () != null && RoundVectorToFives (MEATS [i].transform.position) == gt.transform.position) {
					return MEATS [i];
				}
			}
		}
		return null; //tile contains nature nature not weak
	}
}
