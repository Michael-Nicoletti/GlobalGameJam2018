using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

	int speed;
	Vector3 direction;

	void Start(){
		StartCoroutine ("SelfDestruct");
	}

	void FixedUpdate(){
		transform.position = Vector3.MoveTowards (transform.position, direction + transform.position, speed * Time.fixedDeltaTime);

		if (GameTileManager.instance.GetTileFromPos(RoundVectorToFives (transform.position)) != null){
			GameTile currentTile = GameTileManager.instance.GetTileFromPos(RoundVectorToFives (transform.position));
			if (currentTile.type == GameTile.TileType.Impassible || currentTile.type == GameTile.TileType.Transmitter) {
				Destroy (this.gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider other){
		other.SendMessageUpwards ("Kill");
		Destroy (this.gameObject);
	}

	public void Prime (int spd, Vector3 dir){
		speed = spd;
		direction = dir;
	}

	IEnumerator SelfDestruct(){
		yield return new WaitForSeconds (10);
		Destroy (this.gameObject);
	}

	Vector3 RoundVectorToFives(Vector3 n) {
		return new Vector3 ( Mathf.Round (n.x) / 5 * 5, Mathf.Round (n.y) / 5 * 5, Mathf.Round (n.z) / 5 * 5);
	}
}
