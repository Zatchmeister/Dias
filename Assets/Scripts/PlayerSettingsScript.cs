using UnityEngine;
using System.Collections;

public class PlayerSettingsScript : MonoBehaviour {


	//Player Settings
	public bool xMovementInversion {get;set;}		//Inversion for sideways movement
	public bool xCameraInversion {get;set;}			//Inversion for sideways camera panning
	public bool yCameraInversion {get;set;}			//Inversion for up and down camera panning
	public float xCameraSensitivity {get;set;}		//Sensitivity at which camera pans sideways
	public float yCameraSensitivity {get;set;}		//Sensitivity at which camera pans up and down
	public float scrollCameraSensitivity {get;set;}	//Sensitivity at which camera zooms in and out (proportion of their input)
	public float cameraFOV {get;set;}				//field of vision for the main camera
	public string playerName {get;set;}				//player's name

	void Awake()
	{
		//set the default values for settings
		SetToDefault();

		//attempt to pull settings from the Unity PlayerPrefs
		RestoreFromPrefs();

		//save any initial changes that may have been made to PlayerPrefs
		SaveSettings();
	}

	public void SetToDefault()
	{
		xMovementInversion = false;
		xCameraInversion = false;
		yCameraInversion = false;
		xCameraSensitivity = 40f;
		yCameraSensitivity = 40f;
		scrollCameraSensitivity = 0.5f;
		cameraFOV = 60f;
	}

	public void SaveSettings()
	{
		SetBool("xMovementInversion", xMovementInversion);
		SetBool("xCameraInversion", xCameraInversion);
		SetBool("yCameraInversion", yCameraInversion);
		PlayerPrefs.SetFloat("xCameraSensitivity", xCameraSensitivity);
		PlayerPrefs.SetFloat("yCameraSensitivity", yCameraSensitivity);
		PlayerPrefs.SetFloat("scrollCameraSensitivity", scrollCameraSensitivity);
		PlayerPrefs.SetFloat("cameraFOV", cameraFOV);
	}

	public void RestoreFromPrefs()
	{
		xMovementInversion = GetBool("xMovementInversion", xMovementInversion);
		xCameraInversion = GetBool("xCameraInversion", xCameraInversion);
		yCameraInversion = GetBool("yCameraInversion", yCameraInversion);
		xCameraSensitivity = Mathf.Clamp( PlayerPrefs.GetFloat("xCameraSensitivity", xCameraSensitivity), GameSettings.minXCamSensitivity, GameSettings.maxXCamSensitivity );
		yCameraSensitivity = Mathf.Clamp( PlayerPrefs.GetFloat("yCameraSensitivity", yCameraSensitivity), GameSettings.minYCamSensitivity, GameSettings.maxYCamSensitivity );
		scrollCameraSensitivity = Mathf.Clamp( PlayerPrefs.GetFloat("scrollCameraSensitivity", scrollCameraSensitivity), GameSettings.minScrollSensitivity, GameSettings.maxScrollSensitivity );
		cameraFOV = Mathf.Clamp( PlayerPrefs.GetFloat("cameraFOV", cameraFOV), GameSettings.minCamFOV, GameSettings.maxCamFOV );
	}

	void SetBool(string playerPrefLoc, bool defaultVal)
	{
		PlayerPrefs.SetInt(playerPrefLoc, defaultVal ? 1 : 0);
	}

	bool GetBool(string playerPrefLoc, bool defaultVal)
	{
		return Mathf.Clamp01( PlayerPrefs.GetInt(playerPrefLoc, defaultVal ? 1 : 0) ) == 1;
	}
}
