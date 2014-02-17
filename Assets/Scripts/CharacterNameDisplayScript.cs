using UnityEngine;
using System.Collections;

public class CharacterNameDisplayScript : MonoBehaviour {

	Vector3 pos;
	CharacterBuildScript build;

	float bufferFraction = 10f;
	float xCenter;
	float yCenter;
	float buffer;

	void Awake()
	{
		pos = Vector3.zero;
		xCenter = Screen.width/2;
		yCenter = Screen.height/2;
		buffer = Screen.width/bufferFraction;
	}

	void Update()
	{
		pos = GetActiveCamera().WorldToScreenPoint(transform.position);
	}

	void OnGUI()
	{
		//if the nameplate is not ours, is in front of us, and within a bounding box at the center of the screen, draw the name
		if(build && !build.networkView.isMine && pos.z > 0 && pos.x > xCenter-buffer && pos.x < xCenter+buffer && pos.y > yCenter-buffer && pos.y < yCenter+buffer)
		{
			GUILayout.BeginArea( new Rect((pos.x-50), (Screen.height - pos.y - 50), 100, 50) );
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();

			//grab the current GUI color, so we can modify it
			Color guiColor = GUI.color;

			GUI.color = build.build.team.color;
			GUILayout.Label(build.build.playerName);

			//reset the GUI color to its value before this method
			GUI.color = guiColor;

			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
	
	public void RegisterPlayerData(PlayerCharacterSettingsScript characterBuild)
	{
		build = characterBuild.character;
	}

	Camera GetActiveCamera()
	{
		foreach(Camera c in Camera.allCameras)
		{
			if(c.enabled)
				return c;
		}
		return null;
	}
}
