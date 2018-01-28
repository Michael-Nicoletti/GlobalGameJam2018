using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : GameUnit {

	[SerializeField]GameObject minionPrefab;
	[SerializeField] int maxMinionCharges = 1;
	int minionCharges = 0;

	protected override void Awake() {
		base.Awake ();
		RefreshMinionCharges ();
	}

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	//Manual Movement - Check appropriate cardinal tile. 
	//Skip if null, impassable, or a transmitter.
	//Then check if the tile contains a weakling, if yes, send message of doom.
	//If no, then add the harmless tile to pathing.
	void TryAttackMove(Vector3 target){
		if (!asleep && !travelling){
			GameTile currentTile = GameTileManager.instance.GetTileFromPos(RoundVectorToFives (transform.position));

			if(movesRemaining > 0){
				if (  currentTile.tiles [(int)GameTile.Direction.Up] != null && target == Vector3.forward
					&&  currentTile.tiles [(int)GameTile.Direction.Up].type != GameTile.TileType.Impassible
					&&  currentTile.tiles [(int)GameTile.Direction.Up].type != GameTile.TileType.Transmitter) {

					GameObject potentialWeakling = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Up]);
					if (potentialWeakling != null) {
						potentialWeakling.SendMessage ("Kill"); //MESSAGE OF PAIN
					} 
					pathing.Add (currentTile.tiles [(int)GameTile.Direction.Up]);

				} else if (  currentTile.tiles [(int)GameTile.Direction.Left] != null && target == Vector3.left
					&&  currentTile.tiles [(int)GameTile.Direction.Left].type != GameTile.TileType.Impassible
					&&  currentTile.tiles [(int)GameTile.Direction.Left].type != GameTile.TileType.Transmitter) {

					GameObject potentialWeakling = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Left]);
					if (potentialWeakling != null) {
						potentialWeakling.SendMessage ("Kill"); //MESSAGE OF PAIN
					} 
					pathing.Add (currentTile.tiles [(int)GameTile.Direction.Left]);

				} else if (  currentTile.tiles [(int)GameTile.Direction.Down] != null  && target == Vector3.back
					&&  currentTile.tiles [(int)GameTile.Direction.Down].type != GameTile.TileType.Impassible
					&&  currentTile.tiles [(int)GameTile.Direction.Down].type != GameTile.TileType.Transmitter) {

					GameObject potentialWeakling = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Down]);
					if (potentialWeakling != null) {
						potentialWeakling.SendMessage ("Kill"); //MESSAGE OF PAIN
					}
					pathing.Add (currentTile.tiles [(int)GameTile.Direction.Down]);
					
				} else if (  currentTile.tiles [(int)GameTile.Direction.Right] != null  && target == Vector3.right
					&&  currentTile.tiles [(int)GameTile.Direction.Right].type != GameTile.TileType.Impassible
					&&  currentTile.tiles [(int)GameTile.Direction.Right].type != GameTile.TileType.Transmitter) {

					GameObject potentialWeakling = tileContainsWEAKLING (currentTile.tiles [(int)GameTile.Direction.Right]);
					if (potentialWeakling != null) {
						potentialWeakling.SendMessage ("Kill"); //MESSAGE OF PAIN
					} 
					pathing.Add (currentTile.tiles [(int)GameTile.Direction.Right]);
				}
			}
		}
	}

	GameObject tileContainsWEAKLING(GameTile gt){
		List<GameObject> MEATS = GameManager.instance.GetPlayersInGame ();
		for (int i = 0; i < MEATS.Count; i++) {
			if (RoundVectorToFives (MEATS [i].transform.position) == gt.transform.position) {
				return MEATS [i];
			}
		}
		return null; //tile contains nature nature not weak
	}

	protected override void TravelAlongTiles(){
		if (movesRemaining == 0) {
			pathing.Clear();
		}

		if (pathing.Count >= 1){
			transform.position = Vector3.Lerp (transform.position, pathing[pathing.Count-1].transform.position + (pathing[pathing.Count-1].transform.position - transform.position).normalized*1.3f, Time.deltaTime * tileTransitionSpeed);
			Debug.DrawRay (transform.position, Vector3.up * 5, Color.yellow, 1.0f);
			GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (transform.position);
			travelling = true;
			ControlHandler.instance.controlLock = true;
			if (GameManager.instance.WhoseTurnIsIt () == gameObject) {
				GameTileManager.instance.UpdateActiveListBasedOnPlayerPosition (transform.position);
			}
			if (Vector3.Distance (transform.position, pathing[pathing.Count-1].transform.position) < tileSnapDistance) {
				transform.position = pathing[pathing.Count-1].transform.position;
				pathing.RemoveAt (pathing.Count-1);
				movesRemaining--;
				travelling = false;
				ControlHandler.instance.controlLock = false;
			}

			if (pathing.Count >= 1) Debug.DrawRay (pathing[pathing.Count-1].transform.position, Vector3.up * 8f, Color.white);
		}
	}

	void SpawnMinion(){
		if (minionCharges > 0) {
			GameObject minion = Instantiate (minionPrefab, RoundVectorToFives (transform.position), Quaternion.identity);
			GameManager.instance.AddMinionToGame (minion);
			minionCharges--;
		}
	}

	public void RefreshMinionCharges(){
		minionCharges = maxMinionCharges;
	}
}
