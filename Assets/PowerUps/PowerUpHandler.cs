using UnityEngine;
using System.Collections;

public class PowerUpHandler : MonoBehaviour {
	public GameObject damageBoost;
	public GameObject speedBoost;

	GameObject currentPowerUp;

	bool isSpawning = false;


	// Use this for initialization
	void Start () {
		if(Network.isServer)
		{
			Invoke ("SpawnPowerUp", 20);
			isSpawning = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.isServer)
		{
			if(currentPowerUp == null && !isSpawning && GameHandler.state == GameHandler.State.Game)
			{
				Invoke ("SpawnPowerUp", 30);
				isSpawning = true;
			}
		}
	}

	void SpawnPowerUp()
	{
		int randomNr = Random.Range(1, 3);
		switch(randomNr)
		{
			case 1:
				currentPowerUp = (GameObject)Network.Instantiate(damageBoost, transform.position, Quaternion.identity, 0);
			break;
			case 2:
				currentPowerUp = (GameObject)Network.Instantiate(speedBoost, transform.position, Quaternion.identity, 0);
			break;
		}
		isSpawning = false;
	}
}
