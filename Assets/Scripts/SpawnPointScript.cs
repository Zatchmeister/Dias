using UnityEngine;
using System.Collections;

public class SpawnPointScript : MonoBehaviour {

	// Use this for initialization
	void OnNetworkInstantiate(NetworkMessageInfo info) 
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<SpawnManagerScript>().RegisterSpawnPoint(this);
	}
}
