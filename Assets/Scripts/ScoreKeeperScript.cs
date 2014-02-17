using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerScoreInfo
{
	public PlayerCharacterSettingsScript player;
	public int kills;
	public int assists;
	public int deaths;

	public PlayerScoreInfo(PlayerCharacterSettingsScript pl, int kill, int assist, int death)
	{
		player = pl;
		kills = kill;
		assists = assist;
		deaths = death;
	}
}

public class ScoreKeeperScript : MonoBehaviour 
{
	PlayerDataScript playerData;
	List<PlayerScoreInfo> score;

	HUDScript hud;
	ScoreMenuScript scoreMenu;
	SpawnManagerScript spawnManager;
	GameManagerScript gameManager;

	// Use this for initialization
	void Awake () 
	{
		score = new List<PlayerScoreInfo>();
		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		hud = game.GetComponent<HUDScript>();
		scoreMenu = game.GetComponent<ScoreMenuScript>();
		gameManager = game.GetComponent<GameManagerScript>();
		spawnManager = game.GetComponent<SpawnManagerScript>();

		hud.RegisterScoreKeeper(this);
		scoreMenu.RegisterScoreKeeper(this);
		gameManager.RegisterScoreKeeper(this);
		spawnManager.RegisterScoreKeeper(this);

		ResetScore();
	}

	void OnDestroy()
	{
		hud.ClearScoreKeeper();
		scoreMenu.ClearScoreKeeper();
		gameManager.ClearScoreKeeper();
		spawnManager.ClearScoreKeeper();
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int numPlayers = -1;
		NetworkViewID playerID = NetworkViewID.unassigned;
		int kills = -1, assists = -1, deaths = -1;
		if(stream.isWriting)
		{
			numPlayers = score.Count;
			stream.Serialize(ref numPlayers);

			foreach(PlayerScoreInfo playerScore in score)
			{
				playerID = playerScore.player.owner;
				kills = playerScore.kills;
				assists = playerScore.assists;
				deaths = playerScore.deaths;
				stream.Serialize(ref playerID);
				stream.Serialize(ref kills);
				stream.Serialize(ref assists);
				stream.Serialize(ref deaths);
			}
		}
		else
		{
			stream.Serialize(ref numPlayers);

			for(int x = 0; x < numPlayers; x++)
			{
				stream.Serialize(ref playerID);
				stream.Serialize(ref kills);
				stream.Serialize(ref assists);
				stream.Serialize(ref deaths);
				UpdateScoreForPlayer(playerID, kills, assists, deaths);
			}
		}
	}

	void ResetScore()
	{
		score.Clear();
		foreach(PlayerCharacterSettingsScript player in PlayerDataScript.playerData)
		{
			score.Add(new PlayerScoreInfo(player, 0, 0, 0));
		}
	}

	PlayerScoreInfo GetPlayerScore(NetworkViewID playerID)
	{
		foreach(PlayerScoreInfo playerScore in score)
		{
			if(playerScore.player.owner == playerID)
				return playerScore;
		}
		return null;
	}

	void UpdateScoreForPlayer(NetworkViewID playerID, int kills, int assists, int deaths)
	{
		PlayerScoreInfo playerScore = GetPlayerScore(playerID);

		if(playerScore != null)
		{
			playerScore.kills = kills;
			playerScore.assists = assists;
			playerScore.deaths = deaths;
		}
		else
		{
			score.Add(new PlayerScoreInfo(playerData.GetPlayerData(playerID), kills, assists, deaths));
		}
	}

	public List<PlayerScoreInfo> GetPlayersScores()
	{
		return new List<PlayerScoreInfo>(score);
	}
	
	public Dictionary<Team, int> GetTeamScore()
	{
		Dictionary<Team, int> teamScore = new Dictionary<Team, int>();
		
		foreach(PlayerScoreInfo playerScore in score)
		{
			if(teamScore.ContainsKey(playerScore.player.team))
				teamScore[playerScore.player.team] += playerScore.kills;
			else
				teamScore.Add(playerScore.player.team, playerScore.kills);
		}
		
		return teamScore;
	}

	public void ReportDeath(CharacterBuildScript build)
	{
		if(build.networkView.isMine)		//we only report scores for our own player
		{
			networkView.RPC("AddDeath", RPCMode.Server, build.build.owner);
			//GetPlayerScore(build.build.owner).deaths++;

			Targetable playerHit = build.GetComponent<Targetable>();

			if(!playerHit)
			{
				Debug.LogError("There is no Targetable on this player to report a score for");
				return;
			}

			HitStruct lastHit = new HitStruct(-1f);
			List<HitStruct> hits = playerHit.GetHits(GameSettings.hitAddsToScoreTime);
			foreach(HitStruct hit in hits)
			{
				if(hit.timestamp > lastHit.timestamp)
					lastHit = hit;
			}

			//apply kill/assist points appropriately to all players that had hit the dead player
			foreach(HitStruct hit in hits)
			{
				if(hit.attacker == lastHit.attacker)
					networkView.RPC("AddKill", RPCMode.Server, lastHit.attacker.build.owner);
					//GetPlayerScore(lastHit.attacker.build.owner).kills++;
				else
					networkView.RPC("AddAssist", RPCMode.Server, hit.attacker.build.owner);
					//GetPlayerScore(hit.attacker.build.owner).assists++;				
			}
		}
	}

	[RPC]
	void AddKill(NetworkViewID player)
	{
		GetPlayerScore( player ).kills++;
	}

	[RPC]
	void AddAssist(NetworkViewID player)
	{
		GetPlayerScore( player ).assists++;
	}

	[RPC]
	void AddDeath(NetworkViewID player)
	{
		GetPlayerScore( player ).deaths++;
	}

	/*
	public void UpdateScore(Team t)
	{
		if(networkManager.isServer)
			CharacterDeath((int)t);
		else
			networkView.RPC("CharacterDeath", RPCMode.Server, (int)t);
	}

	[RPC]
	void CharacterDeath(int teamNum)
	{
		Team t = (Team)teamNum;//playerData.GetPlayerData(id).team;
		Dictionary<Team, int> tempScore = new Dictionary<Team, int>(score);
		foreach(KeyValuePair<Team, int> teamScore in score)
		{
			if(teamScore.Key != t)
			{
				tempScore[teamScore.Key] += 1;
			}
		}
		score = tempScore;
	}*/
}
