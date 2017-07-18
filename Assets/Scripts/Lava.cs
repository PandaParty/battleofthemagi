using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit2D(Collider2D other)
	{
		/*
		if(other.gameObject.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent("DamageSystem");
			damageSystem.inLava = true;
		}
		*/
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		/*
		if(other.gameObject.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent("DamageSystem");
			damageSystem.inLava = false;
		}
		*/
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent("DamageSystem");
			damageSystem.inLava = true;
		}
	}
}
