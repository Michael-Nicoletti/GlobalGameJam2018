using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour {

	public static CameraMan instance;
	[SerializeField] private Vector3 cameraOffset = new Vector3 (-4, 10, -7);
	[SerializeField] private float CameraSpeed = 3;
	[SerializeField] private float zoomOutDist = 50f;
	private Transform target;
	private float originalOrthoSize;
	private Camera thisCamera;


	public enum CameraModes{Free, Follow, Full}
	public CameraModes cameraMode = CameraModes.Follow;

	void Awake () {
		instance = this;
		thisCamera = this.GetComponent<Camera> ();
		originalOrthoSize = thisCamera.orthographicSize;
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		switch (cameraMode) {
		case CameraModes.Free:
			thisCamera.orthographicSize = originalOrthoSize;
			break;
		case CameraModes.Follow:
			thisCamera.orthographicSize = originalOrthoSize;
			if (target) SlideSmoothestlyToTargetGrounded (target.transform.position);
			break;
		case CameraModes.Full:
			thisCamera.orthographicSize = Mathf.Lerp (thisCamera.orthographicSize, zoomOutDist, Time.deltaTime);
			SlideSmoothestlyToTargetAerial (new Vector3 (5, 0, -5));
			break;
		}
	}

	void FixedUpdate() {
		
	}

	public void SlideSmoothestlyToTargetGrounded(Vector3 target)
	{
		//Lerp smoothly from the camera position to the target position. Include offset.
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, 0, target.z) + cameraOffset, CameraSpeed * Time.fixedDeltaTime);
		return;
	}

	public void SlideSmoothestlyToTargetAerial(Vector3 target)
	{
		//Lerp smoothly from the camera position to the target position. Include offset.
		transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, target.z) + cameraOffset, CameraSpeed * Time.fixedDeltaTime);
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

	public void ChangeCameraModes(CameraModes cm){
		cameraMode = cm;
	}
}
