using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MenuConnection : NetworkLobbyManager
{
    List<LobbyPlayer> playerList = new List<LobbyPlayer>();
    public InputField nameInput;
    public Button startGameButton;

    public GameObject lobbyName;
    public GameObject team1Players;
    public GameObject team2Players;

    public GameObject iceWizard;
    public GameObject fireWizard;

    public void Awake()
    {
        if (PlayerPrefs.HasKey("Player Name"))
        {
            nameInput.text = PlayerPrefs.GetString("Player Name");
        }
    }

    public void SetPlayerName(string name)
    {
        PlayerPrefs.SetString("Player Name", name);
    }

    public void OnClickStartHost()
    {
        StartHost();
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
        Debug.Log("Someone connected");
    }

    public override void OnLobbyServerPlayersReady()
    {
        startGameButton.interactable = true;
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
