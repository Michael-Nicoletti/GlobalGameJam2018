using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : GameUnit {

	// Use this for initialization
	void Start () {
		pathfindFromTo (transform.position, new Vector3 (0, 0, 25));
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate(){
		base.FixedUpdate ();
	}
}
