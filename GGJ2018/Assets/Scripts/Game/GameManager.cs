
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[SerializeField] private GameObject playerPrefab;
    public GameObject tileObjectTest;

	List<GameObject> playersInGame = new List<GameObject>(); //Reference to all players.
	Vector3 closestTileToMouse;

	// Use this for initialization
	void Start () {
		//Spawn Player
		playersInGame.Add(Instantiate (playerPrefab));

	}

	void Update() {
		MouseDetection ();	
	}

	void FixedUpdate() {
	}

	void MouseDetection()
	{
		//Send out a spherecast from the camera to catch all objects, where it be terrain that needs to be made transparent,
		//or game objects we need to interact with.
		RaycastHit[] mouseHits;
		Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		mouseHits = Physics.RaycastAll (mouseRay.origin, mouseRay.direction);

		//Check all mouseHits, if we hit a gametile, find the closest one to the mouse's on-screen position.
		Vector3 closestGameTile = Vector3.positiveInfinity;
		float closestDistance = Mathf.Infinity;
		//
		for (int i = 0; i < mouseHits.Length; i++ ) {
			if (mouseHits[i].transform.CompareTag ("GameTile") ) {
				
				if (Vector3.Distance (mouseHits[i].transform.position, mouseHits[i].point) < closestDistance) {
					closestGameTile = mouseHits[i].transform.position;

				}
			}
		}

		//We've got a game tile, let's cast a ray from it to show it's highlighted.
		if (closestGameTile != Vector3.positiveInfinity) {
			Debug.DrawRay (closestGameTile, Vector3.up * 5f, Color.yellow);
		}

	}

	public void getHighlightedTile (){

	}
}
