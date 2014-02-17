using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class TelescopeScript : MonoBehaviour {

	//Storage Variables -- DO NOT EDIT
	CameraControllerScript camController;
	PlayerSettingsScript settings;
	float currZoomDistance;

	// Use this for initialization
	void Awake()
	{
		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		camController = game.GetComponent<CameraControllerScript>();
		settings = game.GetComponent<PlayerSettingsScript>();
	}

	void Update()
	{
		currZoomDistance = Mathf.Clamp(currZoomDistance, GameSettings.telescopeZoomBoundsLower, GameSettings.telescopeZoomBoundsUpper);
		camera.fieldOfView = currZoomDistance;
	}

	public void Disable()
	{
		gameObject.SetActive(false);
		currZoomDistance = GameSettings.telescopeLookBoundsUpper;
	}
	
	public void Enable()
	{
		gameObject.SetActive(true);
	}

	public void Zoom(float zoom)
	{
		currZoomDistance += zoom * settings.scrollCameraSensitivity;
	}

	//should be called once per frame, if telescoping
	public void Look(float lookX, float lookY)
	{
		//update the camera x and y angles in the camera controller, based on our movement for this camera
		camController.lookXAng += lookX * Time.deltaTime * GameSettings.telescopeSensitivity;
		camController.lookYAng = Mathf.Clamp( camController.lookYAng + lookY * Time.deltaTime * GameSettings.telescopeSensitivity, GameSettings.telescopeLookBoundsLower, GameSettings.telescopeLookBoundsUpper);

		Vector3 currRotation = camera.transform.rotation.eulerAngles;
		camera.transform.rotation = Quaternion.Euler(camController.lookYAng, currRotation.y, currRotation.z);
	}
}
