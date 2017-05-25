﻿using UnityEngine;
using System.Collections;

public class DamageBoost : MonoBehaviour {
	bool someoneCapping = false;

	public GameObject effect;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			if(other.networkView.isMine)
			{
				other.GetComponent<SpellCasting>().StartChannelingPowerUp(gameObject, 4);
			}
		}
		else if(other.CompareTag("Spell"))
		{
			if(other.name == "WindWalkShield(Clone)" || other.name == "NewShield(Clone)")
			{
				Network.Destroy(other.gameObject);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			if(other.networkView.isMine)
			{
				other.SendMessage("EndChannelingPowerUp");
			}
		}
	}

	void Capped(GameObject player)
	{
		player.GetComponent<DamageSystem>().Damage(-15, 0, transform.position, "world");
		player.GetComponent<SpellCasting>().DamageBoost(1.5f, 10f);
		networkView.RPC ("CreateEffect", RPCMode.All, player.GetComponent<SpellCasting>().playerName, 10.0f);
		Network.Destroy(gameObject);
	}


	[RPC]
	void CreateEffect(string playerName, float duration)
	{
		GameObject powerUpEffect = (GameObject)GameObject.Instantiate(effect);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			if(player.GetComponent<SpellCasting>().playerName == playerName)
			{
				powerUpEffect.GetComponent<FollowPlayer>().SetFollow(player, 8f);
				break;
			}
		}
	}
}
