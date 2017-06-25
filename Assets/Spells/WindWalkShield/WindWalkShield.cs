using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WindWalkShield : NetworkBehaviour
{

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

        if (!isServer)
            return;

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

		//if(GetComponent<NetworkView>().isMine)
		//{
		//	Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
		//	if(upgrading.windShieldDuration.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("IncreaseDuration", RPCMode.All, upgrading.windShieldDuration.currentLevel);
				
		//		if(upgrading.windShieldDamage.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("ActivateDamage", RPCMode.All);
		//		}
		//	}

		//	if(upgrading.windShieldInvis.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC("ActivateInvis", RPCMode.All);
		//	}
		//}

		owner.SendMessage("IsShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.Invoke ("KillSelf", duration);
        
        //StartInvis();
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
        if (!isServer)
            return;

		transform.position = owner.transform.position;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(other.CompareTag ("Spell"))
		{
			Spell otherSpell = (Spell)other.GetComponent("Spell");
			if(spell.team != otherSpell.team)
			{
				if(otherSpell.type == Spell.spellType.Projectile)
				{
					otherSpell.damage = 0;
					GameObject hitEffect = Instantiate(shieldHit, other.transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hitEffect);

                    Destroy(other.gameObject);
                    StartInvis();
					//GetComponent<NetworkView>().RPC("Invis", RPCMode.All);

				}
			}
		}
	}

    void StartInvis()
    {
        spell.CancelInvoke("KillSelf");
        
        gameObject.GetComponent<Collider2D>().enabled = false;
        owner.GetComponent<Movement>().RpcSpeedBoost(1.75f, invisDuration);
        owner.GetComponent<DamageSystem>().DmgInvis();
        RpcInvis();
    }

	void TeamInvis()
	{
		SpriteRenderer[] sRenderers = owner.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer renderer in sRenderers)
		{
			renderer.color = new Color(1, 1, 1, 0.5f);
		}
		


		//if(damageBoost > 0)
		//{
		//	owner.GetComponent<SpellCasting>().DamageBoost(damageBoost, invisDuration);
		//}
		//owner.GetComponent<NetworkView>().RPC ("DmgInvis", RPCMode.All);
	}

    [ClientRpc]
    void RpcInvis()
    {
        AudioSource.PlayClipAtPoint(hit, transform.position);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        string ownerName = spell.owner;
        foreach (GameObject player in players)
        {
            string playerName = ((SpellCasting)player.GetComponent("SpellCasting")).playerName;
            if (ownerName == playerName)
            {
                owner = player;
                break;
            }
        }

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Invoke("EndInvis", invisDuration);
        
        if (owner.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            TeamInvis();
            return;
        }

        renderers = owner.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
            //renderer.color = new Color(1, 1, 1, 0.5f);
        }
        owner.GetComponent<SpellCasting>().Invis();

    }

	[RPC]
	void Invis()
	{
		Debug.Log ("Starting invis!");
		AudioSource.PlayClipAtPoint(hit, transform.position);
        
		owner.SendMessage ("Invis");
	}

	void EndInvis()
	{
		Debug.Log ("End invis!");
        Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
		
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
