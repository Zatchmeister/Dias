using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSettings 
{
	public static float minXCamSensitivity {get;private set;}			//minimum mouse X sensitivity
	public static float maxXCamSensitivity {get;private set;}			//maximum mouse X sensitivity
	public static float minYCamSensitivity {get;private set;}			//minimum mouse Y sensitivity
	public static float maxYCamSensitivity {get;private set;}			//maximum mouse Y sensitivity
	public static float minScrollSensitivity {get;private set;}			//minimum mouse scroll sensitivity (CANNOT BE LESS THAN 0)
	public static float maxScrollSensitivity {get;private set;}			//maximum mouse scroll sensitivity (CANNOT BE GREATER THAN 1)
	public static float minCamFOV {get;private set;}					//minimum FOV
	public static float maxCamFOV {get;private set;}					//maximum FOV
	public static string defaultPlayerName {get;private set;}			//default name given to players that have never played before

	public static float moveSpeed {get;private set;}					//standard movement speed;
	public static float standardJumpHeight {get;private set;}			//standard jump heigh
	public static float sprintSpeedIncrease {get;private set;}			//proportion the movement speed increases when sprinting
	public static float minLookDist {get;private set;}					//minimum distance of the camera from the player
	public static float maxLookDist {get;private set;}					//maximum distance of the camera from the player
	public static float telescopeSensitivity {get;private set;}			//value that look sensitivity is altered when in telescope mode
	public static float characterCamMinLookDist {get;private set;}		//Minimum zoom distance for the character camera
	public static float characterCamMaxLookDist {get;private set;}		//Maximum zoom distance for the character camera
	public static float characterCamVerticalOffset {get;private set;}			//character camera offset from vertical allowed
	public static float characterCamCharVisibleDistance {get;private set;}		//distance where character camera will start making the player become transparent
	public static Vector3 characterCamLocProportion {get;private set;}	//(Normalized) vector containing the proportion of x/y/z distance from the player to the character camera
	public static float telescopeLookBoundsUpper {get;private set;}		//camera upper bounds (degrees) for the telescope camera
	public static float telescopeLookBoundsLower {get;private set;}		//camera lower bounds (degrees) for the telescope camera
	public static float telescopeZoomBoundsUpper {get;private set;}		//maximum zoom distance for the telescope (by maximum, we mean most zoomed out)
	public static float telescopeZoomBoundsLower {get;private set;}		//minimum zoom distance for the telescope (by minimum, we mean most zoomed in)
	public static float controllerTriggerSensitivity {get;private set;}	//(0 to 1) point where the triggers are activated, and considered a button press
	public static float characterGroundedHeight {get;private set;}		//height on the character that a contact constitutes being grounded
	public static float characterGroundedTime {get;private set;}		//time the player remains considered on the ground, after leaving the surface

	public static float standardRespawnTime {get;private set;}			//standard movement speed;
	public static int standardJumpCount {get;private set;}				//standard number of jumps a player has

	public static float chatBoxOpacity{get;private set;}				//How see through the chatbox is in game
	public static float connectionTestDelay{get;private set;}				//time between each connection test

	public static float hitAddsToScoreTime {get;private set;}			//time before a player's death that hits will count as assists or kills on them

	static GameSettings()
	{
		moveSpeed = 20f;
		standardJumpHeight = 12f;
		sprintSpeedIncrease = 2f;
		telescopeSensitivity = 0.5f;
		
		characterCamMinLookDist = 5f;
		characterCamMaxLookDist = 15f;
		characterCamVerticalOffset = 10f;
		characterCamCharVisibleDistance = 4f;
		characterCamLocProportion = new Vector3(0.1f, 0.18f, -1f).normalized;
		
		telescopeLookBoundsUpper = 75f;
		telescopeLookBoundsLower = -75f;
		telescopeZoomBoundsUpper = 25f;
		telescopeZoomBoundsLower = 1f;
		
		controllerTriggerSensitivity = 0.2f;
		minXCamSensitivity = 10f;
		maxXCamSensitivity = 100f;
		minYCamSensitivity = 10f;
		maxYCamSensitivity = 100f;
		minScrollSensitivity = 0.2f;
		maxScrollSensitivity = 1f;
		minCamFOV = 40f;
		maxCamFOV = 120f;
		characterGroundedHeight = 0.3f;
		characterGroundedTime = 0.1f;

		defaultPlayerName = "New Player";

		standardRespawnTime = 1.5f;
		standardJumpCount = 1;

		chatBoxOpacity = 0.4f;
		connectionTestDelay = 3f;

		hitAddsToScoreTime = 5f;
	}

	//This class cannot be instantiated
	private GameSettings() {}
}

public sealed class Team
{
	public static readonly Team RED;
	public static readonly Team BLUE;
	
	private static List<Team> teamList;

	static Team()
	{
		teamList = new List<Team>();
		RED = new Team(0, Color.red, "Red");
		BLUE = new Team(1, Color.blue, "Blue");
	}
	
	public static Team GetTeam(int num)
	{
		foreach(Team t in teamList)
		{
			if(t.value == num)
				return t;
		}
		return null;
	}
	
	public static List<Team> GetTeamList()
	{
		return new List<Team>(teamList);
	}
	
	public int value {get; private set;}
	public Color color {get; private set;}
	public string name {get; private set;}
	
	private Team(int v, Color c, string n)		//only the static values can be created
	{
		value = v;
		color = c;
		name = n;
		Team.teamList.Add(this);
	}
	private Team() {}
}
