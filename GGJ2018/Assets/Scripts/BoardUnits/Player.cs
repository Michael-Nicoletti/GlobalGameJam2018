using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : GameUnit {

	protected override void FixedUpdate(){
		base.FixedUpdate ();
	}

	public void TryMovement(GameObject target){
		pathfindFromTo (transform.position, target.transform.position);
	}
}
