using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDScript : MenuScript {

	bool isActive = false;
	bool respawning = false;

	bool sendToPause = false;
	PauseMenuScript pause;

	bool sendToMainMenu = false;
	MainMenuScript mainMenu;

	bool sendToScore = false;
	ScoreMenuScript scoreMenu;

	Vector2 scrollPos;
	List<string> chat;
	Color chatBoxColor;

	public Texture2D crosshairs;

	NetworkManagerScript networkManager;
	CharacterBuildScript playerBuild;
	ScoreKeeperScript scoreKeeper;
	ChatRoomScript chatRoom;

	Rect chatRect;

	void Start()
	{
		networkManager = GetComponent<NetworkManagerScript>();
		mainMenu = GetComponent<MainMenuScript>();
		pause = GetComponent<PauseMenuScript>();
		scoreMenu = GetComponent<ScoreMenuScript>();
		chatRoom = GetComponent<ChatRoomScript>();
		chatBoxColor = new Color(1,1,1,GameSettings.chatBoxOpacity);

		chatRect = new Rect(0, 100, 300, 400);
	}

	void Update()
	{
		if(isActive)
		{
			if(Input.GetButtonDown("Pause"))
				sendToPause = true;
			if(!respawning && Input.GetButtonDown("Score"))		//we can't switch to score mode if we're respawning
				sendToScore = true;
		}
	}

	void OnGUI()
	{
		if(isActive && Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t')
			Event.current.Use();
	}

	void DrawAbilityGUI(Ability ability)
	{
		GUILayout.BeginVertical ();
		ability.DrawGUILabel();
		GUILayout.Box (ability.abilityData.abilityName);
		GUILayout.EndVertical ();
	}
	
	public override void GUIMethod()
	{
		if(!networkManager.connected)		//if we lose connection, send back to the main menu
			sendToMainMenu = true;

		//The Crosshairs
		GUI.DrawTexture(new Rect(Screen.width/2 - crosshairs.width/2, Screen.height/2 - crosshairs.height/2, crosshairs.width, crosshairs.height), crosshairs);

		if(scoreKeeper)
		{
			//draw the score
			Dictionary<Team, int> score = scoreKeeper.GetTeamScore();
			foreach(KeyValuePair<Team, int> teamScore in score)
			{
				GUILayout.Label(teamScore.Key.name + ": " + teamScore.Value);
			}
		}

		// For "pause"
		GUILayout.BeginArea(new Rect(Screen.width - 75, 0f, 75, 50));
		GUILayout.Label ("ESC/Start for MENU");
		GUILayout.EndArea ();

		//Only show ability stuff if we have a player build registered
		if(playerBuild)
		{
			// For Abilities
			GUILayout.BeginArea (new Rect(200, Screen.height - 75, Screen.width - 400, 75));
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();

			DrawAbilityGUI(playerBuild.GetAbility(2));
			DrawAbilityGUI(playerBuild.GetAbility(3));
			DrawAbilityGUI(playerBuild.GetAbility(4));
			DrawAbilityGUI(playerBuild.GetAbility(5));

			//gap between role abilities and movement abilites
			GUILayout.BeginVertical();
			GUILayout.EndVertical();

			DrawAbilityGUI(playerBuild.GetAbility(0));	//jump ability
			DrawAbilityGUI(playerBuild.GetAbility(1));	//dash ability

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();
		}

		// For ChatBox
		Color myCol = GUI.color;
		GUI.color = chatBoxColor;
		if(Event.current.Equals (Event.KeyboardEvent ("escape"))) // If we're typing and want to pause
			sendToPause = true;
		chatRoom.GUIMethod (chatRect, 300, 250, true);
		GUI.color = myCol;
	
		// For MiniMap / Radar
		GUILayout.BeginArea (new Rect(0, Screen.height - 150, 150, 150));
		GUILayout.Box ("RADAR");
		GUILayout.EndArea ();
	}

	public override MenuScript GetNextMenu()
	{
		if(sendToPause)
		{
			sendToPause = isActive = false;
			return pause;
		}
		else if(sendToMainMenu)
		{
			networkManager.Disconnect();
			sendToMainMenu = isActive = false;
			return mainMenu;
		}
		else if(sendToScore)
		{
			sendToScore = isActive = false;
			return scoreMenu;
		}
		isActive = true;
		return this;
	}
	
	public void RespawnAction(bool start)
	{
		if( (respawning = start) )		//we just started respawning, control should be sent to the ScoreMenu
		{
			sendToScore = true;
		}
	}

	public void RegisterScoreKeeper(ScoreKeeperScript sk)
	{
		scoreKeeper = sk;
	}

	public void ClearScoreKeeper()
	{
		scoreKeeper = null;
	}

	public void RegisterCharacterBuild(CharacterBuildScript build)
	{
		playerBuild = build;
	}

	public void ClearCharacterBuild()
	{
		playerBuild = null;
	}
}
