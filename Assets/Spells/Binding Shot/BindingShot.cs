﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BindingShot : NetworkBehaviour 
{
	public float speed = 50;
	private float oldSpeed;
	public Spell spell;
	public GameObject bindingShotHit;
	public AudioClip cast;
	public AudioClip bind;

	public float duration;
	public float length;

	public GameObject bindRope;

	bool silences;
	bool amplifies;

	// Use this for initialization
	void Start () {
		oldSpeed = speed;
		spell.SetColor();
		Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
		spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
		spell.Invoke ("KillSelf", 5);
		AudioSource.PlayClipAtPoint(cast, transform.position);

		//if(GetComponent<NetworkView>().isMine)
		//{
		//	Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
		//	if(upgrading.bindingDuration.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("IncreaseDur", RPCMode.All, upgrading.bindingDuration.currentLevel);
				
		//		if(upgrading.bindingSilence.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("ActivateSilence", RPCMode.All);
		//		}
		//	}
		//	if(upgrading.bindingLength.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("DecreaseLength", RPCMode.All, upgrading.bindingLength.currentLevel);
				
		//		if(upgrading.bindingAmplify.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("ActivateAmplify", RPCMode.All);
		//		}
		//	}
		//}
	}

	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
	}

	[RPC]
	void IncreaseDur(int level)
	{
		duration += 0.5f * level;
	}

	[RPC]
	void DecreaseLength(int level)
	{
		length *= 1 - 0.25f * level;
	}

	[RPC]
	void ActivateSilence()
	{
		silences = true;
	}

	[RPC]
	void ActivateAmplify()
	{
		amplifies = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(other.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
			if(spell.team != damageSystem.Team())
			{
				if(!other.GetComponent<SpellCasting>().isShielding && !other.GetComponent<DamageSystem>().invulnerable)
				{
					if(amplifies)
					{
						other.GetComponent<DamageSystem>().Amplify(0.35f, duration);
					}
					damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
					
					other.GetComponent<Movement>().RpcBound(duration, length);
					if(silences)
					{
						other.GetComponent<SpellCasting>().RpcSilence(1.5f);
					}

                    SpellCasting spellCasting = (SpellCasting)other.GetComponent("SpellCasting");
                    //GameObject rope = Instantiate(bindRope, this.transform.position, Quaternion.identity);
                    //rope.GetComponent<BindRope>().SetKill(duration);
                    //rope.GetComponent<BindRope>().SetBinds(transform.position, spellCasting.playerName);
                    GameObject hit = Instantiate(bindingShotHit, this.transform.position, Quaternion.identity);
                    //NetworkServer.Spawn(rope);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
			}
		}

		if(other.CompareTag ("Obstacle"))
		{
			other.SendMessage("Damage", spell.damage);
            GameObject hit = Instantiate(bindingShotHit, this.transform.position, Quaternion.identity);
            NetworkServer.Spawn(hit);
            Destroy(gameObject);
        }

		if(other.CompareTag ("Spell"))
        {
			Spell otherSpell = (Spell)other.GetComponent("Spell");
			if(spell.team != otherSpell.team && otherSpell.type == Spell.spellType.Projectile)
			{
                if (spell.destroysSpells)
                {
                    GameObject hit = Instantiate(bindingShotHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(other.gameObject);
                }
                if (otherSpell.destroysSpells)
				{
                    GameObject hit = Instantiate(bindingShotHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
			}
		}
	}
}