using UnityEngine;
using System.Collections;

public class PlacedShield : MonoBehaviour {

	public Spell spell;
	public GameObject owner;
	public GameObject shieldHit;
	public float duration;
	public AudioClip cast;

	float amplifyAmount;
	bool knockImmune;
	bool speedBoost;

	// Use this for initialization
	void Start () {
		transform.position = spell.aimPoint;
		AudioSource.PlayClipAtPoint(cast, transform.position);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		string ownerName = spell.owner;
		foreach(GameObject player in players)
		{
			string playerName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
			
			if(ownerName == playerName)
			{
				owner = player;
				break;
			}
		}
		//owner.SendMessage("IsShielding");
		//owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.Invoke ("KillSelf", duration);


		if(networkView.isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.placedShieldAmp.currentLevel > 0)
			{
				networkView.RPC ("ActivateAmplify", RPCMode.All, upgrading.placedShieldAmp.currentLevel);
				
				if(upgrading.placedShieldKnockImmune.currentLevel > 0)
				{
					networkView.RPC ("ActivateKnockImmune", RPCMode.All);
				}
			}
			
			if(upgrading.placedShieldCd.currentLevel > 0)
			{
				if(upgrading.placedShieldSpeed.currentLevel > 0)
				{
					networkView.RPC ("ActivateSpeedBoost", RPCMode.All);
				}
			}
		}

	}

	[RPC]
	void ActivateAmplify(int ampLevel)
	{
		amplifyAmount = 1 + ampLevel * 0.1f;
	}

	[RPC]
	void ActivateKnockImmune()
	{
		knockImmune = true;
	}

	[RPC]
	void ActivateSpeedBoost()
	{
		speedBoost = true;
	}


	// Update is called once per frame
	void Update () {
		//transform.position = owner.transform.position;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(other.networkView != null)
		{
			if(other.networkView.isMine)
			{
				if(other.CompareTag ("Spell"))
				{
					Spell otherSpell = (Spell)other.GetComponent("Spell");
					if(spell.team != otherSpell.team && otherSpell.type == Spell.spellType.Projectile)
					{
						Network.Instantiate(shieldHit, other.transform.position, Quaternion.identity, 0);
						Network.Destroy(other.gameObject);
					}
				}
				else if(other.CompareTag("Player"))
				{
					DamageSystem dmgSys = other.GetComponent<DamageSystem>();
					if(dmgSys.Team() != spell.team)
					{
						//if(dmgSys.movement.speed >= dmgSys.movement.oldSpeed)
						//{
							//dmgSys.movement.SpeedBoost(0.5f, 0.1f);
						//}
					}
					else
					{
						if(amplifyAmount > 0)
						{
							if(dmgSys.spellCasting.dmgBoost <= 1)
							{
								dmgSys.spellCasting.DamageBoost(1 + amplifyAmount, 0.1f);
							}
						}
						if(!speedBoost)
						{
							if(dmgSys.movement.speed <= dmgSys.movement.oldSpeed)
							{
								dmgSys.movement.SpeedBoost(1.5f, 0.1f);
							}
						}
						else if(speedBoost)
						{
							if(dmgSys.movement.speed <= dmgSys.movement.oldSpeed)
							{
								dmgSys.movement.SpeedBoost(2f, 0.1f);
							}
						}
						else if(knockImmune)
						{
							dmgSys.knockback = Vector3.zero;
						}
					}
				}
			}
			/*
			if(other.CompareTag ("Spell"))
			{
				Spell otherSpell = (Spell)other.GetComponent("Spell");
				if(spell.team != otherSpell.team)
				{
					if(otherSpell.type == Spell.spellType.Projectile)
					{
						Network.Instantiate(shieldHit, other.transform.position, Quaternion.identity, 0);
						if(absorbAmount > 0)
						{
							owner.GetComponent<DamageSystem>().Damage (-otherSpell.damage * absorbAmount, 0, transform.position, spell.owner);
						}
						Network.Destroy(otherSpell.gameObject);
					}
				}
			}
			*/
		}
	}
}
