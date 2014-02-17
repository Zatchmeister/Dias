using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*	Manages the creation and ending of games, as well as when a player were to leave a game before it's ending
 */

public class GameManagerScript : MonoBehaviour 
{
	NetworkManagerScript networkManager;
	MenuManager menuManager;
	SpawnManagerScript spawnManager;
	PlatformManagerScript platformManager;
	GameLobbyScript lobby;
	MainMenuScript mainMenu;
	PlayerDataScript playerData;

	ScoreKeeperScript scoreKeeper;

	public bool gameIsActive {get; private set;}
	
	// Use this for initialization
	void Start ()
	{
		platformManager = GetComponent<PlatformManagerScript>();
		networkManager = GetComponent<NetworkManagerScript>();
		spawnManager = GetComponent<SpawnManagerScript>();
		menuManager = GetComponent<MenuManager>();
		lobby = GetComponent<GameLobbyScript>();
		mainMenu = GetComponent<MainMenuScript>();
		playerData = GetComponent<PlayerDataScript>();
		gameIsActive = false;
	}

	void Update()
	{
		if(gameIsActive)
		{
			if(!networkManager.connected)		//if a game is going, and we have lost connection, delete everything, and kick the user back to the main menu
			{
				CleanUpGame(mainMenu);
			}
			else if(networkManager.isServer && scoreKeeper)	//if the game is going, we are the server, and we still have a scorekeeper object, check for gameover
			{
				CheckGameover();
			}
		}
	}

	public void CheckGameover()
	{
		Dictionary<Team, int> score = scoreKeeper.GetTeamScore();
		foreach(KeyValuePair<Team, int> teamScore in score)
		{
			if(teamScore.Value >= 50)
			{
				EndGame();
			}
		}
	}

	//THIS SHOULD ONLY BE CALLED BY THE HOST PLAYER
	public void BeginGameAsHost()
	{
		if(networkManager.isServer)
		{
			networkView.RPC("GameStarting", RPCMode.AllBuffered);
			scoreKeeper = ((GameObject)Network.Instantiate(Resources.Load("ScoreKeeperObject"), Vector3.zero, Quaternion.identity, (int)NetworkGroup.GAME)).GetComponent<ScoreKeeperScript>();

			platformManager.GenerateArena();
			networkView.RPC("CreatePlayer", RPCMode.AllBuffered);
		}
		else
			Debug.LogError("A nonhost player has attempted to begin the game");
	}
	
	public void EndGame()
	{
		if(networkManager.isServer)		//the host has ended the game
		{
			gameIsActive = false;		//this needs to be reported asap for the scorekeeper, so that it knows to not respawn the last player that is killed in a game
			networkView.RPC ("CleanUpGameAndReturnToLobby", RPCMode.All);		//tells all clients to quit the game
		}
		else 							//the player has left the game before it was over
		{
			networkManager.Disconnect();										//disconnect from the server
			CleanUpGame(mainMenu);
		}
	}

	[RPC]
	public void CreatePlayer()
	{
		GameObject obj = spawnManager.SpawnPlayer();
		obj.networkView.RPC("ApplyBuild", RPCMode.AllBuffered, networkManager.myID);
	}

	void CleanUpGame(MenuScript nextMenu)
	{
		gameIsActive = false;

		if(networkManager.isServer)			//if we are the server, we need to tell all clients to delete some stuff, and remove RPCs
		{
			networkManager.ClearAllRPCs();
			networkManager.NetworkDestroyObjectsWithTag("Player");
			networkManager.NetworkDestroyObjectsWithTag("ArenaObject");
			Network.Destroy(scoreKeeper.gameObject);
		}
		else if(!networkManager.connected)	//if we are no longer connected to anyone, we need to delete some stuff on our own
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject p in players)
				GameObject.Destroy(p);

			GameObject[] objects = GameObject.FindGameObjectsWithTag("ArenaObject");
			foreach(GameObject o in objects)
				GameObject.Destroy(o);

			GameObject.Destroy(scoreKeeper.gameObject);
		}

		platformManager.ClearArena();
		spawnManager.ClearSpawnPoints();
		menuManager.SetCurrent(nextMenu);
		menuManager.EnableMenuBackgroundCamera(true);
	}

	[RPC]
	public void CleanUpGameAndReturnToLobby()
	{
		CleanUpGame(lobby);
	}

	[RPC]
	public void GameStarting()
	{
		gameIsActive = true;
	}

	//if we are not the server, we will need our scorekeeper registered with us
	public void RegisterScoreKeeper(ScoreKeeperScript sk)
	{
		scoreKeeper = sk;
	}
	
	public void ClearScoreKeeper()
	{
		scoreKeeper = null;
	}
}
