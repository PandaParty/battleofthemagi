using UnityEngine;
using System.Collections;

public class DisasterFireball : MonoBehaviour {
	public GameObject fireball;
	// Use this for initialization
	void Start () {
		if(Network.isServer)
		{
			Vector3 spawnPoint;
			for(int i = 1; i < 21; i++)
			{
				spawnPoint = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (360/20) * i) * 20, Mathf.Sin (Mathf.Deg2Rad * 360/20 * i) * 20, 0);
				GameObject newFire = (GameObject)Network.Instantiate(fireball, spawnPoint, Quaternion.identity, 0);
				newFire.GetComponent<Spell>().aimPoint = Vector3.zero;
				newFire.GetComponent<Spell>().team = i * 111;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
