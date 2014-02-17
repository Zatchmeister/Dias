using UnityEngine;
using System.Collections;

public class AbilityData 
{
	public string abilityName {get; private set;}
	public string scriptName {get; private set;}
	public string abilityDescription {get; private set;}
	public float cooldownLength {get; private set;}			//a cooldownLength of -1 indicates that an ability does not have a cooldown

	public AbilityData(string ability_name, string script_name, string ability_description, float cooldown_length)
	{
		abilityName = ability_name;
		scriptName = script_name;
		abilityDescription = ability_description;
		cooldownLength = cooldown_length;
	}

	public void AddToPlayer(CharacterBuildScript player)
	{
		Ability ability = (Ability)player.gameObject.AddComponent(scriptName);
		ability.abilityData = this;
		player.AddAbility(ability);
		ability.AddPlayer(player);
	}
}
