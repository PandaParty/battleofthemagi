using UnityEngine;
using System.Collections;

public class DisasterHandler : MonoBehaviour {
	public GameObject disasterFireball;

	public AudioClip fireDisSound;
	// Use this for initialization
	void Start () {
		if(Network.isServer)
		{
			InvokeRepeating("DisasterTime", 75, 60);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void DisasterTime()
	{
		if(GameHandler.state == GameHandler.State.Game)
		{
			int randomDis = Random.Range(1, 2);

			switch(randomDis)
			{
				case 1:
					Invoke ("FireDisaster", 5);
					GetComponent<NetworkView>().RPC ("FireDisSound", RPCMode.All);
				break;
			}
		}
	}

	void FireDisaster()
	{
		GameObject.Instantiate(disasterFireball);
	}

	[RPC]
	void FireDisSound()
	{
		AudioSource.PlayClipAtPoint(fireDisSound, Vector3.zero);
	}
}
