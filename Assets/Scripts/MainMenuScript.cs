using UnityEngine;
using System.Collections;

public class MainMenuScript : MenuScript {

	NetworkManagerScript networkManager;
	string networkIP = "127.0.0.1";

	SettingsMenu settings;
	bool sendToSettings = false;

	GameLobbyScript lobby;
	bool sendToLobby = false;

	// Use this for initialization
	void Start ()
	{
		lobby = GetComponent<GameLobbyScript>();
		settings = GetComponent<SettingsMenu>();
		networkManager = GetComponent<NetworkManagerScript>();
	}

	public override void GUIMethod()
	{
		if(networkManager.connected)
			sendToLobby = true;
		else if(networkManager.connecting)
			GUILayout.Label("Connecting");
		else
		{
			BeginScreenCentering();
			
			if(GUILayout.Button("Create Server"))
				networkManager.StartServer();

			GUILayout.BeginHorizontal();
			networkIP = GUILayout.TextField(networkIP, GUILayout.Width(80));
			if(GUILayout.Button("Join Server"))
				networkManager.StartClient(networkIP);
			GUILayout.EndHorizontal();
			
			if(GUILayout.Button("Settings"))
				sendToSettings = true;

			if(GUILayout.Button("Post-Play Questionnaire"))
				Application.OpenURL("https://docs.google.com/forms/d/1ShENM4RCEFEHZDjpFBLB-MQY3HIDwZBWSgwLFIPfYJQ/viewform");
			
			if(GUILayout.Button("Exit"))
				Application.Quit();
			
			EndScreenCentering();
		}
	}

	public override MenuScript GetNextMenu()
	{
		if(sendToLobby)
		{
			sendToLobby = false;
			return lobby;
		}
		else if(sendToSettings)
		{
			sendToSettings = false;
			settings.SetPrevious(this);
			return settings;
		}

		return this;
	}
}
