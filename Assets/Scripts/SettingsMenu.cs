using UnityEngine;
using System.Collections;

public enum SettingDisplayType {NORMAL, ROUND_TO_INT, TIMES_100}

public class SettingsMenu : MenuScript {

	PlayerSettingsScript settings;

	bool returnToPrevious = false;
	MenuScript previous;

	void Start()
	{
		settings = GetComponent<PlayerSettingsScript>();
	}

	public override void GUIMethod()
	{
		float areaWidth = 600f, nameWidth = 100f;

		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Back"))
			returnToPrevious = true;
		if(GUILayout.Button("Save"))
			settings.SaveSettings();
		if(GUILayout.Button("Restore Defaults"))
			settings.SetToDefault();
		GUILayout.EndHorizontal();

		settings.xCameraInversion = DisplayToggle(settings.xCameraInversion, "Mouse X Inversion", nameWidth, areaWidth);

		settings.xCameraSensitivity = DisplaySlider(settings.xCameraSensitivity, "Mouse X Sensitivity", nameWidth, areaWidth, GameSettings.minXCamSensitivity, GameSettings.maxXCamSensitivity, SettingDisplayType.ROUND_TO_INT);

		settings.yCameraInversion = DisplayToggle(settings.yCameraInversion, "Mouse Y Inversion", nameWidth, areaWidth);

		settings.yCameraSensitivity = DisplaySlider(settings.yCameraSensitivity, "Mouse Y Sensitivity", nameWidth, areaWidth, GameSettings.minYCamSensitivity, GameSettings.maxYCamSensitivity, SettingDisplayType.ROUND_TO_INT);

		settings.xMovementInversion = DisplayToggle(settings.xMovementInversion, "Horizontal Movement Inversion", nameWidth, areaWidth);

		settings.scrollCameraSensitivity = DisplaySlider(settings.scrollCameraSensitivity, "Mouse Scroll Sensitivity", nameWidth, areaWidth, GameSettings.minScrollSensitivity, GameSettings.maxScrollSensitivity, SettingDisplayType.TIMES_100);

		settings.cameraFOV = DisplaySlider(settings.cameraFOV, "FOV", nameWidth, areaWidth, GameSettings.minCamFOV, GameSettings.maxCamFOV, SettingDisplayType.ROUND_TO_INT);

		GUILayout.EndVertical();
		EndScreenCentering();
	}

	bool DisplayToggle(bool value, string name, float nameWidth, float areaWidth)
	{
		GUILayout.BeginHorizontal(GUILayout.Width(areaWidth));
		GUILayout.Label(name, GUILayout.Width(nameWidth));
		bool toReturn = GUILayout.Toggle(value, "");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		return toReturn;
	}

	float DisplaySlider(float value, string name, float nameWidth, float areaWidth, float min, float max, SettingDisplayType displayType)
	{
		GUILayout.BeginHorizontal(GUILayout.Width(areaWidth));
		GUILayout.Label(name, GUILayout.Width(nameWidth));
		float toReturn = GUILayout.HorizontalSlider(value, min, max);
		if(displayType == SettingDisplayType.ROUND_TO_INT)
			GUILayout.Label(Mathf.Round(toReturn).ToString(),GUILayout.Width(30));
		else if(displayType == SettingDisplayType.NORMAL)
			GUILayout.Label(toReturn.ToString("#.00"), GUILayout.Width(30));
		else if(displayType == SettingDisplayType.TIMES_100)
			GUILayout.Label(Mathf.Round(toReturn * 100).ToString(), GUILayout.Width(30));
		GUILayout.EndHorizontal();
		return toReturn;
	}
	
	public override MenuScript GetNextMenu()
	{
		if(returnToPrevious)
		{
			settings.RestoreFromPrefs();	//always pull from the settings file when exiting this menu, in case they made changes and didn't want to save them
			returnToPrevious = false;
			return previous;
		}

		return this;
	}

	public void SetPrevious(MenuScript prev)
	{
		previous = prev;
	}
}
