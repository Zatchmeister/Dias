using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionTesterScript : MonoBehaviour {

	Dictionary<NetworkPlayer, bool> connectedStatus = new Dictionary<NetworkPlayer, bool>();
	NetworkManagerScript networkManager;
	float timer;

	void Awake()
	{
		timer = 0f;
	}

	void Start () 
	{
		networkManager = GetComponent<NetworkManagerScript>();
	}
	
	void Update () 
	{
		if(networkManager.connected)		//if we aren't connected, there's no need to test our connections
		{
			if( (timer += Time.deltaTime) >= GameSettings.connectionTestDelay )
			{
				//for all of our connections (that we have test data for), we need to test if they have responded since the last test
				UpdateConnections();

				//request a reply from all our real connections (yes, everyone that is actually connected)
				foreach(NetworkPlayer p in Network.connections)
				{
					networkView.RPC("SendReply", p);
				}

				//reset the timer
				timer = 0f;	
			}
		}
		else
		{
			connectedStatus.Clear();
		}
	}

	void UpdateConnections()
	{
		Dictionary<NetworkPlayer, bool> status = new Dictionary<NetworkPlayer, bool>(connectedStatus);
		foreach(KeyValuePair<NetworkPlayer, bool> connected in status)
		{
			if(!connected.Value)	//if the player failed our connection test, disconnect from them
			{
				networkManager.Disconnect(connected.Key);
				connectedStatus.Remove(connected.Key);
			}
			else 					//if the player passed our connection test, reset him for the next test
			{
				connectedStatus[connected.Key] = false;
			}
		}
	}

	[RPC]
	void SendReply(NetworkMessageInfo info)
	{
		networkView.RPC("ReceiveReply", info.sender);
	}
	
	[RPC]
	void ReceiveReply(NetworkMessageInfo info)
	{
		if(!connectedStatus.ContainsKey(info.sender))
			connectedStatus.Add(info.sender, true);
		else
			connectedStatus[info.sender] = true;
	}
}
