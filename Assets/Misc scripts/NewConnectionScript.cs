using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInfo {
	public string name;
	public NetworkPlayer networkPlayer;
	public string team;
	public bool ready = false;

	public PlayerInfo(string _name, string _team, NetworkPlayer player)
	{
		name = _name;
		team = _team;
		networkPlayer = player;
	}
}
public class NewConnectionScript : MonoBehaviour {
	string playerName = "Player";
	string connectIp = "127.0.0.1";
	string team = "1";
	string rounds = "3";
	bool ready;
	TransferVariables transferScript;

	public GameObject spellSelections;

	enum MenuState { MainMenu, Lobby };

	MenuState menuState = MenuState.MainMenu;

	List<PlayerInfo> playerList = new List<PlayerInfo>();

	public GUIStyle text;
	public GUIStyle style;
	public GUIStyle textField;
	public GUIStyle toggle;

    public InputField nameField;

	void Start () {
		transferScript = (TransferVariables)GameObject.Find ("TransferVariables").GetComponent("TransferVariables");
        nameField.text = ReadPlayerName();
        //spellSelections.SetActive(false);
        //RefreshHostList ();
    }
	
    public void SetPlayerName(string name)
    {
        playerName = name;
        PlayerPrefs.SetString("Player Name", playerName);
        
    }

    public string ReadPlayerName()
    {
        if (PlayerPrefs.HasKey("Player Name"))
        {
            playerName = PlayerPrefs.GetString("Player Name");
        }
        return playerName;
    }

    public void StartServer()
    {
        transferScript.isHost = true;
        transferScript.team = int.Parse(team);
        menuState = MenuState.Lobby;
        //spellSelections.SetActive(true);
        Network.InitializeServer(32, 25001, true);
        MasterServer.RegisterHost("BotM" + Application.version, playerName + "'s game");
        CancelInvoke("RefreshHostList");
    }

    public void Refresh()
    {
        MasterServer.RequestHostList("BotM" + Application.version);
        if (MasterServer.PollHostList().Length != 0)
        {
            HostData[] hostData = MasterServer.PollHostList();
            int i = 0;
            while (i < hostData.Length)
            {
                Debug.Log(hostData[i]);
            }
        }
    }

    void OnGUI(){
		if(menuState == MenuState.MainMenu)
		{
			if (MasterServer.PollHostList().Length != 0) {
	        	HostData[] hostData = MasterServer.PollHostList();
	        	int i = 0;
	        	while (i < hostData.Length) {
                    Debug.Log(hostData[i]);
					Upgrading.DrawOutline(new Rect(10, 250, 100, 100), "Game name: " + hostData[i].gameName, text, Color.black);
					if(GUI.Button (new Rect(220, 250, 100, 20), "", style)){
						string tmpIp = "";
	           	 		int j = 0;
	            		while (j < hostData[i].ip.Length) {
	                		tmpIp = hostData[i].ip[j] + " ";
	                		j++;
	            		}
						transferScript.connectIp = tmpIp;
						transferScript.team = int.Parse(team);
						menuState = MenuState.Lobby;
						spellSelections.SetActive(true);
						Network.Connect(tmpIp, 25001);
						CancelInvoke("RefreshHostList");
						//Application.LoadLevel("test");
					}
					Upgrading.DrawOutline(new Rect(220, 250, 100, 20), "Connect", text, Color.black);
	            	i++;
	        	}
			}
		}
		else if(menuState == MenuState.Lobby)
		{
			ready = GUI.Toggle(new Rect(10, 150, 20, 20), ready, "", toggle);
			Upgrading.DrawOutline(new Rect(25, 150, 70, 20), "Ready", text, Color.black);
			GetComponent<NetworkView>().RPC ("SyncReady", RPCMode.All, ready);

			Upgrading.DrawOutline(new Rect(10, 210, 100, 100), "Team 1", text, Color.black);
			if(GUI.Button (new Rect(10, 210, 100, 40), "", style))
			{
				team = "1";
				transferScript.team = int.Parse(team);
				GetComponent<NetworkView>().RPC ("UpdateTeam", RPCMode.All, team);
			}
			Upgrading.DrawOutline(new Rect(10, 220, 100, 20), "Switch team", text, Color.black);

			Upgrading.DrawOutline(new Rect(160, 210, 100, 100), "Team 2", text, Color.black);
			if(GUI.Button (new Rect(160, 210, 100, 40), "", style))
			{
				team = "2";
				transferScript.team = int.Parse(team);
				GetComponent<NetworkView>().RPC ("UpdateTeam", RPCMode.All, team);
			}
			Upgrading.DrawOutline(new Rect(160, 220, 100, 20), "Switch team", text, Color.black);

			Upgrading.DrawOutline(new Rect(310, 210, 100, 100), "Team 3", text, Color.black);
			if(GUI.Button (new Rect(310, 210, 100, 40), "", style))
			{
				team = "3";
				transferScript.team = int.Parse(team);
				GetComponent<NetworkView>().RPC ("UpdateTeam", RPCMode.All, team);
			}
			Upgrading.DrawOutline(new Rect(310, 220, 100, 20), "Switch team", text, Color.black);

			Upgrading.DrawOutline(new Rect(460, 210, 100, 100), "Team 4", text, Color.black);
			if(GUI.Button (new Rect(460, 210, 100, 40), "", style))
			{
				team = "4";
				transferScript.team = int.Parse(team);
				GetComponent<NetworkView>().RPC ("UpdateTeam", RPCMode.All, team);
			}
			Upgrading.DrawOutline(new Rect(460, 220, 100, 20), "Switch team", text, Color.black);

			int team1Count = 0;
			int team2Count = 0;
			int team3Count = 0;
			int team4Count = 0;
			bool allReady = true;
			text.alignment = TextAnchor.UpperLeft;
			for(int i = 0; i < playerList.Count; i++)
			{
				if(!playerList[i].ready)
				{
					allReady = false;
				}
				switch(playerList[i].team)
				{
					case "1":
						Upgrading.DrawOutline(new Rect(10, 270 + team1Count * 20, 100, 100), playerList[i].name, text, Color.black);
						team1Count ++;
					break;
					case "2":
						Upgrading.DrawOutline(new Rect(160, 270 + team2Count * 20, 100, 100), playerList[i].name, text, Color.black);
						team2Count ++;
					
					break;
					case "3":
						Upgrading.DrawOutline(new Rect(310, 270 + team3Count * 20, 100, 100), playerList[i].name, text, Color.black);
						team3Count ++;
					
					break;
					case "4":
						Upgrading.DrawOutline(new Rect(460, 270 + team4Count * 20, 100, 100), playerList[i].name, text, Color.black);
						team4Count ++;
					
					break;
				}
			}
			text.alignment = TextAnchor.MiddleCenter;

			if(allReady)
			{
				Upgrading.DrawOutline(new Rect(125, 150, 100, 20), "Everyone ready", text, Color.black);
			}

			if(Network.isServer)
			{
				Upgrading.DrawOutline(new Rect(10, 0, 200, 20), "Amount of rounds", text, Color.black);
				rounds = GUI.TextField(new Rect(10, 30, 200, 20), rounds, textField);
				GetComponent<NetworkView>().RPC ("SyncRounds", RPCMode.All, rounds);
				if(GUI.Button(new Rect(10, 60, 200, 20), "", style) && allReady)
				{
					Debug.Log("Going to try to start the game!");
					GetComponent<NetworkView>().RPC ("StartGame", RPCMode.AllBuffered);
					Invoke ("ServerLoadGame", 1f);
				}
				Upgrading.DrawOutline(new Rect(10, 60, 200, 20), "Start game", text, Color.black);
				//Application.LoadLevel("test");
			}
		}
	}

