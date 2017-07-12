using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameHandler : NetworkBehaviour
{
    [SyncVar]
	public int team1Left;
    [SyncVar]
    public int team2Left;

	public int team1Score;
	public int team2Score;
    public Text team1ScoreText;
    public Text team2ScoreText;

    public AudioClip winSound;

	public string winnerText = "";

	public int currentRound;

    [SyncVar]
	public int maxRounds;

	public enum State { Game, Upgrade };

	public static State state = State.Game;

	public bool isUpgrading;
    
	float timeCounter = 0;
    
	void Start ()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;

        if (!isServer)
            return;

		TransferVariables trScript = (TransferVariables)GameObject.Find ("TransferVariables").GetComponent("TransferVariables");
		maxRounds = trScript.rounds;
        Debug.Log(maxRounds);
	}
	
	void Update () 
	{
		if(timeCounter > 0)
		{
			timeCounter -= Time.deltaTime;
		}
	}

	void OnGUI()
	{
		//GUI.Label(new Rect(600, 10, 300, 100), "Team 1: " + team1Score + "\t\tTeam 2: " + team2Score);
		if(state == State.Upgrade)
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

			//damageText.Text = "Damage";
			GUI.Label(new Rect(10, 300, 100, 100), "Damage done: ");
			int i = 0;
			foreach(GameObject player in players)
			{
				GUI.Label(new Rect(10, 320 + i * 30, 100, 100), player.GetComponent<SpellCasting>().playerName + ": " + ((int)player.GetComponent<SpellCasting>().damageDone).ToString() +"\n");
				//i++;
				//damageText.Text += "\n" + player.GetComponent<SpellCasting>().playerName + " - " + ((int)player.GetComponent<SpellCasting>().damageDone).ToString();
			}
		    GUI.Label(new Rect(950, 10, 200, 100), "Time left for upgrading: " + ((int)timeCounter).ToString());
			//timeText.Text = "Time left for upgrading \n\n" + ((int)timeCounter).ToString();
		}
	}

    [Command]
	public void CmdNewPlayer(int team)
	{
		Debug.Log ("new player in team: " + team);
        switch (team)
        {
            case 1:
                team1Left++;
                break;
            case 2:
                team2Left++;
                break;
        }
    }

	public void PlayerDead(int team)
	{
		Debug.Log ("player died in team: " + team);
        switch (team)
        {
            case 1:
                team1Left--;
                break;

            case 2:
                team2Left--;
                break;
        }
        bool roundOver = false;
        if (team2Left <= 0)
        {
            team1Score++;
            roundOver = true;
        }
        else if (team1Left <= 0)
        {
            team2Score++;
            roundOver = true;
        }

        if (roundOver)
        {
            RpcSyncScore(team1Score, team2Score);
            RpcIncreaseGold();
            CheckGameOver();
        }
    }
    
	void CheckGameOver()
	{
		if(currentRound >= maxRounds)
		{
			if(team1Score > team2Score)
			{
                //team 1 won
                Debug.Log("Team 1 won");
				//GetComponent<NetworkView>().RPC ("DisplayWinner", RPCMode.AllBuffered, "Team 1 has won the match!", 1);
				//DisplayWinner ("Team 1 has won the match!");
			}
			if(team2Score > team1Score)
			{
                //team 2 won
                Debug.Log("Team 2 won");
                //GetComponent<NetworkView>().RPC ("DisplayWinner", RPCMode.AllBuffered, "Team 2 has won the match!", 2);
				//DisplayWinner ("Team 2 has won the match!");
			}
		}
		else
		{
			team1Left = 0;
			team2Left = 0;
			Debug.Log ("Sending rpc call to upgrade");
            //GetComponent<NetworkView>().RPC ("Upgrade", RPCMode.AllBuffered);
            RpcUpgrade();
			Debug.Log ("RPC call sent");
		}
	}

    [ClientRpc]
    void RpcIncreaseGold()
    {
        SpellCasting sc = this.gameObject.GetComponent<Upgrading>().spellCasting;

        sc.gold += 160;
    }

	[ClientRpc]
	void RpcUpgrade()
	{
		currentRound ++;
		state = State.Upgrade;
		isUpgrading = true;
		timeCounter = 60;
		Invoke ("SwapToGame", 60);
	}

	[ClientRpc]
	void RpcSyncScore(int score1, int score2)
	{
		team1Score = score1;
		team2Score = score2;
        team1ScoreText.text = team1Score.ToString();
        team2ScoreText.text = team2Score.ToString();
    }

	void SwapToGame()
	{
		state = State.Game;
		isUpgrading = false;

        //gameObject.GetComponent<Upgrading>().spellCasting.gameObject.GetComponent<NetworkView>().RPC ("StartReset", RPCMode.AllBuffered);

        gameObject.GetComponent<Upgrading>().spellCasting.gameObject.GetComponent<DamageSystem>().CmdFullReset();
        CmdNewPlayer(gameObject.GetComponent<Upgrading>().spellCasting.team);
	}

	[RPC]
	void DisplayWinner(string text, int team)
	{
		AudioSource.PlayClipAtPoint(winSound, transform.position);
		winnerText = text;

	}
}
