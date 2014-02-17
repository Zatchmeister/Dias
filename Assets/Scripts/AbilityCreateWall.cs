using UnityEngine;
using System.Collections;

public class AbilityCreateWall : Ability
{
	NetworkManagerScript networkManager;
	CameraControllerScript cam;
	GameObject wall;
	Vector3 castPos;

	void Awake ()
	{
		GameObject game = GameObject.FindGameObjectWithTag ("GameController");
		networkManager = game.GetComponent<NetworkManagerScript> ();
	
		if (networkView.isMine)
			cam = game.GetComponent<CameraControllerScript> ();
		else
			cam = null;
	
		wall = (GameObject)Resources.Load ("WallObject");
	
		castPos = Vector3.zero;
		ClearCooldown ();
	}
	
	public override void DrawGUILabel ()
	{
		DrawCooldownLabel ();
	}
	
	public override void Reset ()
	{
		ClearCooldown ();
		StartCooldownCounter ();
	}
	
	public override bool CanCast ()
	{
		return cooldownRemaining <= 0d && GetCastPosition () != Vector3.zero;
	}
	
	public override void Cast (Vector3 castLoc, Quaternion castDir)
	{
		if (networkView.isMine) {
			GameObject obj = (GameObject)Network.Instantiate (wall, castLoc, castDir, (int)NetworkGroup.GAME);
			obj.networkView.RPC ("SetCaster", RPCMode.All, networkManager.myID);
		}
		ResetCooldown ();
	
		//after we cast, we need to reset the cast position, so we don't incorrectly double calculate it
		castPos = Vector3.zero;
	}
	
	public override Quaternion GetCastDirection ()
	{ 
		Vector3 camRot = cam.GetCameraRotation ().eulerAngles; 
		camRot.z = 0;
		camRot.x = 0;
		return Quaternion.Euler (camRot);
	}
	
	public override Vector3 GetCastPosition ()
	{ 
		//this check is what saves us from double raycasting to find the cast position
		if (castPos != Vector3.zero)
			return castPos;
	
		RaycastHit hitInfo;
		if (Physics.Raycast (cam.GetCameraPosition (), cam.GetCameraRotation () * Vector3.forward, out hitInfo, 50f)) {		//there is an object we are trying to hit
			castPos = hitInfo.point + new Vector3 (0f, .75f, 0f);
			return castPos;
		} else 										//there is no object where we are aiming
			return Vector3.zero;
	}
}
