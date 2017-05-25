using UnityEngine;
using System.Collections;

public class ConnectionScript : MonoBehaviour {
	string playerName = "Player";
	string connectIp = "127.0.0.1";
	string team = "1";
	TransferVariables transferScript;
	
	void Start () {
		transferScript = (TransferVariables)GameObject.Find ("TransferVariables").GetComponent("TransferVariables");
	    if(PlayerPrefs.HasKey("Player Name")){
			playerName = PlayerPrefs.GetString("Player Name");
		}
		if(PlayerPrefs.HasKey("Team")){
			team = PlayerPrefs.GetString("Team");
		}
	}
	
	void OnGUI(){
	    playerName = GUI.TextField(new Rect(10, 10, 200, 20), playerName);
		PlayerPrefs.SetString("Player Name", playerName);

		//GUI.Label(new Rect(10, 70, 200, 20), "Team");
		//connectIp = GUI.TextField(new Rect(10, 100, 200, 20), connectIp);
		GUI.Label(new Rect(10, 130, 200, 20), "Team");
		team = GUI.TextField(new Rect(10, 160, 200, 20), team);
		PlayerPrefs.SetString ("Team", team);
		if(GUI.Button (new Rect(10, 190, 200, 20), "Start Server")){
			transferScript.isHost = true;
			transferScript.team = int.Parse(team);
			Application.LoadLevel("test");
		}
		
		if(GUI.Button (new Rect(10, 220, 200, 20), "Refresh")){
			MasterServer.RequestHostList("AntonsExample");

		}
		
		if (MasterServer.PollHostList().Length != 0) {
        	HostData[] hostData = MasterServer.PollHostList();
        	int i = 0;
        	while (i < hostData.Length) {
           	    GUI.Label(new Rect(10, 250, 200, 100), "Game name: " + hostData[i].gameName);
				if(GUI.Button (new Rect(220, 250, 100, 20), "Connect")){
					string tmpIp = "";
           	 		int j = 0;
            		while (j < hostData[i].ip.Length) {
                		tmpIp = hostData[i].ip[j] + " ";
                		j++;
            		}
					transferScript.connectIp = tmpIp;
					transferScript.team = int.Parse(team);
					Application.LoadLevel("test");
				}
            	i++;
        	}
		}
	}
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
        Debug.Log("Could not connect to master server: " + info);
    }
}
