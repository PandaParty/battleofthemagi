using UnityEngine;
using System.Collections;

public class NoLava : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent("DamageSystem");
			damageSystem.inLava = false;
		}
	}
}
