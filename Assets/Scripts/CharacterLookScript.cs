using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class CharacterLookScript : MonoBehaviour {

	//Variables set up in the Start Method
	float currLookDist;				//current distance of the camera from the player
	float goalLookDist;				//goal distance of the camera from the player, based on the player's modifications
	
	//Storage Variables -- DO NOT EDIT
	GameObject parentObject;
	PlayerSettingsScript settings;
	CameraControllerScript camController;
	public float lookBoundsLower{get; private set;}		//the upper angle the player is allowed to look at
	public float lookBoundsUpper{get; private set;}		//the lower angle the player is allowed to look at
	Material modelMaterial;

	void Awake()
	{
		parentObject = transform.parent.gameObject;
		modelMaterial = parentObject.transform.Find("CharacterModel").renderer.material;
		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		camController = game.GetComponent<CameraControllerScript>();
		settings = game.GetComponent<PlayerSettingsScript>();
		currLookDist = goalLookDist = 10f;

		//calculate the lower and upper limits based on the currect proportion in characterCamLocProportion
		lookBoundsUpper = Mathf.Abs( Mathf.Atan(GameSettings.characterCamLocProportion.z / GameSettings.characterCamLocProportion.y) ) * Mathf.Rad2Deg;
		lookBoundsLower = lookBoundsUpper - 180;

		//apply an offset, so the camera won't look over the top, or under the bottom
		lookBoundsUpper -= GameSettings.characterCamVerticalOffset;
		lookBoundsLower += GameSettings.characterCamVerticalOffset;
	}

	void Update()
	{
		//if our goal distance is different than our current distance, push the current distance toward the goal distance
		if(currLookDist != goalLookDist)
		{
			//slowly move the camera from the currlookdist to the goallookdist, but don't let it go beyond min or max
			goalLookDist = Mathf.Clamp(goalLookDist, GameSettings.characterCamMinLookDist, GameSettings.characterCamMaxLookDist);
			currLookDist = Mathf.Lerp (currLookDist, goalLookDist, 0.2f);
		}

		//set out FOV equal to what the player has set in their settings
		camera.fieldOfView = settings.cameraFOV;
	}

	//returns the offset angle of the camera from the object it is looking at
	public float GetOffsetLookAngle()
	{
		Vector3 defaultCameraPos = parentObject.transform.position + Quaternion.identity * GameSettings.characterCamLocProportion * currLookDist;
		Vector3 lookAtPos = GetLookAtPos();
		float opposite = defaultCameraPos.y - lookAtPos.y;
		float adjascent = defaultCameraPos.z - lookAtPos.z;
		return Mathf.Atan(opposite/adjascent) * Mathf.Rad2Deg;
	}

	public void Disable()
	{
		if(gameObject.activeInHierarchy)
			camController.lookYAng -= GetOffsetLookAngle();		//we are going to remove the weird angle issue with this camera

		gameObject.SetActive(false);
	} 

	public void Enable()
	{
		if(!gameObject.activeInHierarchy)
			camController.lookYAng += GetOffsetLookAngle();		//we are going to remove the weird angle issue with this camera

		gameObject.SetActive(true);
	}
	
	//should be called once per frame
	public void Zoom(float zoom)
	{
		goalLookDist += zoom * settings.scrollCameraSensitivity;
	}

	//should be called once per frame
	public void Look(float lookX, float lookY)
	{
		//update the camera x and y angles in the camera controller, based on our movement for this camera
		camController.lookXAng += (lookX * Time.deltaTime);
		camController.lookYAng = Mathf.Clamp(camController.lookYAng + (lookY * Time.deltaTime), lookBoundsLower, lookBoundsUpper);

		//find the position we want to put the camera at, and where we want it looking at
		Vector3 newCamPos = GetPositionAround(parentObject.transform.position, camController.lookYAng, camController.lookXAng, currLookDist, GameSettings.characterCamLocProportion);
		Vector3 lookAtPos = GetLookAtPos();

		//shoot a ray from the lookat position to the new camera position, if we hit something, set our position equal to the place we hit
		RaycastHit hitInfo;
		Vector3 directionToCamera = newCamPos - lookAtPos;
		Color modelColor = modelMaterial.GetColor("_Color");
		if( Physics.Raycast(lookAtPos, directionToCamera, out hitInfo, directionToCamera.magnitude, Physics.DefaultRaycastLayers) )
		{
			//set the desired new camera position to away from the hitpoint from the raycast
			newCamPos = hitInfo.point + (hitInfo.normal/10);

			//set the look at position based on the new camera position
			lookAtPos = GetLookAtPos();

			//set the character model's color's alpha as the camera approaches it
			if(hitInfo.distance < GameSettings.characterCamCharVisibleDistance)	//we need to make the player slightly less visible
				modelColor.a = Mathf.Lerp(-1f, 1f, hitInfo.distance / GameSettings.characterCamCharVisibleDistance);
			else 						//we need to return the player to regular opacity
				modelColor.a = 1;

		}
		else
			modelColor.a = 1;		
		modelMaterial.SetColor("_Color", modelColor);

		//move the camera to the desired position, and set it's lookat equal to the lookat position
		camera.transform.position = newCamPos;
		camera.transform.LookAt(lookAtPos);
	}

	Vector3 GetPositionAround(Vector3 origin, float yAng, float xAng, float distance, Vector3 proportion)
	{
		return origin + Quaternion.Euler(yAng, xAng, 0) * proportion * distance;
	}

	Vector3 GetLookAtPos()
	{
		Vector3 lookAtPos = GameSettings.characterCamLocProportion;
		lookAtPos.z = 0f;
		return GetPositionAround(parentObject.transform.position, camController.lookYAng, camController.lookXAng, currLookDist, lookAtPos);
	}
}
