using UnityEngine;
using System;
using System.Collections;

public enum NetworkGroup{GAME = 0, CHAT = 1}

/*	Handles creating of a server and client, as well as provides some functions for creating and removing players and gameobjects over the network
 */
public class NetworkManagerScript : MonoBehaviour {

	int port = 35930;
	int connections = 32;
	
	//actual variables
	public bool connected {get; private set;}
	public bool connecting {get; private set;}
	public bool isServer {get {return Network.isServer;}}
	public NetworkViewID myID {get; private set;}

	PlayerDataScript playerData;

	void Awake()
	{
		connected = false;
		connecting = false;
	}

	void Start()
	{
		playerData = GetComponent<PlayerDataScript>();
	}

	public void StartServer()
	{
		connecting = true;
		Network.InitializeServer(connections, port, !Network.HavePublicAddress());
	}

	public void StartClient(string ip)
	{
		connecting = true;
		Network.Connect(ip, port);
	}

	public void Disconnect()
	{
		connected = false;
		connecting = false;
		Network.Disconnect();
		ClearMyID();
		playerData.RemoveAllPlayers();
	}

	public void Disconnect(NetworkPlayer p)
	{
		foreach(NetworkPlayer conn in Network.connections)
		{
			if(conn == p)
			{
				Network.CloseConnection(p, true);
				return;
			}
		}
	}

	public void ClearAllRPCs()
	{
		foreach(NetworkPlayer p in Network.connections)
			Network.RemoveRPCs(p);			//remove any leftover rpc calls
		Network.RemoveRPCs(Network.player);
	}

	public void NetworkDestroy(GameObject go)
	{
		if(go.networkView.isMine)
			GameObject.Destroy(go);
		else
			Network.Destroy(go);
	}

	public void NetworkDestroyObjectsWithTag(string tag)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag(tag);
		foreach(GameObject go in players)
		{
			Network.Destroy(go);
			Network.RemoveRPCs(go.networkView.viewID);
		}
	}

	public string GetServerIP()
	{
		if(isServer)
			return Network.player.ipAddress;
		else
		{
			if(Network.connections.Length > 0)
				return Network.connections[0].ipAddress;
			else
				return "Unknown";
		}
	}

	void GenerateMyID()
	{
		myID = Network.AllocateViewID();
	}

	void ClearMyID()
	{
		myID = NetworkViewID.unassigned;
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		playerData.RemovePlayer(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection disconnection)
	{
		connected = false;
		connecting = false;
		ClearMyID();
		playerData.RemoveAllPlayers();
	}

	void OnFailedToConnect()
	{
		connected = false;
		connecting = false;
	}

	void OnServerInitialized()
	{
		connected = true;
		connecting = false;
		GenerateMyID();
		playerData.SendMyUpdatedPlayerData();
	}

	void OnConnectedToServer()
	{
		connected = true;
		connecting = false;
		GenerateMyID();
		playerData.LoadAllPlayerData();
		playerData.SendMyBufferedUpdatedPlayerData();
	}
}
