using UnityEngine;
using System.Collections;

public class ConnectionHandler_2 : MonoBehaviour {
	public GameObject player;
	public GameObject frostiez;
	public GameObject purpleMage;
	public GameObject ogremagi;
	int team;
	string name;
	public GameObject gameHandler;

	public static ArrayList players = new ArrayList();

	void Awake()
	{
	}
	// Use this for initialization
	void Start () {
		TransferVariables trScript = (TransferVariables)GameObject.Find ("TransferVariables").GetComponent("TransferVariables");
		team = trScript.team;
		name = PlayerPrefs.GetString("Player Name");
		Debug.Log (trScript.name);
		if(trScript.isHost){
			Debug.Log (Network.HavePublicAddress());
			Debug.Log ("I am host");
			Network.InitializeServer(32, 25001, true);
			MasterServer.RegisterHost("AntonsExample", "AntonsExample");
		}
		else
		{
			Debug.Log ("I am not host");
			Network.Connect(trScript.connectIp, 25001);
			Debug.Log ("I am connecting!");
		}
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
	
	void OnConnectedToServer()
	{
		Debug.Log ("I connected!");
		Vector3 spawnPos = Vector3.zero;
		switch(team)
		{
			case 1: spawnPos = new Vector3(-9, 0, 0);
			break;
			case 2: spawnPos = new Vector3(9, 0, 0);
			break;
			case 3: spawnPos = new Vector3(0, -9, 0);
			break;
			case 4: spawnPos = new Vector3(0, 9, 0);
			break;
		}
		GameObject newPlayer;
		if(team == 2)
		{
			//newPlayer = (GameObject)Network.Instantiate(frostiez, spawnPos, transform.rotation, 0);
			//newPlayer = (GameObject)GameObject.Instantiate(frostiez, spawnPos, transform.rotation);
		}
		else if(team == 1)
		{
			//newPlayer = (GameObject)Network.Instantiate(player, spawnPos, transform.rotation, 0);
			//newPlayer = (GameObject)GameObject.Instantiate(player, spawnPos, transform.rotation);
		}
		else
		{
			//newPlayer = (GameObject)Network.Instantiate(purpleMage, spawnPos, transform.rotation, 0);
			//newPlayer = (GameObject)GameObject.Instantiate(purpleMage, spawnPos, transform.rotation);
		}
		GetComponent<NetworkView>().RPC ("CreatePlayer", RPCMode.AllBuffered, team, Network.AllocateViewID(), name);
		/*
		GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");
		CameraScript camScript = (CameraScript)camera.GetComponent("CameraScript");
		newPlayer.SendMessage ("SetTeam", team);
		gameHandler.SendMessage ("NewPlayer", team);
		camScript.PlayerObject = newPlayer;
		GlobalConstants.isFrozen = true;
		*/
	}
	
	void OnServerInitialized()
	{
		Vector3 spawnPos = Vector3.zero;
		switch(team)
		{
		case 1: spawnPos = new Vector3(-9, 0, 0);
			break;
		case 2: spawnPos = new Vector3(9, 0, 0);
			break;
		case 3: spawnPos = new Vector3(0, -9, 0);
			break;
		case 4: spawnPos = new Vector3(0, 9, 0);
			break;
		}
		GameObject newPlayer;

		if(team == 2)
		{
			//newPlayer = (GameObject)Network.Instantiate(frostiez, spawnPos, transform.rotation, 0);
			//newPlayer = (GameObject)GameObject.Instantiate(frostiez, spawnPos, transform.rotation);
		}
		else if(team == 1)
		{
			//newPlayer = (GameObject)Network.Instantiate(player, spawnPos, transform.rotation, 0);
			//newPlayer = (GameObject)GameObject.Instantiate(player, spawnPos, transform.rotation);
		}
		else
		{
			//newPlayer = (GameObject)Network.Instantiate(purpleMage, spawnPos, transform.rotation, 0);
			//newPlayer = (GameObject)GameObject.Instantiate(purpleMage, spawnPos, transform.rotation);
		}
		GetComponent<NetworkView>().RPC ("CreatePlayer", RPCMode.AllBuffered, team, Network.AllocateViewID(), name);

		Invoke ("Unfreeze", 5.0f);
	}

	void Unfreeze()
	{
		GetComponent<NetworkView>().RPC ("UnfreezeAll", RPCMode.All);
	}

	[RPC]
	void UnfreezeAll()
	{
		GlobalConstants.isFrozen = false;
	}

	void OnPlayerConnected()
	{
		Debug.Log ("Someone connected");
	}

	[RPC]
	void CreatePlayer(int team, NetworkViewID viewId, string name)
	{
		Vector3 spawnPos = Vector3.zero;
		switch(team)
		{
		case 1: spawnPos = new Vector3(-9, 0, 0);
			break;
		case 2: spawnPos = new Vector3(9, 0, 0);
			break;
		case 3: spawnPos = new Vector3(0, -9, 0);
			break;
		case 4: spawnPos = new Vector3(0, 9, 0);
			break;
		}
		GameObject newPlayer;
		if(name == "0GRE MAG1")
		{
			newPlayer = (GameObject)GameObject.Instantiate(ogremagi, spawnPos, transform.rotation);
		}
		else if(team == 2)
		{
			newPlayer = (GameObject)GameObject.Instantiate(frostiez, spawnPos, transform.rotation);
		}
		else if(team == 1)
		{
			newPlayer = (GameObject)GameObject.Instantiate(player, spawnPos, transform.rotation);
		}
		else
		{
			newPlayer = (GameObject)GameObject.Instantiate(purpleMage, spawnPos, transform.rotation);
		}
		newPlayer.GetComponent<NetworkView>().viewID = viewId;
		gameHandler.SendMessage ("NewPlayer", team);
		if(newPlayer.GetComponent<NetworkView>().isMine)
		{
			GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");
			CameraScript camScript = (CameraScript)camera.GetComponent("CameraScript");
			newPlayer.SendMessage ("SetTeam", team);
			camScript.PlayerObject = newPlayer;
			GlobalConstants.isFrozen = true;
		}
		players.Add(newPlayer);
		Debug.Log ("Adding someone to playerus");
	}
}
