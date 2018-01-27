using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : GameUnit {

	// Use this for initialization
	void Start () {
		StartCoroutine(LateStart(0.1f));

	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate(){

	}

	IEnumerator LateStart(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		pathfindFromTo (transform.position, new Vector3 (0, 0, 15));
	}
}
