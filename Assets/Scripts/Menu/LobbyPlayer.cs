using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar(hook = "OnMyName")]
    public string playerName;
    [SyncVar(hook = "OnMyTeam")]
    public string team;

    [SyncVar]
    public int rounds;

    MenuConnection menuLobby;

    public void Awake()
    {
        menuLobby = GameObject.Find("LobbyManager").GetComponent<MenuConnection>();
    }

    public override void OnClientEnterLobby()
    {
        Debug.Log("Client entered lobby");
        menuLobby.AddPlayer(this);
        base.OnClientEnterLobby();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        SetupLocalPlayer();
    }

    public override void OnClientReady(bool readyState)
    {
        menuLobby.CheckReady();
        menuLobby.UpdatePlayerList();
        Debug.Log("A client is ready: " + readyState);
        base.OnClientReady(readyState);
    }

    [Command]
    void CmdSetName(string name)
    {
        playerName = name;
        Debug.Log("Command set name:" + name);
    }

    [Command]
    void CmdSetTeam(string newTeam)
    {
        team = newTeam;
        Debug.Log("Command set team:" + team);
    }

    void OnMyTeam(string newTeam)
    {
        team = newTeam;
        menuLobby.UpdatePlayerList();
        Debug.Log("On my team: "+ team);
    }

    void OnMyName(string name)
    {
        playerName = name;
        Debug.Log("On my name: " + playerName);
        menuLobby.UpdatePlayerList();
    }

    void SetupLocalPlayer()
    {
        Debug.Log("Setting up local player");
        Toggle toggleButton = GameObject.Find("Toggle").GetComponent<Toggle>();
        toggleButton.onValueChanged.AddListener(OnReadyClicked);
        CmdSetName(PlayerPrefs.GetString("Player Name"));
        CmdSetTeam("1");
        Button team1Button = GameObject.Find("Team 1").GetComponent<Button>();
        team1Button.onClick.AddListener(OnClickTeam1);
        Button team2Button = GameObject.Find("Team 2").GetComponent<Button>();
        team2Button.onClick.AddListener(OnClickTeam2);
        CmdUpdatePlayerList();
    }
    
    [Command]
    public void CmdUpdatePlayerList()
    {
        //UpdatePlayerList();
        RpcUpdatePlayerList();
    }

    [ClientRpc]
    public void RpcUpdatePlayerList()
    {
        menuLobby.UpdatePlayerList();
    }

    void OnClickTeam1()
    {
        CmdSetTeam("1");
    }

    void OnClickTeam2()
    {
        CmdSetTeam("2");
    }

    void OnReadyClicked(bool value)
    {
        if (value)
        {
            SendReadyToBeginMessage();
        }
        else
        {
            SendNotReadyToBeginMessage();
        }
    }

    public void OnDestroy()
    {
        menuLobby.RemovePlayer(this);
    }
}
