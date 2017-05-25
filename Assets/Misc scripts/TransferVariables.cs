using UnityEngine;
using System.Collections;

public class TransferVariables : MonoBehaviour {
	public bool isHost = false;
	public string connectIp = "";
	public int team;
	public int rounds;
	void Start(){
		DontDestroyOnLoad(gameObject);
	}
}
