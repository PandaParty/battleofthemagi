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
    public InputField clientRoundField;
    public GameObject openLobbiesPanel;
    public GameObject openLobby;
    public GameObject uiLobby;
    public GameObject uiClient;
    public GameObject uiMainMenu;
    public GameObject lobbyName;
    public GameObject team1Players;
    public GameObject team2Players;
    public Button startGameButton;

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

    public void CloseServer()
    {
        if(transferScript.isHost)
        {
            transferScript.isHost = false;
            MasterServer.UnregisterHost();
        }
        team = "1";
        ready = false;
        rounds = "3";
        playerList.Clear();
        Network.Disconnect();
    }

    public void SetRounds(string amount)
    {
        rounds = amount;
        GetComponent<NetworkView>().RPC("SyncRounds", RPCMode.All, rounds);
    }

    public void StartGameServer()
    {
        bool allReady = true;
        GetComponent<NetworkView>().RPC("SyncRounds", RPCMode.All, rounds);
        if (allReady)
        {
            Debug.Log("Going to try to start the game!");
            GetComponent<NetworkView>().RPC("StartGame", RPCMode.AllBuffered);
            Invoke("ServerLoadGame", 1f);
        }
    }

    public void Refresh()
    {
        Debug.Log("Refreshing");

        MasterServer.ClearHostList();
        MasterServer.RequestHostList("BotM" + Application.version);
        Invoke("UpdateLobbyList", 1);
    }

    void UpdateLobbyList()
    {
        Debug.Log("Updating lobby list");
        List<GameObject> openLobbies = new List<GameObject>();
        for (int i = 0; i < openLobbiesPanel.transform.childCount; i++)
        {
            openLobbies.Add(openLobbiesPanel.transform.GetChild(i).gameObject);
        }
        foreach (GameObject lobby in openLobbies)
        {
            GameObject.Destroy(lobby);
        }
        HostData[] hostData = MasterServer.PollHostList();
        GameObject newLobby;
        for (int i = 0; i < hostData.Length; i++)
        {
            Debug.Log("Creating lobby with guid: " + hostData[i].guid);
            newLobby = GameObject.Instantiate(openLobby, openLobbiesPanel.transform);
            newLobby.GetComponent<LobbyInfo>().hostData = hostData[i];
            newLobby.GetComponent<Text>().text = hostData[i].gameName;
            HostData hd = hostData[i];
            newLobby.GetComponentInChildren<Button>().onClick.AddListener(() => { ConnectToServer(hd); });
            newLobby.GetComponentInChildren<Button>().onClick.AddListener(() => { uiMainMenu.SetActive(false); uiClient.SetActive(true); uiLobby.SetActive(true); });
        }
    }

    void UpdatePlayerNames()
    {
        Debug.Log("Updating player list");
        List<GameObject> playerNames = new List<GameObject>();
        for (int i = 0; i < team1Players.transform.childCount; i++)
        {
            playerNames.Add(team1Players.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < team2Players.transform.childCount; i++)
        {
            playerNames.Add(team2Players.transform.GetChild(i).gameObject);
        }
        foreach (GameObject player in playerNames)
        {
            GameObject.Destroy(player);
        }
        
        GameObject newPlayerName;
        foreach (PlayerInfo player in playerList)
        {
            Debug.Log("Creating player with name: " + player.name);
            if(player.team.Equals("1"))
            {
                newPlayerName = GameObject.Instantiate(lobbyName, team1Players.transform);
            }
            else
            {
                newPlayerName = GameObject.Instantiate(lobbyName, team2Players.transform);
            }
            newPlayerName.GetComponent<Text>().text = player.name;
            newPlayerName.transform.Find("Ready").gameObject.SetActive(player.ready);
        }
    }

    void ConnectToServer(HostData hostData)
    {
        Debug.Log("Trying to connect to server: " + hostData.guid);
        string tmpIp = "";
        for (int j = 0; j < hostData.ip.Length; j++)
        {
            tmpIp = hostData.ip[j] + " ";
            j++;
        }
        transferScript.connectIp = tmpIp;
        Network.Connect(tmpIp, 25001);
        CancelInvoke("RefreshHostList");
        //spellSelections.SetActive(true);
    }

    public void SwitchTeam(string newTeam)
    {
        team = newTeam;
        transferScript.team = int.Parse(team);
        GetComponent<NetworkView>().RPC ("UpdateTeam", RPCMode.All, team);
    }

    public void SetReady(bool value)
    {
        ready = value;
        Debug.Log(ready);
        GetComponent<NetworkView>().RPC("SyncReady", RPCMode.All, ready);
    }

    void OnGUI(){
		if(menuState == MenuState.Lobby)
		{
			//int team1Count = 0;
			//int team2Count = 0;
			//int team3Count = 0;
			//int team4Count = 0;
			//bool allReady = true;
			//text.alignment = TextAnchor.UpperLeft;
			//for(int i = 0; i < playerList.Count; i++)
			//{
			//	if(!playerList[i].ready)
			//	{
			//		allReady = false;
			//	}
			//	switch(playerList[i].team)
			//	{
			//		case "1":
			//			Upgrading.DrawOutline(new Rect(10, 270 + team1Count * 20, 100, 100), playerList[i].name, text, Color.black);
			//			team1Count ++;
			//		break;
			//		case "2":
			//			Upgrading.DrawOutline(new Rect(160, 270 + team2Count * 20, 100, 100), playerList[i].name, text, Color.black);
			//			team2Count ++;
					
			//		break;
			//		case "3":
			//			Upgrading.DrawOutline(new Rect(310, 270 + team3Count * 20, 100, 100), playerList[i].name, text, Color.black);
			//			team3Count ++;
					
			//		break;
			//		case "4":
			//			Upgrading.DrawOutline(new Rect(460, 270 + team4Count * 20, 100, 100), playerList[i].name, text, Color.black);
			//			team4Count ++;
					
			//		break;
			//	}
			//}
			//text.alignment = TextAnchor.MiddleCenter;

			//if(allReady)
			//{
			//	Upgrading.DrawOutline(new Rect(125, 150, 100, 20), "Everyone ready", text, Color.black);
			//}

			//if(Network.isServer)
			//{
			//	Upgrading.DrawOutline(new Rect(10, 0, 200, 20), "Amount of rounds", text, Color.black);
			//	rounds = GUI.TextField(new Rect(10, 30, 200, 20), rounds, textField);
			//	GetComponent<NetworkView>().RPC ("SyncRounds", RPCMode.All, rounds);
			//	if(GUI.Button(new Rect(10, 60, 200, 20), "", style) && allReady)
			//	{
			//		Debug.Log("Going to try to start the game!");
			//		GetComponent<NetworkView>().RPC ("StartGame", RPCMode.AllBuffered);
			//		Invoke ("ServerLoadGame", 1f);
			//	}
			//	Upgrading.DrawOutline(new Rect(10, 60, 200, 20), "Start game", text, Color.black);
			//	//Application.LoadLevel("test");
			//}
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
        UpdatePlayerNames();
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
        clientRoundField.text = rounds;
		transferScript.rounds = int.Parse(rounds);
	}

	[RPC]
	void SyncReady(bool value, NetworkMessageInfo info)
    {
        
        if (info.sender+""=="-1")
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
        bool allReady = true;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].ready)
            {
                allReady = false;
            }
        }
        startGameButton.interactable = allReady;
        UpdatePlayerNames();
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
				if(playerInfo.networkPlayer.Equals(Network.player))
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
				if(playerInfo.networkPlayer.Equals(info.sender))
				{
					playerInfo.team = team;
					break;
				}
			}
		}
        UpdatePlayerNames();
    }

	[RPC]
	void PlayerLeft(NetworkPlayer player)
	{
		PlayerInfo removePlayer = null;

		foreach(PlayerInfo playerInfo in playerList)
		{
			if(playerInfo.networkPlayer.Equals(player))
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

        UpdatePlayerNames();
    }


	[RPC]
	void AddPlayerToServer(string name, string _team, NetworkMessageInfo info)
	{
		Debug.Log ("Gonna add someone");
		playerList.Add (new PlayerInfo(name, _team, info.sender));
		GetComponent<NetworkView>().RPC ("ClearPlayers", RPCMode.Others);
        UpdatePlayerNames();
		foreach(PlayerInfo playerInfo in playerList)
		{
			GetComponent<NetworkView>().RPC ("AddPlayer", RPCMode.Others, playerInfo.name, playerInfo.team, playerInfo.networkPlayer);
		}

	}

	[RPC]
	void AddPlayer(string name, string _team, NetworkPlayer player)
	{
		playerList.Add (new PlayerInfo(name, _team, player));
        UpdatePlayerNames();

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
