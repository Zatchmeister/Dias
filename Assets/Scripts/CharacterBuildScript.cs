using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBuildScript : MonoBehaviour {

	public float weight {get; private set;}

	public int jumpCount {get; private set;}
	public int totalJumps {get; set;}

	List<Ability> abilities;
	public PlayerCharacterSettingsScript build;

	CameraControllerScript cam;				//the camera controller
	HUDScript hud;
	ScoreMenuScript scoreMenu;
	PlayerDataScript playerData;
	NetworkManagerScript networkManager;
	CharacterNameDisplayScript nameDisplay;

	void Awake () 
	{
		jumpCount = totalJumps = GameSettings.standardJumpCount;
		abilities = new List<Ability>();

		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		hud = game.GetComponent<HUDScript>();
		scoreMenu = game.GetComponent<ScoreMenuScript>();
		networkManager = game.GetComponent<NetworkManagerScript>();
		playerData = game.GetComponent<PlayerDataScript>();
		nameDisplay = transform.Find("CharacterNameDisplay").GetComponent<CharacterNameDisplayScript>();
	}

	[RPC]
	void ApplyBuild(NetworkViewID p, NetworkMessageInfo info)
	{
		build = playerData.GetPlayerData(p);		//grab our Player's build from PlayerData
		build.character = this;						//mark this CharacterBuildScript in the PlayerData, under the appropriate player

		//for our overhead name display, mark our build
		nameDisplay.RegisterPlayerData(build);

		//Set the player's color based on the team they have selected
		transform.Find("CharacterModel").renderer.material.SetColor("_Color", build.team.color);

		//Always add the two movement abilities first, jump, then dash
		AbilityDatabase.GetMoveAbility(0).AddToPlayer(this);	//jump
		AbilityDatabase.GetMoveAbility(1).AddToPlayer(this);	//dash
		
		//add the role abilities
		AbilityDatabase.GetRoleAbility(0).AddToPlayer(this);	//push
		AbilityDatabase.GetRoleAbility(1).AddToPlayer(this);	//box drop
		AbilityDatabase.GetRoleAbility(2).AddToPlayer(this);	//area push
		AbilityDatabase.GetRoleAbility(4).AddToPlayer(this);	//toggle gravity

		//if this is our player, register the build with the HUD
		if(networkView.isMine)
			hud.RegisterCharacterBuild(this);

		//start the cooldowns for this player
		RestartAbilities();
	}

	void OnDestroy()
	{
		hud.ClearCharacterBuild();
		build.character = null;
	}

	public void RespawnAction(bool start)
	{
		//tell the menus that this player is respawning
		hud.RespawnAction(start);
		scoreMenu.RespawnAction(start);

		//if we are ending the respawn action, reset the abilities to castable
		if(!start)
			RestartAbilities();
	}

	public void RestartAbilities()
	{
		foreach(Ability a in abilities)
			a.Reset();
	}

	public List<Ability> GetAbilities()
	{
		return new List<Ability>(abilities);
	}

	public Ability GetAbility(int abilityNum)
	{
		return abilities[abilityNum];
	}

	public void AddAbility(Ability ability)
	{
		abilities.Add(ability);
	}

	public void ResetJumpCount()
	{
		jumpCount = totalJumps;
	}

	public void DoJump()
	{
		jumpCount--;
	}

	public void DoDash()
	{
		//dash will always be ability 1
		abilities[1].Cast(Vector3.zero, Quaternion.identity);
	}

	public bool CanJump()
	{
		return jumpCount > 0;
	}

	public bool CanDash()
	{
		return abilities[1].CanCast();
	}

	//this should only be used locally by the player that is actually doing the casting of the ability
	public void Cast(bool ability1, bool ability2, bool ability3, bool ability4, bool dashing) 
	{
		TryCast(ability1, 2);
		TryCast(ability2, 3);
		TryCast(ability3, 4);
		TryCast(ability4, 5);

		if(dashing && CanDash())
			DoDash();
	}

	void TryCast(bool trying, int abilityNum)
	{
		if(trying && abilities[abilityNum].CanCast())
			networkView.RPC("CastAbility", RPCMode.All, abilities[abilityNum].GetCastPosition(), abilities[abilityNum].GetCastDirection(), abilityNum);
	}
	
	//this should be called by all clients
	[RPC]
	public void CastAbility(Vector3 castLoc, Quaternion castDir, int abilityNum, NetworkMessageInfo info) 
	{
		abilities[abilityNum].Cast(castLoc, castDir);
	}
}
