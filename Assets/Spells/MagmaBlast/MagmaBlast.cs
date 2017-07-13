using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MagmaBlast : NetworkBehaviour
{
	
	public Spell spell;
	public AudioClip cast;
	public bool amplify;
	public float selfDmg = 4;
	// Use this for initialization
	void Start ()
    {
		spell.SetColor();
		AudioSource.PlayClipAtPoint(cast, transform.position);
        if (!isServer)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        string ownerName = spell.owner;
        
		foreach(GameObject player in players)
		{
            DamageSystem damageSystem = player.GetComponent<DamageSystem>();
			if(spell.team != damageSystem.Team() && !damageSystem.isDead)
			{
				CircleCollider2D coll = (CircleCollider2D)player.GetComponent("CircleCollider2D");
				if(Vector3.Distance(player.transform.position, gameObject.transform.position) - coll.radius < 3.1f)
				{
                    damageSystem.Damage(spell.damage + spell.upgrades.magmaBlastDmg, spell.knockFactor, transform.position, spell.owner);
				}
			}

			string playerName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
					
			if(ownerName == playerName)
			{
				player.GetComponent<DamageSystem>().Damage(selfDmg, 0, transform.position, spell.owner);
			}
		}
		Invoke ("KillSelf", 1);
	}

	public void KillSelf()
	{
		Destroy (gameObject);
	}
}
