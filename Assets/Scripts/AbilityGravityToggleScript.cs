using UnityEngine;
using System.Collections;

public class AbilityGravityToggleScript : Ability
{
	CharacterMoveScript movement;
	
	void Start ()
	{
		movement = GetComponent<CharacterMoveScript> ();
	}
	
	public override void DrawGUILabel ()
	{
		string label = "Gravity: ";
		label += rigidbody.useGravity ? "On" : "Off";
		GUILayout.Label (label);
	}
	
	public override void Reset ()
	{
		rigidbody.useGravity = true;
		if (movement)
			movement.ResetMoveType ();
	}
	
	public override bool CanCast ()
	{
		return true;		//they can always toggle this on and off
	}
	
	public override void Cast (Vector3 castLoc, Quaternion castDir)
	{
		rigidbody.useGravity = !rigidbody.useGravity;
		movement.ToggleMoveType ();
	}
	
	//THESE SHOULD NEVER ACTUALLY BE USED
	public override Quaternion GetCastDirection ()
	{
		return Quaternion.identity;
	}
	public override Vector3 GetCastPosition ()
	{
		return Vector3.zero;
	}
}
