using UnityEngine;
using System.Collections;

public class WindWalkShield : MonoBehaviour {

	public Spell spell;
	public GameObject owner;
	public GameObject shieldHit;
	public float duration;
	public AudioClip cast;
	public AudioClip hit;

	public float invisDuration;


	float damageBoost = 1;


	// Use this for initialization
	void Start () {
		AudioSource.PlayClipAtPoint(cast, transform.position);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		Debug.Log (players.Length);
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

		if(GetComponent<NetworkView>().isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.windShieldDuration.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDuration", RPCMode.All, upgrading.windShieldDuration.currentLevel);
				
				if(upgrading.windShieldDamage.currentLevel > 0)
				{
					GetComponent<NetworkView>().RPC ("ActivateDamage", RPCMode.All);
				}
			}

			if(upgrading.windShieldInvis.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC("ActivateInvis", RPCMode.All);
			}
		}

		owner.SendMessage("IsShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.Invoke ("KillSelf", duration);


	}

	[RPC]
	void IncreaseDuration(int newDur)
	{
		invisDuration += newDur * 0.5f;
		duration += newDur * 0.5f;
		owner.GetComponent<SpellCasting>().CancelInvoke("StopShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.CancelInvoke("KillSelf");
		spell.Invoke ("KillSelf", duration);
	}

	[RPC]
	void ActivateDamage()
	{
		damageBoost = 1.35f;
	}

	[RPC]
	void ActivateInvis()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		Debug.Log (players.Length);
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
		Invis ();
		owner.GetComponent<DamageSystem>().Invulnerability(invisDuration);
		owner.GetComponent<SpellCasting>().StopShielding();
	}

	// Update is called once per frame
	void Update () {
		transform.position = owner.transform.position;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<NetworkView>().isMine)
		{
			if(other.CompareTag ("Spell"))
			{
				Spell otherSpell = (Spell)other.GetComponent("Spell");
				if(spell.team != otherSpell.team)
				{
					if(otherSpell.type == Spell.spellType.Projectile)
					{
						otherSpell.damage = 0;
						Network.Instantiate(shieldHit, other.transform.position, Quaternion.identity, 0);
						Network.Destroy(other.gameObject);

						GetComponent<NetworkView>().RPC("Invis", RPCMode.All);

					}
				}
			}
		}
	}

	void LocalInvis()
	{
		Debug.Log ("Local invis!");
		Invoke ("EndInvis", invisDuration);
		spell.CancelInvoke("KillSelf");
		
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}
		
		SpriteRenderer[] sRenderers = owner.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer renderer in sRenderers)
		{
			renderer.color = new Color(1, 1, 1, 0.5f);
		}
		
		gameObject.GetComponent<Collider2D>().enabled = false;

		owner.GetComponent<Movement>().SpeedBoost(1.75f, invisDuration);
		if(damageBoost > 0)
		{
			owner.GetComponent<SpellCasting>().DamageBoost(damageBoost, invisDuration);
		}
		owner.GetComponent<NetworkView>().RPC ("DmgInvis", RPCMode.All);
	}

	[RPC]
	void Invis()
	{
		Debug.Log ("Starting invis!");
		AudioSource.PlayClipAtPoint(hit, transform.position);
		if(GetComponent<NetworkView>().isMine)
		{
			LocalInvis();
			return;
		}
		spell.CancelInvoke("KillSelf");
		Invoke ("EndInvis", invisDuration);

		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}

		Debug.Log (owner.GetComponent<SpellCasting>().playerName);

		renderers = owner.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}

		gameObject.GetComponent<Collider2D>().enabled = false;
		
		Debug.Log ("Invis Started!");
		owner.SendMessage ("Invis");
	}

	void EndInvis()
	{
		Debug.Log ("End invis!");
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}

		renderers = owner.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}

		SpriteRenderer[] sRenderers = owner.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer sRend in sRenderers)
		{
			sRend.color = new Color(1, 1, 1, 1);
		}

		owner.SendMessage ("EndInvis");
		spell.KillSelf();
	}
}
