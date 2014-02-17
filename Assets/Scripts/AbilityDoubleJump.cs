using UnityEngine;
using System.Collections;

public class AbilityDoubleJump : Ability 
{
	CharacterBuildScript build;

	void Start ()
	{
		build = GetComponent<CharacterBuildScript>();
		build.totalJumps++;
	}

	public override void DrawGUILabel()
	{
		GUILayout.Label("Jumps = " + build.jumpCount);
		//there is nothing to draw, because this ability isn't really 'activated' in the same manner as other abilityies
	}

	//THESE WILL NEVER ACTUALLY BE USED
	public override void Reset() {}
	public override bool CanCast() {return true;}
	public override void Cast(Vector3 castLoc, Quaternion castDir) {}
	public override Quaternion GetCastDirection() {return Quaternion.identity;}
	public override Vector3 GetCastPosition() {return Vector3.zero;}
}
