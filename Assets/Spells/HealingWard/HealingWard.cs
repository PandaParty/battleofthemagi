﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HealingWard : NetworkBehaviour {
	public Spell spell;
	public float duration;
	public float bloomTime;
	
	public AudioClip cast;

	public GameObject effect;

	bool dispels = false;
	bool dispelHeals = false;
	float damages = 0;
	bool lifeSteals = false;
	bool damageReduct = false;
	bool instant = false;

	// Use this for initialization
	void Start () {
		SetColor();
		transform.position = spell.aimPoint;
		transform.position += new Vector3(0, 0, 1);
		AudioSource.PlayClipAtPoint(cast, transform.position);
		Invoke ("Bloom", bloomTime);
		//if(GetComponent<NetworkView>().isMine)
		//{
		//	Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
		//	if(upgrading.healingDispel.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("Dispel", RPCMode.All);
				
		//		if(upgrading.healingDispelHeal.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("DispelHeal", RPCMode.All);
		//		}
		//	}

		//	if(upgrading.healingDuration.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("Duration", RPCMode.All, upgrading.healingDuration.currentLevel);
				
		//		if(upgrading.healingDamageReduct.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("DamageReduct", RPCMode.All);
		//		}
		//	}
			
		//	if(upgrading.healingDmg.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("Damages", RPCMode.All, upgrading.healingDmg.currentLevel);

		//		if(upgrading.healingLifesteal.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("Lifesteals", RPCMode.All);
		//		}
		//	}

		//	if(upgrading.healingBloom.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("BloomTime", RPCMode.All, upgrading.healingBloom.currentLevel);
				
		//		if(upgrading.healingBurst.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("Instant", RPCMode.All);
		//		}
		//	}
		//}
	}

	void SetColor()
	{
		switch(spell.team)
		{
			case 2:
				gameObject.GetComponent<ParticleSystem>().startColor = new Color(0f, 1f, 1f);
				ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem system in systems)
				{
					if(system.gameObject.name == "CFX_CCW")
					{
						system.startColor = new Color(0.27f, 1f, 1f);
					}
				}
			break;
		}
	}

	[RPC]
	void Duration(int level)
	{
		duration += 0.7f * level;
	}

	[RPC]
	void DamageReduct()
	{
		damageReduct = true;
	}

	[RPC]
	void BloomTime(int level)
	{
		bloomTime -= 0.2f * level;
		gameObject.GetComponent<ParticleSystem>().time = 0.7f - bloomTime;
	}

	[RPC]
	void Instant()
	{
		instant = true;
	}

	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void Bloom()
	{
        if (!isServer)
            return;

		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, ((CircleCollider2D)GetComponent<Collider2D>()).radius);

		foreach(Collider2D hitCollider in hitColliders)
		{
			if(hitCollider.CompareTag("Player"))
			{
				if(hitCollider.gameObject.GetComponent<SpellCasting>().team == spell.team)
				{
					if(!instant)
					{
						hitCollider.GetComponent<DamageSystem>().AddHot (spell.damage, duration, 0.0165f, effect);
						if(damageReduct)
						{
							hitCollider.GetComponent<DamageSystem>().Amplify(-0.5f, duration);
						}
					}
					else
					{
						hitCollider.GetComponent<DamageSystem>().Damage(-32f, 0, transform.position, spell.owner);
					}
				}
			}
		}
		spell.Invoke("KillSelf", bloomTime + 0.4f);
	}

	void OnTriggerStay2D(Collider2D other)
	{
		/*
		if(other.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
			if(spell.team == damageSystem.Team())
			{
				if(other.networkView.isMine)
				{
					damageSystem.Damage(-spell.damage, spell.knockFactor, transform.position);
				}
			}
		}
		*/
	}
}