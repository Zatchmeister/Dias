using UnityEngine;
using System.Collections;

public class PlatformScript : MonoBehaviour {
	
	// Use this for initialization
	void Awake () 
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<PlatformManagerScript>().RegisterPlatform(this);	
	}
}
