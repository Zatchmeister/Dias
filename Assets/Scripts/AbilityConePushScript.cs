using UnityEngine;
using System.Collections;

public class AbilityConePushScript : Ability
{
	GameObject cone;						//cone area for conepushcone
	Vector3 relativeCastPos;
	CameraControllerScript cam;

	// Use this for initialization
	void Awake ()
	{		
		if (networkView.isMine)
			cam = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CameraControllerScript> ();
		else
			cam = null;

		relativeCastPos = new Vector3 (0f, 1.5f, 0f);
		cone = (GameObject)Resources.Load ("ConePushCone");
		cooldownRemaining = 0f;
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
		return cooldownRemaining <= 0;
	}

	public override void Cast (Vector3 castLoc, Quaternion castDir)
	{
		GameObject obj = (GameObject)GameObject.Instantiate (cone, castLoc, castDir);
		ConePushScript conePush = obj.GetComponent<ConePushScript> ();
		conePush.SetCaster (player.networkView.viewID);
		conePush.SetCasterClient (player.build.owner);
		ResetCooldown ();
	}

	public override Quaternion GetCastDirection ()
	{
		RaycastHit hitInfo;
		Quaternion camRot = cam.GetCameraRotation ();
		Vector3 castPos = GetCastPosition ();
		if (Physics.Raycast (castPos, camRot * Vector3.forward, out hitInfo, 100f)) {			//there is an object we are trying to hit
			return Quaternion.LookRotation (hitInfo.point - castPos);
		} else { 																				//there is no object where we are aiming
			return camRot;
		}
	}

	public override Vector3 GetCastPosition ()
	{
		return transform.position + transform.TransformDirection (relativeCastPos);
	}
}
