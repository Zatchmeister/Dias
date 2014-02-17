using UnityEngine;
using System.Collections;

public class AbilityManagerScript : AbilityHitListScript {

	CameraControllerScript cam;				//the camera controller

	GameObject cone;						//cone area for conepushcone
	Vector3 relativeCastPos;

	// Use this for initialization
	void Start () 
	{
		if(networkView.isMine)
			cam = GameObject.FindGameObjectWithTag("GameController").GetComponent<CameraControllerScript>();
		else
			cam = null;
		relativeCastPos = new Vector3(0f, 1.5f, 0f);
		cone = (GameObject)Resources.Load("ConePushCone");
	}

	Quaternion GetCastDirection()
	{
		RaycastHit hitInfo;
		Quaternion camRot = cam.GetCameraRotation();
		if( Physics.Raycast(transform.position + relativeCastPos, camRot * Vector3.forward, out hitInfo, 100f) )
		{
			return Quaternion.LookRotation(hitInfo.point - (transform.position + relativeCastPos));
		}
		else
			return camRot;
	}

	//this should only be used locally by the player that is actually doing the casting of the ability
	public void Cast(int abilityNum) 
	{
		Vector3 castLoc = transform.position + relativeCastPos;
		Quaternion castDir = GetCastDirection();
		networkView.RPC("CastAbility", RPCMode.All, castLoc, castDir, abilityNum);
	}

	//this should be called by all clients
	[RPC]
	public void CastAbility(Vector3 castLoc, Quaternion castDir, int abilityNum, NetworkMessageInfo info) 
	{
		//If ability 1, cast a "cone push" -- temporary
		if(abilityNum == 1)
		{
			GameObject obj = (GameObject)GameObject.Instantiate (cone, castLoc, castDir);
			obj.GetComponent<ConePushScript>().SetCaster(info.networkView.viewID);
		}
	}

}
