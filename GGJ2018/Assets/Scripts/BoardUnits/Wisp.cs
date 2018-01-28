using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : GameUnit {

	GameObject owner;
	GameObject target;
	bool targetAchieved = false;

	public void Prime(GameObject o, GameObject gt){
		owner = o;
		target = gt;
	}

	public void ScoutOrReturn(){
		if (targetAchieved) Return ();
		else Scout ();
	}

	void Scout(){
		pathfindFromTo (transform.position, target.transform.position);
	}

	void Return(){
		pathfindFromTo (transform.position, owner.transform.position);
	}

	protected override void TravelAlongTiles(){
		if (movesRemaining == 0) {
			pathing.Clear();
		}

		if (pathing.Count >= 1){
			transform.position = Vector3.Lerp (transform.position, pathing[pathing.Count-1].transform.position + (pathing[pathing.Count-1].transform.position - transform.position).normalized*1.3f, Time.deltaTime * tileTransitionSpeed);
			Debug.DrawRay (transform.position, Vector3.up * 5, Color.yellow, 1.0f);
			GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (this);
			travelling = true;
			if (GameManager.instance.WhoseTurnIsIt () == gameObject) {
				GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (this);
			}
			if (Vector3.Distance (transform.position, pathing[pathing.Count-1].transform.position) < tileSnapDistance) {
				transform.position = pathing[pathing.Count-1].transform.position;
				pathing.RemoveAt (pathing.Count-1);
				movesRemaining--;
				travelling = false;

				if (transform.position == target.transform.position) Return ();
			}

			if (pathing.Count >= 1) Debug.DrawRay (pathing[pathing.Count-1].transform.position, Vector3.up * 8f, Color.white);
		}
	}
}
