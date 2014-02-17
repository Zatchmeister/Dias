using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManagerScript : MonoBehaviour {

	List<SpawnPointScript> spawnPoints;

	List<GameObject> respawning;

	GameObject playerObject;		//the gameobject that represents the player (For the initial spawn)

	ScoreKeeperScript scoreKeeper;
	GameManagerScript gameManager;
	MenuManager menuManager;

	// Use this for initialization
	void Awake ()
	{
		playerObject = (GameObject)Resources.Load("Character");
		spawnPoints = new List<SpawnPointScript>();
		respawning = new List<GameObject>();
	}

	void Start()
	{
		gameManager = GetComponent<GameManagerScript>();
		menuManager = GetComponent<MenuManager>();
	}

	public void RegisterSpawnPoint(SpawnPointScript spawnPoint)
	{
		spawnPoints.Add(spawnPoint);
	}

	public void ClearSpawnPoints()
	{
		spawnPoints.Clear();
	}

	public void Respawn(GameObject go)
	{
		if(go.networkView)
		{
			respawning.Add(go);
			networkView.RPC("BeginNetworkRespawn", RPCMode.Others, go.networkView.viewID);
		}

		CharacterBuildScript build = go.GetComponent<CharacterBuildScript>();
		if(build)
		{
			build.RespawnAction(true);						//tell the character that we have started to respawn them

			scoreKeeper.ReportDeath(build);		//report the death to the scorekeeper
		}
		StartCoroutine( DoRespawn(go) );
	}

	IEnumerator DoRespawn(GameObject go)
	{
		go.SetActive(false);
		menuManager.EnableMenuBackgroundCamera(true);
		for(float respawnTime = GameSettings.standardRespawnTime; respawnTime >= 0; respawnTime -= Time.deltaTime)
		{
			if(!gameManager.gameIsActive)
			{
				GameObject.Destroy(go);
				respawning.Remove(go);
				return false;
			}
			yield return 0;
		}

		Rigidbody rb = go.GetComponent<Rigidbody>();
		rb.velocity = Vector3.zero;

		Transform newLocation = GetSpawnPoint();
		go.transform.position = newLocation.position;
		go.transform.rotation = newLocation.rotation;

		if(go.networkView)
		{
			respawning.Remove(go);
			networkView.RPC("EndNetworkRespawn", RPCMode.Others, go.networkView.viewID, go.transform.position, go.transform.rotation);
		}
		
		menuManager.EnableMenuBackgroundCamera(false);
		go.SetActive(true);

		//we must restart the cooldowns after the object is active again, otherwise they will not properly start (because coroutines don't run on inactive objects)
		CharacterBuildScript build = go.GetComponent<CharacterBuildScript>();
		if(build)
			build.RespawnAction(false);		//tell the build we are done respawning
	}

	[RPC]
	void BeginNetworkRespawn(NetworkViewID id)
	{
		GameObject go = NetworkView.Find(id).gameObject;
		respawning.Add(go);
		go.SetActive(false);
	}

	[RPC]
	void EndNetworkRespawn(NetworkViewID id, Vector3 pos, Quaternion rot)
	{
		//we can't use NetworkView.Find(id), because the object should be disabled, so we have to grab it from our own list
		GameObject go = null;
		foreach(GameObject obj in respawning)
		{
			if(obj.networkView.viewID == id)
				go = obj;
		}

		if(!go)
			Debug.LogError("We are attempting to respawn an object that can't be found");

		go.SetActive(true);

		NetworkCharacterScript netCharacter = go.GetComponent<NetworkCharacterScript>();
		if(netCharacter)
			netCharacter.SetTransform(pos, rot);

		respawning.Remove(go);
	}

	public GameObject SpawnPlayer()
	{
		Transform t = GetSpawnPoint();
		return (GameObject)Network.Instantiate(playerObject, t.position, t.rotation, int.Parse(Network.player.ToString()));
	}

	Transform GetSpawnPoint()
	{
		return spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
	}

	public void RegisterScoreKeeper(ScoreKeeperScript sk)
	{
		scoreKeeper = sk;
	}
	
	public void ClearScoreKeeper()
	{
		scoreKeeper = null;
	}
}