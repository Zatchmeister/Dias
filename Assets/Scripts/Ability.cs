using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ability : MonoBehaviour
{	
	public AbilityData abilityData {get; set;}
	public CharacterBuildScript player {get; private set;}
	public float cooldownRemaining {get; protected set;}

	public abstract bool CanCast();
	public abstract void Reset();					//should return the ability to the uncasted, ready to cast position, as well as restart any cooldown timers, if relevant
	public abstract void Cast(Vector3 castLoc, Quaternion castDir);
	public abstract Quaternion GetCastDirection();
	public abstract Vector3 GetCastPosition();
	public abstract void DrawGUILabel();

	public void AddPlayer(CharacterBuildScript build)
	{
		player = build;
	}

	public void ClearCooldown()
	{
		cooldownRemaining = 0f;
	}

	public virtual void ResetCooldown()
	{
		cooldownRemaining = abilityData.cooldownLength;
	}

	//should be called when the ability is created
	public void StartCooldownCounter()
	{
		StartCoroutine(CooldownCounter());
	}

	public IEnumerator CooldownCounter()
	{
		while(true)
		{
			cooldownRemaining -= Time.deltaTime;
			yield return 0;
		}
	}

	protected void DrawCooldownLabel()
	{
		//grab the current GUI color, so we can modify it
		Color guiColor = GUI.color;
		
		if(cooldownRemaining > 0)
		{
			GUI.color = Color.red;
			GUILayout.Label(Mathf.CeilToInt(cooldownRemaining).ToString());
		}
		else
		{
			GUI.color = Color.green;
			GUILayout.Label("Ready");
		}
		
		//reset the GUI color
		GUI.color = guiColor;	
	}
}