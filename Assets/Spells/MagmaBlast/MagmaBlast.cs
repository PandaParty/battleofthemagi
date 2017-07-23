using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MagmaBlast : NetworkBehaviour
{
	public Spell spell;
	public AudioClip cast;
	public bool amplify;
	public float selfDmg = 4;

    public Material fireMat;
    public Material iceMat;

    void Start ()
    {
		SetColor();
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
                    damageSystem.Damage(spell.damage + spell.upgrades.magmaBlastDmg * 1.5f, spell.knockFactor + (amplify ? 3 : 0), transform.position, spell.owner);
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


    void SetColor()
    {
        switch (spell.team)
        {
            case 1:
                gameObject.GetComponent<ParticleSystemRenderer>().material = fireMat;
                break;
            case 2:
                gameObject.GetComponent<ParticleSystemRenderer>().material = iceMat;
                break;
        }
    }

    public void KillSelf()
	{
		Destroy (gameObject);
	}
}
