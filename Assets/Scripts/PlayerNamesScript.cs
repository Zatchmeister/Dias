using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNamesScript : MonoBehaviour {

	public static Dictionary<string, string> names = new Dictionary<string, string>();

	[RPC]
	void CallMeThis(string playerName, NetworkMessageInfo info)
	{
		string key = info.sender.ToString();
		if(PlayerNamesScript.names.ContainsKey(key))
			PlayerNamesScript.names.Remove(key);
		
		if(PlayerNamesScript.names.ContainsValue(playerName))
		{
			int num = 1;
			while(PlayerNamesScript.names.ContainsValue(playerName + num))
				num++;
			playerName = playerName + num;
		}
		
		PlayerNamesScript.names.Add (key, playerName);
	}
	
	[RPC]
	void PlayerLeft(NetworkPlayer player)
	{
		string key = player.ToString();
		if(PlayerNamesScript.names.ContainsKey (key))
			PlayerNamesScript.names.Remove(key);
		else
			Debug.Log ("Player we didn't know about left the game!?");
	}
}
