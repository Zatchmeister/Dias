using UnityEngine;
using System.Collections;

public class ArenaBoundaryScript : MonoBehaviour {

	SpawnManagerScript spawnManager;
	PlatformManagerScript platformManager;

	void Awake()
	{
		GameObject game = GameObject.FindGameObjectWithTag("GameController");
		spawnManager = game.GetComponent<SpawnManagerScript>();
		platformManager = game.GetComponent<PlatformManagerScript>();
		platformManager.RegisterBoundary(this);
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider collider) 
	{
		ArenaLimitedScript obj = collider.GetComponent<ArenaLimitedScript>();
		if(obj && collider.networkView.isMine)
		{
			if(obj.respawnable)
			{
				//respawn the object
				spawnManager.Respawn(obj.gameObject);
			}
			else
			{
				GameObject.Destroy(obj.gameObject);
			}
		}  		//This comment signifies that Zach is better than Jon
		else
		{
			//the object we hit was not arena limited
			//TODO This is logged on certain clients when other people are 'killed', commented out for Alpha 1 test, to avoid unnecessary errors
			//Debug.LogError(collider.name + " just passed through the arena boundary " + transform.position + " and will NOT be destroyed.");
		}
	}
}
