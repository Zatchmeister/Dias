using UnityEngine;
using System.Collections;

public class AbilityAreaPushScript : Ability {

	GameObject area;						//area for conepushcone
	Vector3 relativeCastPos;
	CharacterMoveScript movement;
	
	// Use this for initialization
	void Awake()
	{
		relativeCastPos = new Vector3(0f, 1.5f, 0f);
		area = (GameObject)Resources.Load("AreaPushDetector");
		cooldownRemaining = 0f;
	}

	void Start()
	{
		movement = GetComponent<CharacterMoveScript>();
	}

	public override void DrawGUILabel()
	{
		DrawCooldownLabel();
	}
	
	public override void Reset()
	{
		ClearCooldown();
		StartCooldownCounter();
	}
	
	public override bool CanCast()
	{
		return cooldownRemaining <= 0 && movement.isGrounded;
	}
	
	public override void Cast(Vector3 castLoc, Quaternion castDir)
	{
		GameObject obj = (GameObject)GameObject.Instantiate (area, castLoc, castDir);
		AreaPushScript areaPush = obj.GetComponent<AreaPushScript>();
		areaPush.SetCaster(player.networkView.viewID);
		areaPush.SetCasterClient(player.build.owner);
		ResetCooldown();
	}
	
	public override Quaternion GetCastDirection() {return Quaternion.identity;}
	
	public override Vector3 GetCastPosition()
	{
		return transform.position + transform.TransformDirection(relativeCastPos);
	}
}
