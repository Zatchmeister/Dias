using UnityEngine;
using System;
using System.Collections;

public class PlayerCharacterSettingsScript 
{
	public static PlayerCharacterSettingsScript currentBuild;

	static PlayerCharacterSettingsScript()
	{
		currentBuild = new PlayerCharacterSettingsScript();
	}

	bool allowUpdates = false;							//can this PlayerCharacterSettingsScript save changes to the user's preferences

	//Player Character Settings
	public float weight {get;set;}					//player's weight (float between 0 and 10)
	public int jumpAbility {get;set;}				//player's jump ability (0, 1, or 2)
	public int movementAbility {get;set;}			//player's movement ability (0, 1, or 2)
	public int firstAbility {get;set;}				//player's first ability (0, 1, or 2)
	public int secondAbility {get;set;}				//player's second ability (0, 1, or 2)
	public int thirdAbility {get;set;}				//player's third ability (0, 1, or 2)
	public int fourthAbility {get;set;}				//player's fourth ability (0, 1, or 2)
	public string playerName {get;set;}				//player's name
	public Team team {get;set;}						//player's team
	public CharacterBuildScript character {get;set;}//the actual player within the game
	public NetworkViewID owner {get;set;}			//the player that owns this settings script
	
	private PlayerCharacterSettingsScript()
	{
		//allow updates from this PlayerCharacterSettingsScript
		allowUpdates = true;

		//set the default values for settings
		SetToDefault();
		
		//attempt to pull settings from the Unity PlayerPrefs
		RestoreFromPrefs();
		
		//save any initial changes that may have been made to PlayerPrefs
		SaveSettings();

		//we don't need to save the team in PlayerPrefs, so we simply set it to the first team in the list of potentials on load
		team = Team.GetTeam(0);
	}

	public PlayerCharacterSettingsScript(string pname, float _weight, int jump, int move, int ab1, int ab2, int ab3, int ab4, Team t, NetworkViewID id)
	{
		//do NOT allow updates from this PlayerCharacterSettingsScript
		allowUpdates = false;

		playerName = pname;
		weight = _weight;
		movementAbility = move;
		firstAbility = ab1;
		secondAbility = ab2;
		thirdAbility = ab3;
		fourthAbility = ab4;
		team = t;
		owner = id;
	}
	
	public void SetToDefault()
	{
		if(allowUpdates)
		{
			weight = 5;
			jumpAbility = 0;
			movementAbility = 0;
			firstAbility = 0;
			secondAbility = 0;
			thirdAbility = 0;
			fourthAbility = 0;
			playerName = GameSettings.defaultPlayerName;
		}
		else
			Debug.LogError("Trying to SetToDefault from invalid PlayerCharacterSettingsScript");
	}
	
	public void SaveSettings()
	{
		if(allowUpdates)
		{
			PlayerPrefs.SetFloat ("weight", weight);
			PlayerPrefs.SetInt ("jumpAbility", jumpAbility);
			PlayerPrefs.SetInt ("movementAbility", movementAbility);
			PlayerPrefs.SetInt ("firstAbility", firstAbility);
			PlayerPrefs.SetInt ("secondAbility", secondAbility);
			PlayerPrefs.SetInt ("thirdAbility", thirdAbility);
			PlayerPrefs.SetInt ("fourthAbility", fourthAbility);
			PlayerPrefs.SetString ("playerName", playerName);
		}
		else
			Debug.LogError("Trying to SaveSettings from invalid PlayerCharacterSettingsScript");
	}
	
	public void RestoreFromPrefs()
	{
		if(allowUpdates)
		{
			weight = PlayerPrefs.GetFloat("weight");
			jumpAbility = PlayerPrefs.GetInt("jumpAbility");
			movementAbility = PlayerPrefs.GetInt("movementAbility");
			firstAbility = PlayerPrefs.GetInt("firstAbility");
			secondAbility = PlayerPrefs.GetInt("secondAbility");
			thirdAbility = PlayerPrefs.GetInt("thirdAbility");
			fourthAbility = PlayerPrefs.GetInt("fourthAbility");
			playerName = PlayerPrefs.GetString ("playerName");
		}
		else
			Debug.LogError("Trying to RestoreFromPrefs from invalid PlayerCharacterSettingsScript");
	}
}
