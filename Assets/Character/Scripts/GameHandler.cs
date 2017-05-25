using UnityEngine;
using System.Collections;

public class GameHandler : MonoBehaviour {
	public int team1Left;
	public int team2Left;
	public int team3Left;
	public int team4Left;

	public int team1Score;
	public int team2Score;
	public int team3Score;
	public int team4Score;

	public AudioClip winSound;

	public string winnerText = "";

	public int currentRound;
	public int maxRounds;

	public enum State { Game, Upgrade };

	public static State state = State.Game;

	public bool isUpgrading;

	bool received = false;

	float timeCounter = 0;

	public dfLabel damageText;
	public dfLabel timeText;


	// Use this for initialization
	void Start () 
	{
		TransferVariables trScript = (TransferVariables)GameObject.Find ("TransferVariables").GetComponent("TransferVariables");
		maxRounds = trScript.rounds;
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 1;
	}
	
	// Update is called once per frame
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

			damageText.Text = "Damage";
			//GUI.Label(new Rect(10, 300, 100, 100), "Damage done: ");
			int i = 0;
			foreach(GameObject player in players)
			{
				//GUI.Label(new Rect(10, 320 + i * 30, 100, 100), player.GetComponent<SpellCasting>().playerName + ": " + ((int)player.GetComponent<SpellCasting>().damageDone).ToString() +"\n");
				//i++;
				damageText.Text += "\n" + player.GetComponent<SpellCasting>().playerName + " - " + ((int)player.GetComponent<SpellCasting>().damageDone).ToString();
			}
			//GUI.Label(new Rect(950, 10, 200, 100), "Upgrade time: " + ((int)timeCounter).ToString());
			timeText.Text = "Time left for upgrading \n\n" + ((int)timeCounter).ToString();
		}
		if(received)
		{
			//GUI.Label(new Rect(10, 10, 100, 100), "I HAVE RECEIVED THE RPC CALL");
		}
	}

	void NewPlayer(int team)
	{
		Debug.Log ("new player in team: " + team);
		//networkView.RPC("IncreasePlayers", RPCMode.AllBuffered, team);
		IncreasePlayers(team);
	}

	void PlayerDead(int team)
	{
		Debug.Log ("player died in team: " + team);
		received = true;
		if(!Network.isServer)
		{
			networkView.RPC("DecreasePlayers", RPCMode.Server, team);
		}
		else
		{
			DecreasePlayers(team);
		}
		//DecreasePlayers(team);
	}

	[RPC]
	void IncreasePlayers(int team)
	{
		switch(team)
		{
			case 1:
				team1Left ++;
			break;
				
			case 2:
				team2Left ++;
			break;
				
			case 3:
				team3Left ++;
			break;
			
			case 4:
				team4Left ++;
			break;
		}
		Debug.Log ("Increased players in team: " + team);
	}

	[RPC]
	void DecreasePlayers(int team)
	{
		Debug.Log ("A player from team: " + team + " has died!");
		switch(team)
		{
			case 1:
				team1Left --;
			break;
			
			case 2:
				team2Left --;
			break;
			
			case 3:
				team3Left --;
			break;
			
			case 4:
				team4Left --;
			break;
		}
		bool roundOver = false;
		if(team2Left + team3Left + team4Left <= 0)
		{
			team1Score ++;
			networkView.RPC ("WonRound", RPCMode.AllBuffered, 1);
			roundOver = true;
		}
		else if(team1Left + team3Left + team4Left <= 0)
		{
			team2Score ++;
			networkView.RPC ("WonRound", RPCMode.AllBuffered, 2);
			roundOver = true;
		}
		else if(team1Left + team2Left + team4Left <= 0)
		{
			team3Score ++;
			networkView.RPC ("WonRound", RPCMode.AllBuffered, 3);
			roundOver = true;
		}
		else if(team1Left + team2Left + team3Left == 0)
		{
			team4Score ++;
			networkView.RPC ("WonRound", RPCMode.AllBuffered, 4);
			roundOver = true;
		}
		if(roundOver)
		{
			CheckGameOver();
		}
	}

	void CheckGameOver()
	{
		if(currentRound >= maxRounds)
		{
			if(team1Score > team2Score && team1Score > team3Score && team1Score > team4Score)
			{
				//team 1 won
				networkView.RPC ("DisplayWinner", RPCMode.AllBuffered, "Team 1 has won the match!", 1);
				//DisplayWinner ("Team 1 has won the match!");
			}
			if(team2Score > team1Score && team2Score > team3Score && team2Score > team4Score)
			{
				//team 2 won
				networkView.RPC ("DisplayWinner", RPCMode.AllBuffered, "Team 2 has won the match!", 2);
				//DisplayWinner ("Team 2 has won the match!");
			}
			if(team3Score > team2Score && team3Score > team1Score && team3Score > team4Score)
			{
				//team 3 won
				networkView.RPC ("DisplayWinner", RPCMode.AllBuffered, "Team 3 has won the match!", 3);
				//DisplayWinner ("Team 3 has won the match!");
			}
			if(team4Score > team1Score && team4Score > team2Score && team4Score > team3Score)
			{
				//team 4 won
				networkView.RPC ("DisplayWinner", RPCMode.AllBuffered, "Team 4 has won the match!", 4);
				//DisplayWinner ("Team 4 has won the match!");
			}
		}
		else
		{
			if(Network.isServer)
			{
				team1Left = 0;
				team2Left = 0;
				team3Left = 0;
				team4Left = 0;
				Debug.Log ("Sending rpc call to upgrade");
				networkView.RPC ("SyncScore", RPCMode.All, team1Score, team2Score);
				networkView.RPC ("Upgrade", RPCMode.AllBuffered);
				Debug.Log ("RPC call sent");
			}
			/*
			currentRound ++;
			state = State.Upgrade;
			Invoke ("SwapToGame", 30);

			if(team2Left + team3Left + team4Left == 0)
			{
				team1Score ++;
				WonRound(1);
			}
			
			else if(team1Left + team3Left + team4Left == 0)
			{
				team2Score ++;
				WonRound(2);
			}
			
			else if(team1Left + team2Left + team4Left == 0)
			{
				team3Score ++;
				WonRound(3);
			}
			
			else if(team1Left + team2Left + team3Left == 0)
			{
				team4Score ++;
				WonRound(4);
			}
			*/
			//networkView.RPC ("RoundEnd", RPCMode.AllBuffered, currentRound);
		}
	}

	[RPC]
	void Upgrade()
	{
		//received = true;
		currentRound ++;
		state = State.Upgrade;
		isUpgrading = true;
		timeCounter = 60;
		Invoke ("SwapToGame", 60);
	}

	[RPC]
	void WonRound(int team)
	{
		SpellCasting sc = this.gameObject.GetComponent<Upgrading>().spellCasting;
		
		sc.gold += 120;

		if(sc.team == team)
		{
			GA.API.Design.NewEvent("Winrate:" + sc.off1.spellName, 1);
			GA.API.Design.NewEvent("Winrate:" + sc.off2.spellName, 1);
			GA.API.Design.NewEvent("Winrate:" + sc.off3.spellName, 1);
			GA.API.Design.NewEvent("Winrate:" + sc.mob.spellName, 1);
			GA.API.Design.NewEvent("Winrate:" + sc.def.spellName, 1);
			GA.API.Design.NewEvent("Player:" + sc.playerName + ":Winrate", 1);
		}
		else
		{
			GA.API.Design.NewEvent("Winrate:" + sc.off1.spellName, 0);
			GA.API.Design.NewEvent("Winrate:" + sc.off2.spellName, 0);
			GA.API.Design.NewEvent("Winrate:" + sc.off3.spellName, 0);
			GA.API.Design.NewEvent("Winrate:" + sc.mob.spellName, 0);
			GA.API.Design.NewEvent("Winrate:" + sc.def.spellName, 0);
			GA.API.Design.NewEvent("Player:" + sc.playerName + ":Winrate", 0);
		}
	}

	[RPC]
	void SyncScore(int score1, int score2)
	{
		team1Score = score1;
		team2Score = score2;
	}

	void SwapToGame()
	{
		state = State.Game;
		isUpgrading = false;
		gameObject.GetComponent<Upgrading>().spellCasting.gameObject.networkView.RPC ("StartReset", RPCMode.AllBuffered);

		if(!Network.isServer)
		{
			networkView.RPC ("IncreasePlayers", RPCMode.Server, gameObject.GetComponent<Upgrading>().spellCasting.team);
		}
		else
		{
			IncreasePlayers(gameObject.GetComponent<Upgrading>().spellCasting.team);
		}
	}

	[RPC]
	void DisplayWinner(string text, int team)
	{
		AudioSource.PlayClipAtPoint(winSound, transform.position);
		winnerText = text;

	}
}
