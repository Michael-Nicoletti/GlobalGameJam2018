using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour {

	public static GameObject highlightedElement;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		MouseDetection ();	
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
			//Checking mouse tiles
			if (mouseHits[i].transform.CompareTag ("GameTile") ) {

				if (Vector3.Distance (mouseHits[i].transform.position, mouseHits[i].point) < closestDistance) {
					closestGameTile = mouseHits[i].transform.position;

				}
			}
			//Check for additional mouse hits here (for ui elements or enemies or whatever)
		}

		//We've got a game tile, let's cast a ray from it to show it's highlighted, and make it the "highlightedElement."
		if (closestGameTile != Vector3.positiveInfinity) {
			Debug.DrawRay (closestGameTile, Vector3.up * 5f, Color.yellow);
			highlightedElement = GameTileManager.instance.GetTileFromPos (closestGameTile).gameObject;
		}

	}
}
