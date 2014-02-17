using UnityEngine;
using System.Collections;

public class AbilityDash : Ability 
{
	CharacterMoveScript movement;
	float duration = 0.2f;

	void Start()
	{
		movement = GetComponent<CharacterMoveScript>();
	}

	public override void DrawGUILabel()
	{
		//grab the current GUI color, so we can modify it
		Color guiColor = GUI.color;

		if(cooldownRemaining > 0)
		{
			GUI.color = Color.red;
			GUILayout.Label(Mathf.CeilToInt(cooldownRemaining).ToString());
		}
		else if(!movement.isGrounded)
		{
			GUI.color = Color.red;
			GUILayout.Label("Not Grounded");
		}
		else
		{
			GUI.color = Color.green;
			GUILayout.Label("Ready");
		}

		//reset the GUI color
		GUI.color = guiColor;
	}

	public override void Reset()
	{
		if(movement)
			movement.moveSpeedModifier = 1f;
		cooldownRemaining = 0;
		StartCooldownCounter();
	}

	public override bool CanCast()
	{
		return cooldownRemaining <= 0 && movement.isGrounded;
	}

	public override void Cast(Vector3 castLoc, Quaternion castDir)
	{
		ResetCooldown();
		StartCoroutine(DoDash());
	}

	public IEnumerator DoDash()
	{
		float dashTimer = 0f;
		movement.moveSpeedModifier = 3f;
		while(dashTimer < duration)
		{
			dashTimer += Time.deltaTime;
			yield return 0;
		}
		movement.moveSpeedModifier = 1f;
	}

	//THESE SHOULD NEVER ACTUALLY BE USED
	public override Quaternion GetCastDirection() {return Quaternion.identity;}
	public override Vector3 GetCastPosition() {return Vector3.zero;}
}