	void RefreshHostList()
	{
		MasterServer.RequestHostList ("BoA");
		Invoke ("RefreshHostList", 3);
	}

	void OnServerInitialized()
	{
		Debug.Log ("Server started");
		//networkView.RPC ("AddPlayerToServer", RPCMode.Server, playerName, team);
		playerList.Add (new PlayerInfo(playerName, team, Network.player));
		Debug.Log (Network.player);
	}
	void OnConnectedToServer()
	{
		Debug.Log ("I connected!");
		GetComponent<NetworkView>().RPC ("AddPlayerToServer", RPCMode.Server, playerName, team);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		//Called on host
		//Remove player information from playerlist
		GetComponent<NetworkView>().RPC("PlayerLeft", RPCMode.All, player);
	}

	void ServerLoadGame()
	{
		Network.Disconnect();
		Application.LoadLevel(1);
	}

	[RPC]
	void SyncRounds(string rounds)
	{
		transferScript.rounds = int.Parse(rounds);
	}

	[RPC]
	void SyncReady(bool value, NetworkMessageInfo info)
	{
		if(info.sender+""=="-1")
		{
			foreach(PlayerInfo playerInfo in playerList)
			{
				if(playerInfo.networkPlayer == Network.player)
				{
					playerInfo.ready = value;
					break;
				}
			}
		}
		else
		{
			foreach(PlayerInfo playerInfo in playerList)
			{
				if(playerInfo.networkPlayer == info.sender)
				{
					playerInfo.ready = value;
					break;
				}
			}
		}
	}

	[RPC]
	void StartGame()
	{
		Debug.Log ("Time to load the level");
		if (!Network.isServer) 
		{
			Network.Disconnect ();
			Invoke ("LoadGame", 1);
		}
	}

	void LoadGame()
	{
		Application.LoadLevel(1);
	}

	[RPC]
	void UpdateTeam(string team, NetworkMessageInfo info)
	{
		if(info.sender+""=="-1")
		{
			foreach(PlayerInfo playerInfo in playerList)
			{
				if(playerInfo.networkPlayer == Network.player)
				{
					playerInfo.team = team;
					break;
				}
			}
		}
		else
		{
			foreach(PlayerInfo playerInfo in playerList)
			{
				if(playerInfo.networkPlayer == info.sender)
				{
					playerInfo.team = team;
					break;
				}
			}
		}
	}

	[RPC]
	void PlayerLeft(NetworkPlayer player)
	{
		PlayerInfo removePlayer = null;

		foreach(PlayerInfo playerInfo in playerList)
		{
			if(playerInfo.networkPlayer == player)
			{
				removePlayer = playerInfo;
				break;
			}
		}
		if(removePlayer != null)
		{
			playerList.Remove (removePlayer);
		}
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}


	[RPC]
	void AddPlayerToServer(string name, string _team, NetworkMessageInfo info)
	{
		Debug.Log ("Gonna add someone");
		playerList.Add (new PlayerInfo(name, _team, info.sender));
		GetComponent<NetworkView>().RPC ("ClearPlayers", RPCMode.Others);

		foreach(PlayerInfo playerInfo in playerList)
		{
			GetComponent<NetworkView>().RPC ("AddPlayer", RPCMode.Others, playerInfo.name, playerInfo.team, playerInfo.networkPlayer);
		}

	}

	[RPC]
	void AddPlayer(string name, string _team, NetworkPlayer player)
	{
		playerList.Add (new PlayerInfo(name, _team, player));

	}

	[RPC]
	void ClearPlayers()
	{
		playerList.Clear();
	}

	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
        Debug.Log("Could not connect to master server: " + info);
    }
}
