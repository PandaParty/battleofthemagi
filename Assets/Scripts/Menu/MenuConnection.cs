using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class MenuConnection : NetworkLobbyManager
{
    List<LobbyPlayer> playerList = new List<LobbyPlayer>();
    public InputField nameInput;
    public Button startGameButton;
    public VerticalLayoutGroup lobbyList;
    public GameObject lobbyItem;

    public GameObject lobbyName;
    public GameObject team1Players;
    public GameObject team2Players;

    public GameObject iceWizard;
    public GameObject fireWizard;

    public GameObject menulobby;
    public GameObject menuMain;
    public GameObject menuclient;
    public GameObject menuhost;
    public GameObject playMenu;

    private string gameName = "3v3 BotM pros only -apem";

    public void Awake()
    {
        matchMaker = gameObject.AddComponent<NetworkMatch>();

        if (PlayerPrefs.HasKey("Player Name"))
        {
            nameInput.text = PlayerPrefs.GetString("Player Name");
        }
        Invoke("RefreshLobbies", 0);
    }

    public void SetPlayerName(string name)
    {
        PlayerPrefs.SetString("Player Name", name);
    }

    public void OnClickStartHost()
    {
        SwapToLobby();
        StartHost();
    }

    public void OnClickStartMM()
    {
        CancelInvoke("RefreshLobbies");
        matchMaker.CreateMatch(gameName, 10, true, "", "", "", 0, 0, OnMatchCreate);
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        this.matchInfo = matchInfo;
        base.OnMatchCreate(success, extendedInfo, matchInfo);
    }

    public void OnClickRefresh()
    {
        matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    public void RefreshLobbies()
    {
        matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
        Invoke("RefreshLobbies", 1);
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        base.OnMatchList(success, extendedInfo, matchList);
        foreach(Transform child in lobbyList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(MatchInfoSnapshot info in matchList)
        {
            Debug.Log(info.name);
            GameObject newItem = Instantiate(lobbyItem, lobbyList.transform);
            newItem.transform.Find("LobbyName").GetComponent<Text>().text = info.name;
            newItem.transform.Find("Slots").GetComponent<Text>().text = info.currentSize + "/" + info.maxSize;
            newItem.GetComponentInChildren<Button>().onClick.AddListener(SwapToLobby);
            newItem.GetComponentInChildren<Button>().onClick.AddListener(delegate { ConnectToMM(info.networkId);});
        }
    }

    void SwapToLobby()
    {
        CancelInvoke("RefreshLobbies");
        playMenu.SetActive(false);
        menulobby.SetActive(true);
        menuclient.SetActive(true);
        menuhost.SetActive(false);
    }

    void ConnectToMM(NetworkID id)
    {
        matchMaker.JoinMatch(id, "", "", "", 0, 0, OnMatchJoined);
    }

    public void SetGameName(string n)
    {
        gameName = n;
    }

    public void OnClickConnect()
    {
        Debug.Log("clicked connect");
        StartClient();
    }

    public void OnClickStartGame()
    {
        base.OnLobbyServerPlayersReady();
    }

    public override void OnLobbyStartHost()
    {
        Debug.Log("Started host");
    }
    
    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        Debug.Log("Someone connected " + conn.address);
    }

    public override void OnLobbyServerPlayersReady()
    {
        startGameButton.interactable = true;
    }

    public void OnClickBack()
    {
        Invoke("RefreshLobbies", 0.5f);
    }

    public void CheckReady()
    {
        bool allready = true;
        foreach(LobbyPlayer player in playerList)
        {
            if(!player.readyToBegin)
            {
                allready = false;
                break;
            }
        }
        if(allready)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }

    public void AddPlayer(LobbyPlayer player)
    {
        if (playerList.Contains(player))
            return;

        playerList.Add(player);
        CheckReady();
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        playerList.Remove(player);
        CheckReady();
        UpdatePlayerList();
    }

    public void UpdatePlayerList()
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
            Destroy(player);
        }

        GameObject newPlayerName;
        foreach (LobbyPlayer player in playerList)
        {
            Debug.Log("Creating player with name: " + player.playerName);
            if (player.team.Equals("1"))
            {
                newPlayerName = Instantiate(lobbyName, team1Players.transform);
            }
            else
            {
                newPlayerName = Instantiate(lobbyName, team2Players.transform);
            }
            newPlayerName.GetComponent<Text>().text = player.playerName;
            newPlayerName.transform.Find("Ready").gameObject.SetActive(player.readyToBegin);
        }
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer player = lobbyPlayer.GetComponent<LobbyPlayer>();
        SpellCasting spellCasting = gamePlayer.GetComponent<SpellCasting>();
        spellCasting.team = int.Parse(player.team);
        Debug.Log("ServerSceneLoadedForPlayer: " + player.playerName);
        if (player.team == "1")
        {
            gamePlayer.transform.position = new Vector3(-11, 0);
        }
        else if (player.team == "2")
        {
            gamePlayer.transform.position = new Vector3(11, 0);
        }

        return true;
    }
}
