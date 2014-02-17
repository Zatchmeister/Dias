using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreMenuScript : MenuScript {

	bool isActive = false;
	bool respawning = false;

	bool sendToPause = false;
	PauseMenuScript pause;
	
	bool sendToMainMenu = false;
	MainMenuScript mainMenu;

	bool sendToHUD = false;
	HUDScript hud;
	
	int labelWidth = 44;					//the label width for the kills/assists/deaths columns

	NetworkManagerScript networkManager;
	ScoreKeeperScript scoreKeeper;

	void Start()
	{
		networkManager = GetComponent<NetworkManagerScript>();
		mainMenu = GetComponent<MainMenuScript>();
		hud = GetComponent<HUDScript>();
		pause = GetComponent<PauseMenuScript>();
	}

	void Update()
	{
		if(isActive && !respawning)
		{
			if(Input.GetButtonDown("Pause"))
				sendToPause = true;
			else if(Input.GetButtonUp("Score"))
				sendToHUD = true;
		}
	}

	void HorizontalCenterLabel(string label, int width)
	{
		GUILayout.BeginHorizontal(GUILayout.Width(width));
		GUILayout.FlexibleSpace();
		GUILayout.Label(label);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public override void GUIMethod()
	{
		BeginScreenCentering();
		GUILayout.BeginHorizontal(GUILayout.Height(Screen.height / 2));

		Dictionary<Team, int> teamScore = scoreKeeper.GetTeamScore();
		List<PlayerScoreInfo> score = scoreKeeper.GetPlayersScores();

		foreach(KeyValuePair<Team, int> team in teamScore)
		{
			GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 2 / teamScore.Count));

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(team.Key.name + ": " + team.Value);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Player");
			GUILayout.FlexibleSpace();
			HorizontalCenterLabel("Kills", labelWidth);
			HorizontalCenterLabel("Assists", labelWidth);
			HorizontalCenterLabel("Deaths", labelWidth);
			GUILayout.EndHorizontal();

			for(int x = 0; x < score.Count; x++)
			{
				PlayerScoreInfo playerScore = score[x];
				if(playerScore.player.team == team.Key)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(playerScore.player.playerName);
					GUILayout.FlexibleSpace();
					HorizontalCenterLabel(playerScore.kills.ToString(), labelWidth);
					HorizontalCenterLabel(playerScore.assists.ToString(), labelWidth);
					HorizontalCenterLabel(playerScore.deaths.ToString(), labelWidth);
					GUILayout.EndHorizontal();
				}
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}

		GUILayout.EndHorizontal();
		EndScreenCentering();
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
		else if(sendToHUD)
		{
			sendToHUD = isActive = false;
			return hud;
		}
		isActive = true;
		return this;
	}

	public void RespawnAction(bool start)
	{
		if( !(respawning = start) )		//we just finished respawning, control should be returned to the HUD
		{
			sendToHUD = true;
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
}
