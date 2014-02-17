using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformManagerScript : MonoBehaviour {

	public GameObject platformObject;
	public GameObject boundaryCube;
	public Vector3 arenaSize {get; private set;}
	List<PlatformScript> platforms;
	List<ArenaBoundaryScript> boundaries;

	void Awake()
	{
		platforms = new List<PlatformScript>();
		boundaries = new List<ArenaBoundaryScript>();
	}

	public Vector3 GenerateArena()
	{
		arenaSize = new Vector3(200f, 50f, 200f);

		//generate the platforms in the arena
		List<Vector3> platformPositions = new List<Vector3>();
		platformPositions.Add(new Vector3(75f, 30f, 15f));
		platformPositions.Add(new Vector3(75f, 30f, 135f));
		platformPositions.Add(new Vector3(75f, 50f, 75f));
		platformPositions.Add(new Vector3(105f, 20f, 30f));
		platformPositions.Add(new Vector3(105f, 20f, 120f));
		platformPositions.Add(new Vector3(135f, 15f, 50f));
		platformPositions.Add(new Vector3(135f, 15f, 100f));
		platformPositions.Add(new Vector3(90f, 10f, 55f));
		platformPositions.Add(new Vector3(90f, 10f, 95f));
		platformPositions.Add(new Vector3(50f, 40f, 45f));
		platformPositions.Add(new Vector3(50f, 40f, 105f));
		platformPositions.Add(new Vector3(25f, 30f, 55f));
		platformPositions.Add(new Vector3(25f, 30f, 95f));
		platformPositions.Add(new Vector3(60f, 20f, 75f));

		for(int k = 0; k < platformPositions.Count; k++) 
		{
			platformPositions[k] -= new Vector3(75, 25, 75);		//this line corrects for the fact that Jon is an idiot and built his placeholder arena from 0,0,0 to 150,50,150 rather than -75,-25,-75 to 75,25,75
			Network.Instantiate(platformObject, platformPositions[k], platformObject.transform.rotation, (int)NetworkGroup.GAME);
		}

		//create the 6 boundaries to the arena
		GenerateBoundaries(arenaSize);

		return arenaSize;
	}

	public void RegisterPlatform(PlatformScript p)
	{
		platforms.Add(p);
	}

	public void RegisterBoundary(ArenaBoundaryScript boundary)
	{
		boundaries.Add(boundary);
	}

	public void ClearArena()
	{
		//destroy the platforms
		foreach(PlatformScript platform in platforms)
		{
			GameObject.Destroy(platform.gameObject);
		}
		platforms.Clear();

		//destroy the boundaries
		foreach(ArenaBoundaryScript boundary in boundaries)
		{
			GameObject.Destroy(boundary.gameObject);
		}
		boundaries.Clear();
	}

	public PlatformScript FindNearestPlatform(Vector3 pos)
	{
		PlatformScript closest = null;
		float dist = float.MaxValue, tmp;
		foreach(PlatformScript platform in platforms)
		{
			tmp = (platform.transform.position - pos).sqrMagnitude;
			if(!closest || dist > tmp)
			{
				closest = platform;
				dist = tmp;
			}
		}
		return closest;
	}

	[RPC]
	public void SpawnBoundary(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		GameObject go = (GameObject)GameObject.Instantiate(boundaryCube, pos, rot);
		go.transform.localScale = scale;
	}

	void GenerateBoundaries(Vector3 arenaSize)
	{
		Vector3 padding = new Vector3(platformObject.transform.localScale.x * 1.5f, GameSettings.standardJumpHeight * 5f, platformObject.transform.localScale.z * 1.5f);
		arenaSize += padding;
		float boundaryWidth = 10f;
		networkView.RPC("SpawnBoundary", RPCMode.AllBuffered, new Vector3(-(arenaSize.x/2), 0f, 0f), Quaternion.Euler(0f, 0f, 0f), new Vector3(boundaryWidth, arenaSize.y, arenaSize.z));		//left
		networkView.RPC("SpawnBoundary", RPCMode.AllBuffered, new Vector3((arenaSize.x/2), 0f, 0f), Quaternion.Euler(0f, 0f, 0f), new Vector3(boundaryWidth, arenaSize.y, arenaSize.z));		//right
		networkView.RPC("SpawnBoundary", RPCMode.AllBuffered, new Vector3(0f, (arenaSize.y/2), 0f), Quaternion.Euler(0f, 0f, 0f), new Vector3(arenaSize.x, boundaryWidth, arenaSize.z));		//front
		networkView.RPC("SpawnBoundary", RPCMode.AllBuffered, new Vector3(0f, -(arenaSize.y/2), 0f), Quaternion.Euler(0f, 0f, 0f), new Vector3(arenaSize.x, boundaryWidth, arenaSize.z));		//back
		networkView.RPC("SpawnBoundary", RPCMode.AllBuffered, new Vector3(0f, 0f, (arenaSize.z/2)), Quaternion.Euler(0f, 0f, 0f), new Vector3(arenaSize.x, arenaSize.y, boundaryWidth));		//top
		networkView.RPC("SpawnBoundary", RPCMode.AllBuffered, new Vector3(0f, 0f, -(arenaSize.z/2)), Quaternion.Euler(0f, 0f, 0f), new Vector3(arenaSize.x, arenaSize.y, boundaryWidth));		//bottom
	}
}
