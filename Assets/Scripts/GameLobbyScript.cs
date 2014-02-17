using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLobbyScript : MenuScript {

	NetworkManagerScript networkManager;
	GameManagerScript gameManager;
	MenuManager menuManager;
	PlayerDataScript playerData;
	ChatRoomScript chatRoom;

	bool sendToMainMenu = false;
	MainMenuScript mainMenu;

	bool beginGame = false;
	HUDScript hud;

	Vector2 playerScrollPos;
	Vector2 gameScroll;

	//Rects that determine where to draw our GUI stuff
	Rect firstCol;
	Rect secCol;
	Rect thirdCol;

	//String arrays for the ability selection table
	string[] teamList;
	string[] jumpAbilities = new string[] {"Jump Ability 1", "Jump Ability 2", "Jump Ability 3"};
	string[] movementAbilities = new string[] {"Movement Ability 1", "Movement Ability 2", "Movement Ability 3"};
	string[] row1 = new string[] {AbilityDatabase.GetRoleAbility(0).abilityName, AbilityDatabase.GetRoleAbility(1).abilityName, AbilityDatabase.GetRoleAbility(2).abilityName};
	string[] row2 = new string[] {AbilityDatabase.GetRoleAbility(3).abilityName, AbilityDatabase.GetRoleAbility(4).abilityName, AbilityDatabase.GetRoleAbility(5).abilityName};
	string[] row3 = new string[] {AbilityDatabase.GetRoleAbility(6).abilityName, AbilityDatabase.GetRoleAbility(7).abilityName, AbilityDatabase.GetRoleAbility(8).abilityName};
	string[] row4 = new string[] {AbilityDatabase.GetRoleAbility(9).abilityName, AbilityDatabase.GetRoleAbility(10).abilityName, AbilityDatabase.GetRoleAbility(11).abilityName};

	//Game Settings Stuff
	string[] gameModes = new string[] {"Team Deathmatch"};
	int gameModeSelection = 0;

	void Start()
	{
		networkManager = GetComponent<NetworkManagerScript>();
		mainMenu = GetComponent<MainMenuScript>();
		gameManager = GetComponent<GameManagerScript>();
		playerData = GetComponent<PlayerDataScript>();
		menuManager = GetComponent<MenuManager>();
		hud = GetComponent<HUDScript>();
		chatRoom = GetComponent<ChatRoomScript>();

		// column rects			Horizontal start		Vert Start		Width					Height
		firstCol = new Rect(	12,						 50,		Screen.width/3 - 18,	Screen.height - 50);
		secCol = new Rect(		Screen.width/3 + 6,		 50,		Screen.width/3 - 68, 	Screen.height - 50);
		thirdCol = new Rect(	2*Screen.width/3 - 44,	 50,		Screen.width/3 + 32, 	Screen.height - 50);

		//the list of teams, as strings
		List<Team> teams = Team.GetTeamList();
		teamList = new string[teams.Count];
		for(int x = 0; x < teams.Count; x++)
		{
			teamList[x] = teams[x].name;
		}
	}

	public override void GUIMethod()
	{
		if(!networkManager.connected)		//if we lose connection, send back to the main menu
			sendToMainMenu = true;

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Back"))
			sendToMainMenu = true;

		GUILayout.Label("Server IP = " + networkManager.GetServerIP());

		GUILayout.EndHorizontal();

		/*	TODO add the ability to go to settings from the lobby
		 * 			This might screw up when a server starts a game and such, you have been warned
		if(GUI.Button(new Rect(Screen.width-60f, 0f, 60f, 22f), "Settings"))
			Debug.Log("Going to settings");
		*/

		//******************************************First Column of STUFF**************************************************
		//Here will be a preview of the Game Map, and a ChatBox

		GUILayout.BeginArea (firstCol);
		GUILayout.Box("This will be the MAP");

		chatRoom.GUIMethod(firstCol, 350, 350, false);

		GUILayout.EndArea ();


		//******************************************Second Column of STUFF**************************************************
		//Here will be a preview of the Game Settings, the players all in the ChatRoom, and a Ready Button
		GUILayout.BeginArea (secCol);

		//At the top display the game settings
		//If they are the host, make them options that he/she can select
		//Otherwise just display the currently selected options as labels
		GUILayout.BeginVertical ("box");
		gameScroll = GUILayout.BeginScrollView (gameScroll);
		// Display all the game settings
		if(Network.isServer)
		{
			gameModeSelection = GUILayout.SelectionGrid (gameModeSelection, gameModes, 1);
		}
		else
		{
			GUILayout.Label ("Game Mode: " + gameModes[gameModeSelection]);
		}
		GUILayout.EndScrollView ();
		GUILayout.EndVertical ();
		GUILayout.FlexibleSpace();

		//Display the players that are in the lobby
		GUILayout.BeginVertical ("box");
		GUILayout.BeginHorizontal ("box");
		GUILayout.Label ("Players:");
		GUILayout.EndHorizontal ();

		playerScrollPos = GUILayout.BeginScrollView(playerScrollPos, GUILayout.Width(350), GUILayout.Height(225));
		Color guiColor = GUI.color;
		foreach(PlayerCharacterSettingsScript entry in PlayerDataScript.playerData)
		{
			GUI.color = entry.team.color;
			GUILayout.Label(entry.playerName);
		}
		GUI.color = guiColor;

		GUILayout.EndScrollView();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical ();

		if(GUILayout.Button("READY") && Network.isServer)
		{
			networkView.RPC("LobbyStartGame", RPCMode.AllBuffered);
		}

		GUILayout.FlexibleSpace ();
		
		GUILayout.EndArea ();

		//******************************************Third Column of STUFF**************************************************
		//Here will be all the options for creating a character
		GUILayout.BeginArea (thirdCol);

		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label ("Name:");
		PlayerCharacterSettingsScript.currentBuild.playerName = GUILayout.TextField (PlayerCharacterSettingsScript.currentBuild.playerName, GUILayout.Width(200));
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label ("Team:");
		PlayerCharacterSettingsScript.currentBuild.team = Team.GetTeam( GUILayout.SelectionGrid(PlayerCharacterSettingsScript.currentBuild.team.value, teamList, teamList.Length, GUILayout.Height(30)) );
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ("box");
		GUILayout.Label ("Your Weight: " + PlayerCharacterSettingsScript.currentBuild.weight.ToString("#.00") + " " , GUILayout.Width (110));
		PlayerCharacterSettingsScript.currentBuild.weight = GUILayout.HorizontalSlider (PlayerCharacterSettingsScript.currentBuild.weight, 0, 10f);
		GUILayout.EndHorizontal();

		GUILayout.Space (15);

		GUILayout.BeginVertical ("box");
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label ("Choose Jump Ability!");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		PlayerCharacterSettingsScript.currentBuild.jumpAbility = GUILayout.SelectionGrid (PlayerCharacterSettingsScript.currentBuild.jumpAbility, jumpAbilities, 3, GUILayout.Height (30));
		GUILayout.EndVertical ();

		GUILayout.Space (10);
		
		GUILayout.BeginVertical ("box");
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label ("Choose Movement Ability!");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		PlayerCharacterSettingsScript.currentBuild.movementAbility = GUILayout.SelectionGrid (PlayerCharacterSettingsScript.currentBuild.movementAbility, movementAbilities, 3, GUILayout.Height (30));
		GUILayout.EndVertical ();

		GUILayout.Space (10);

		//*************************************************************************TABLE
		GUILayout.BeginVertical ("box");
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label ("Choose Class Ability!");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label ("Light");
		GUILayout.FlexibleSpace();
		GUILayout.Label ("Support");
		GUILayout.FlexibleSpace();
		GUILayout.Label ("Heavy");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal ();

		PlayerCharacterSettingsScript.currentBuild.firstAbility = GUILayout.SelectionGrid(PlayerCharacterSettingsScript.currentBuild.firstAbility, row1, 3, GUILayout.Height (45));
		PlayerCharacterSettingsScript.currentBuild.secondAbility = GUILayout.SelectionGrid(PlayerCharacterSettingsScript.currentBuild.secondAbility, row2, 3, GUILayout.Height (45)) ;
		PlayerCharacterSettingsScript.currentBuild.thirdAbility = GUILayout.SelectionGrid(PlayerCharacterSettingsScript.currentBuild.thirdAbility, row3, 3, GUILayout.Height (45)) ;
		PlayerCharacterSettingsScript.currentBuild.fourthAbility = GUILayout.SelectionGrid(PlayerCharacterSettingsScript.currentBuild.fourthAbility, row4, 3, GUILayout.Height (45)) ;

		GUILayout.EndVertical ();
		//**********************************************************************END*TABLE
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button ("Save Character", GUILayout.Width(100)))
		{
			PlayerCharacterSettingsScript.currentBuild.SaveSettings();
			playerData.SendMyUpdatedPlayerData();
		}
		GUILayout.EndHorizontal ();
		GUILayout.FlexibleSpace ();

		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}


	[RPC]
	void LobbyStartGame()
	{
		beginGame = true;
		playerData.SendMyBufferedUpdatedPlayerData();
		if(networkManager.isServer)
			gameManager.BeginGameAsHost();
	}

	public override MenuScript GetNextMenu()
	{
		if(sendToMainMenu)
		{
			networkManager.Disconnect();
			sendToMainMenu = false;
			return mainMenu;
		}
		else if(beginGame)
		{
			menuManager.EnableMenuBackgroundCamera(false);
			beginGame = false;
			return hud;
		}
		return this;
	}
}
