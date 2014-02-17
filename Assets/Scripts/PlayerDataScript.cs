using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDataScript : MonoBehaviour {

	public static List<PlayerCharacterSettingsScript> playerData = new List<PlayerCharacterSettingsScript>();

	NetworkManagerScript networkManager;

	void Start()
	{
		networkManager = GetComponent<NetworkManagerScript>();
	}

	public PlayerCharacterSettingsScript GetPlayerData(NetworkViewID p)
	{
		foreach(PlayerCharacterSettingsScript player in playerData)
		{
			if(player.owner == p)
				return player;
		}
		return null;
	}
	
	NetworkViewID GetPlayerData(string n)
	{
		foreach(PlayerCharacterSettingsScript data in playerData)
		{
			if(data.playerName == n)
				return data.owner;
		}
		return NetworkViewID.unassigned;
	}

	void SendMyData(RPCMode rpcMode)
	{
		NetworkViewID id = GetPlayerData(PlayerCharacterSettingsScript.currentBuild.playerName);
		if(id != NetworkViewID.unassigned && id != networkManager.myID)
		{
			int num = 1;
			id = GetPlayerData(PlayerCharacterSettingsScript.currentBuild.playerName + num);
			while(id != NetworkViewID.unassigned && id != networkManager.myID)
			{
				num++;
				id = GetPlayerData(PlayerCharacterSettingsScript.currentBuild.playerName + num);
			}
			
			//update the name with the new number appended
			PlayerCharacterSettingsScript.currentBuild.playerName += num;
			PlayerCharacterSettingsScript.currentBuild.SaveSettings();
			PlayerCharacterSettingsScript.currentBuild.RestoreFromPrefs();
		}
		networkView.RPC ("UpdatePlayerData", rpcMode, networkManager.myID, PlayerCharacterSettingsScript.currentBuild.playerName, PlayerCharacterSettingsScript.currentBuild.weight, PlayerCharacterSettingsScript.currentBuild.jumpAbility, PlayerCharacterSettingsScript.currentBuild.movementAbility, PlayerCharacterSettingsScript.currentBuild.firstAbility, PlayerCharacterSettingsScript.currentBuild.secondAbility, PlayerCharacterSettingsScript.currentBuild.thirdAbility, PlayerCharacterSettingsScript.currentBuild.fourthAbility, PlayerCharacterSettingsScript.currentBuild.team.value);
	}

	//we need to use a buffered send when the game starts, so that any players that join the game after the game is going also receive the same data that we have about our player
	public void SendMyBufferedUpdatedPlayerData()
	{
		SendMyData(RPCMode.AllBuffered);
	}
	
	[RPC]
	public void SendMyUpdatedPlayerData()
	{
		SendMyData(RPCMode.All);
	}

	public void LoadAllPlayerData()
	{
		networkView.RPC("SendMyUpdatedPlayerData", RPCMode.All);
	}

	[RPC]
	void UpdatePlayerData(NetworkViewID player, string playerName, float weight, int jump, int move, int ab1, int ab2, int ab3, int ab4, int team, NetworkMessageInfo info)
	{
		playerData.Remove( GetPlayerData(player) );

		playerData.Add (new PlayerCharacterSettingsScript(playerName, weight, jump, move, ab1, ab2, ab3, ab4, Team.GetTeam(team), player));
	}

	[RPC]
	public void RemovePlayerByID(NetworkViewID player)
	{
		if( !playerData.Remove( GetPlayerData(player) ) )
			Debug.LogError("Player we didn't know about left the game!?");
	}

	public void RemovePlayer(NetworkPlayer player)
	{
		foreach(PlayerCharacterSettingsScript data in playerData)
		{
			if(player == data.owner.owner)
			{
				networkView.RPC("RemovePlayerByID", RPCMode.All, data.owner);
				return;
			}
		}
	}

	public void RemoveAllPlayers()
	{
		playerData.Clear();
	}
}
