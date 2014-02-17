using UnityEngine;
using System.Collections;

public class CameraControllerScript : MonoBehaviour {

	public float lookXAng {get; set;}		//angle of the camera along the X rotational axis, SHOULD ONLY BE CHANGED BY THE CAMERAS
	public float lookYAng {get; set;}		//angle of the camera along the Y rotational axis, SHOULD ONLY BE CHANGED BY THE CAMERAS
	CharacterLookScript character;
	TelescopeScript telescope;
	CharacterMoveScript movement;
	bool telescopeMode;

	void Awake()
	{
		lookXAng = lookYAng = 0;
		telescopeMode = false;
	}

	public Vector3 GetCameraPosition()
	{
		if(telescopeMode)
			return telescope.transform.position;
		else
			return character.transform.position;
	}

	public Quaternion GetCameraRotation()
	{
		if(telescopeMode)
			return telescope.transform.rotation;
		else
			return character.transform.rotation;
	}

	public void RegisterPlayer(GameObject player)
	{
		character = player.transform.Find("CharacterCamera").GetComponent<CharacterLookScript>();
		telescope = player.transform.Find("TelescopeCamera").GetComponent<TelescopeScript>();
		movement = player.GetComponent<CharacterMoveScript>();
	}

	public void Look(float lookX, float lookY, float zooming, bool tsMode)
	{
		if(telescopeMode = tsMode)
		{
			character.Disable();
			telescope.Enable();
			telescope.Look(lookX, lookY);
			telescope.Zoom(zooming);
		}
		else
		{
			telescope.Disable();
			character.Enable();
			character.Look(lookX, lookY);
			character.Zoom(zooming);
		}

		//rotate the player left/right with the camera
		//possible TODO only do this if WE are in control of the character, this could be used for follow camera views while the player is waiting to respawn
		movement.SetYRotation(lookXAng);
	}
}
