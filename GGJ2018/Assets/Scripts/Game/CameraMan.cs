﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour {

	public static CameraMan instance;
	[SerializeField] private Vector3 cameraOffset = new Vector3 (-4, 10, -7);
	[SerializeField] private float CameraSpeed = 1;
	private Transform target;

	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		
		if (target) SlideSmoothestlyToTarget (target.transform.position);

	}

	public void SlideSmoothestlyToTarget(Vector3 target)
	{
		//Lerp smoothly from the camera position to the target position. Include offset.
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, 0, target.z) + cameraOffset, CameraSpeed * Time.fixedDeltaTime);
		return;
	}

	public void SetTarget (Transform newTar)
	{
		target = newTar;
		return;
	}

	public Vector3 GetCameraOffset(){
		return cameraOffset;
	}
}
