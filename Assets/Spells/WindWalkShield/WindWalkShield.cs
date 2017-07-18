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
    
	void Start ()
    {
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

        if (!isServer)
            return;

        IncreaseDuration(spell.upgrades.windShieldDuration);
        if (spell.upgrades.windShieldDamage > 0)
            ActivateDamage();

        if (spell.upgrades.windShieldInvis > 0)
            ActivateInvis();

		owner.SendMessage("IsShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.Invoke ("KillSelf", duration);
    }

	void IncreaseDuration(int newDur)
	{
		invisDuration += newDur * 0.5f;
		duration += newDur * 0.5f;
		owner.GetComponent<SpellCasting>().CancelInvoke("StopShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.CancelInvoke("KillSelf");
		spell.Invoke ("KillSelf", duration);
	}
    
	void ActivateDamage()
	{
		damageBoost = 1.35f;
	}
    
	void ActivateInvis()
	{
        StartInvis();
		owner.GetComponent<SpellCasting>().StopShielding();
	}
    
	void Update ()
    {
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
        RpcInvis();
        Invoke("EndInvis", invisDuration);
    }

	void TeamInvis()
	{
		SpriteRenderer[] sRenderers = owner.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer renderer in sRenderers)
		{
			renderer.color = new Color(1, 1, 1, 0.5f);
		}
        
        if (damageBoost > 0)
        {
            owner.GetComponent<SpellCasting>().RpcDamageBoost(damageBoost, invisDuration);
        }
    }

    [ClientRpc]
    void RpcInvis()
    {
        AudioSource.PlayClipAtPoint(hit, transform.position);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        string ownerName = spell.owner;
        int localTeam = 0;
        foreach (GameObject player in players)
        {
            string playerName = ((SpellCasting)player.GetComponent("SpellCasting")).playerName;
            if (ownerName == playerName)
            {
                owner = player;
            }
            if(player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localTeam = player.GetComponent<SpellCasting>().team;
            }
        }

        owner.GetComponent<DamageSystem>().DmgInvis();

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        
        if (spell.team == localTeam)
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

    [ClientRpc]
    void RpcEndInvis()
    {
        Debug.Log("End invis!");
        Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        SpriteRenderer[] sRenderers = owner.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sRend in sRenderers)
        {
            sRend.color = new Color(1, 1, 1, 1);
        }

        owner.SendMessage("EndInvis");
    }

	void EndInvis()
	{
        RpcEndInvis();
        spell.Invoke("KillSelf", 1);
	}
}
