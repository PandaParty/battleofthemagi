using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerUpHandler : NetworkBehaviour
{
	public GameObject damageBoost;
	public GameObject speedBoost;

	GameObject currentPowerUp;

	bool isSpawning = false;


	// Use this for initialization
	void Start ()
    {
        if(isServer)
        {
			Invoke ("SpawnPowerUp", 20);
			isSpawning = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(isServer)
        {
            if(GameHandler.state == GameHandler.State.Game)
            {
                if (currentPowerUp == null && !isSpawning)
                {
                    StartSpawn();
                }
            }
            else
            {
                if(currentPowerUp != null)
                {
                    Destroy(currentPowerUp);
                }
                else if(isSpawning)
                {
                    CancelSpawn();
                }
            }
		}
	}

    public void CancelSpawn()
    {
        CancelInvoke("SpawnPowerUp");
        isSpawning = false;
    }

    public void StartSpawn()
    {
        Invoke("SpawnPowerUp", 30);
        isSpawning = true;
    }

	void SpawnPowerUp()
	{
		int randomNr = Random.Range(1, 3);
		switch(randomNr)
		{
			case 1:
				currentPowerUp = Instantiate(damageBoost, transform.position, Quaternion.identity);
                NetworkServer.Spawn(currentPowerUp);

            break;
			case 2:
				currentPowerUp = Instantiate(speedBoost, transform.position, Quaternion.identity);
                NetworkServer.Spawn(currentPowerUp);
			break;
		}
		isSpawning = false;
	}
}
