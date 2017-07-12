using UnityEngine;
using System.Collections;

public class TransferVariables : MonoBehaviour {
	public int rounds = 3;
	void Start()
    {
		DontDestroyOnLoad(gameObject);
	}

    public void SetRounds(string input)
    {
        rounds = int.Parse(input);
    }
}
