using UnityEngine;
using System.Collections;

public class AbilityBoxDropScript : Ability {

	CameraControllerScript cam;
	GameObject box;
	Vector3 castPos;

	void Awake()
	{
		GameObject game = GameObject.FindGameObjectWithTag("GameController");

		if(networkView.isMine)
			cam = game.GetComponent<CameraControllerScript>();
		else
			cam = null;

		box = (GameObject)Resources.Load("BoxDropObject");

		castPos = Vector3.zero;
		ClearCooldown();
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
		return cooldownRemaining <= 0 && GetCastPosition() != Vector3.zero;
	}
	
	public override void Cast(Vector3 castLoc, Quaternion castDir)
	{
		if(networkView.isMine)
		{
			GameObject obj = (GameObject)Network.Instantiate (box, castLoc, castDir, (int)NetworkGroup.GAME);
			obj.networkView.RPC("SetCaster", RPCMode.All, player.networkView.viewID);
			obj.networkView.RPC("SetCasterClient", RPCMode.All, player.build.owner);
		}
		ResetCooldown();

		//after we cast, we need to reset the cast position, so we don't incorrectly double calculate it
		castPos = Vector3.zero;
	}
	
	public override Quaternion GetCastDirection() { return Quaternion.identity; }
	
	public override Vector3 GetCastPosition()
	{
		//this check is what saves us from double raycasting to find the cast position
		if(castPos != Vector3.zero)
			return castPos;

		RaycastHit hitInfo;
		if( Physics.Raycast(cam.GetCameraPosition(), cam.GetCameraRotation() * Vector3.forward, out hitInfo, 100f) )			//there is an object we are trying to hit
		{
			castPos = hitInfo.point + new Vector3(0f, 4f, 0f);
			return castPos;
		}
		else 																				//there is no object where we are aiming
			return Vector3.zero;
	}
}
