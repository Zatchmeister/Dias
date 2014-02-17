using UnityEngine;
using System.Collections;

public class PlayerInputScript : MonoBehaviour {

	CharacterBuildScript character;
	CharacterMoveScript movement;
	CameraControllerScript cameraController;
	PlayerSettingsScript settings;
	bool ability1IsHeld;			//used to detect if the controller is inputting on the Ability 1 axis, and simulate a GetButtonDown with it
	bool ability2IsHeld;			//used to detect if the controller is inputting on the Ability 2 axis, and simulate a GetButtonDown with it

	// Use this for initialization
	void Start () 
	{
		character = GetComponent <CharacterBuildScript>();
		movement = GetComponent<CharacterMoveScript>();
		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		cameraController = game.GetComponent<CameraControllerScript>();
		settings = game.GetComponent<PlayerSettingsScript>();
		ability1IsHeld = false;
		ability2IsHeld = false;
	}

	void OnEnable()
	{
		Screen.lockCursor = true;
	}

	void OnDisable()
	{
		Screen.lockCursor = false;
	}

	void FixedUpdate()
	{
		// character movement must happen in fixed update, because it is based on the physics engine, which get's cranky if you don't apply forces in fixed update
		float movex = Input.GetAxis("Horizontal");
		if(settings.xMovementInversion)
			movex = -movex;
		
		float movey = Input.GetAxis("Vertical");

		movement.Move(movex, movey);
	}
	
	// Update is called once per frame
	void Update() 
	{
		float lookx = Input.GetAxis("Look X") * settings.xCameraSensitivity;
		if(settings.xCameraInversion)
			lookx = -lookx;

		float looky = Input.GetAxis("Look Y") * settings.yCameraSensitivity;
		if(settings.yCameraInversion)
			looky = -looky;

		float camzooming = Input.GetAxis("Camera Zooming") * settings.scrollCameraSensitivity;

		bool jumping = Input.GetButtonDown("Jump");
		bool dashing = Input.GetButtonDown("Dash");
		bool telescopeZoom = Input.GetButton("Telescope Zoom");
		bool distanceCheck = Input.GetButtonDown("Distance Check");
		bool score = Input.GetButtonDown("Score");

		bool ability1ControllerPressed = Input.GetAxis("Ability 1") > GameSettings.controllerTriggerSensitivity;
		bool ability2ControllerPressed = Input.GetAxis("Ability 2") > GameSettings.controllerTriggerSensitivity;
		bool ability1 = Input.GetButtonDown("Ability 1") || (!ability1IsHeld && ability1ControllerPressed);
		bool ability2 = Input.GetButtonDown("Ability 2") || (!ability2IsHeld && ability2ControllerPressed);
		bool ability3 = Input.GetButtonDown("Ability 3");
		bool ability4 = Input.GetButtonDown("Ability 4");
		ability1IsHeld = ability1ControllerPressed;
		ability2IsHeld = ability2ControllerPressed;

		/*
		if(distanceCheck)
			Debug.Log("distance check");
		*/

		cameraController.Look(lookx, looky, camzooming, telescopeZoom);
		movement.Jump(jumping);
		character.Cast(ability1, ability2, ability3, ability4, dashing);
	}
}
