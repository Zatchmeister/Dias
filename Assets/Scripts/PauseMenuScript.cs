using UnityEngine;
using System.Collections;

public class PauseMenuScript : MenuScript {

	enum PauseMenuNextMenu {NONE, SETTINGS, GAME, LEAVE_GAME, END_GAME}

	NetworkManagerScript networkManager;
	PlayerInputScript currentPlayer;
	GameManagerScript gameManager;

	PauseMenuNextMenu nextMenu;
	SettingsMenu settings;
	HUDScript hud;
	MainMenuScript mainMenu;
	GameLobbyScript lobby;

	public void RegisterInputScript(PlayerInputScript input)
	{
		currentPlayer = input;
	}

	// Use this for initialization
	void Start () 
	{
		nextMenu = PauseMenuNextMenu.NONE;
		networkManager = GetComponent<NetworkManagerScript>();
		gameManager = GetComponent<GameManagerScript>();
		settings = GetComponent<SettingsMenu>();
		mainMenu = GetComponent<MainMenuScript>();
		lobby = GetComponent<GameLobbyScript>();
		hud = GetComponent<HUDScript>();
	}

	void Update()
	{
		//if(Input.GetButtonDown("Pause"))
		//	sendToGame = true;
	}
	
	public override void GUIMethod()
	{
		currentPlayer.enabled = false;

		if(!networkManager.connected)		//if we lose connection, send back to the main menu
			nextMenu = PauseMenuNextMenu.LEAVE_GAME;

		BeginScreenCentering();

		if(GUILayout.Button("Return to Game"))
			nextMenu = PauseMenuNextMenu.GAME;

		if(GUILayout.Button("Settings"))
			nextMenu = PauseMenuNextMenu.SETTINGS;

		if(networkManager.isServer)
		{
			//the host player can only end the game, they cannot leave
			if(GUILayout.Button("End Game"))
				nextMenu = PauseMenuNextMenu.END_GAME;
		}
		else
		{
			if(GUILayout.Button("Leave Game"))
				nextMenu = PauseMenuNextMenu.LEAVE_GAME;
		}

		EndScreenCentering();
	}
	
	public override MenuScript GetNextMenu()
	{
		if(nextMenu == PauseMenuNextMenu.SETTINGS)
		{
			nextMenu = PauseMenuNextMenu.NONE;
			settings.SetPrevious(this);
			return settings;
		}
		else if(nextMenu == PauseMenuNextMenu.GAME)
		{
			nextMenu = PauseMenuNextMenu.NONE;
			currentPlayer.enabled = true;
			return hud;
		}
		else if(nextMenu == PauseMenuNextMenu.END_GAME)
		{
			nextMenu = PauseMenuNextMenu.NONE;
			gameManager.EndGame();
			return lobby;
		}
		else if(nextMenu == PauseMenuNextMenu.LEAVE_GAME)
		{
			nextMenu = PauseMenuNextMenu.NONE;
			gameManager.EndGame();
			return mainMenu;
		}

		return this;
	}
}
