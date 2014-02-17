using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatRoomScript : MonoBehaviour 
{	
	public static List<string> chatBox = new List<string>();
	PlayerDataScript playerData;
	NetworkManagerScript networkManager;
	
	Vector2 scrollPos;
	string stringToEdit = "";

	void Start()
	{
		networkManager = GetComponent<NetworkManagerScript>();
		playerData = GetComponent<PlayerDataScript>();
	}

	[RPC]
	void SendChatMessage(string msg, NetworkViewID player)
	{
		string sender = playerData.GetPlayerData(player).playerName;
		msg = sender + ": " + msg;
		chatBox.Add(msg);
		scrollPos.y = float.MaxValue;
	}

	public void GUIMethod(Rect area, float width, float height, bool inGame) 
	{
		GUILayout.BeginArea (area);
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical ("box");
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width (width), GUILayout.Height(height));
		foreach(string msg in chatBox)
			GUILayout.Label (msg);	
		GUILayout.EndScrollView ();
		GUILayout.EndVertical();
		
		if (Event.current.Equals(Event.KeyboardEvent("return")))
		{
			// If we're in-game we need to see what has focus
			// In the lobby the chat always has "focus"
			if(inGame)
			{
				// If our focus is on the chatbox
				if(GUI.GetNameOfFocusedControl() == "chatField")
				{
					SendChat (stringToEdit);
					stringToEdit = "";
					GUI.FocusControl ("");
				}
				// If our focus is NOT on the chatbox
				else
					GUI.FocusControl("chatField");
			}
			else
			{
				SendChat (stringToEdit);
				stringToEdit = "";
			}

		}

		GUI.SetNextControlName ("chatField");
		stringToEdit = GUILayout.TextField (stringToEdit); 
		GUILayout.FlexibleSpace();
		GUILayout.EndArea ();
	}

	void SendChat(string msg)
	{
		if(msg != "")
			networkView.RPC ("SendChatMessage", RPCMode.All, msg, networkManager.myID);
	}
}
