using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : GameUnit {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	protected override void FixedUpdate(){
		base.FixedUpdate ();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
	}

	public void TryMovement(GameObject target){
		pathfindFromTo (transform.position, target.transform.position, false);
	}
}
