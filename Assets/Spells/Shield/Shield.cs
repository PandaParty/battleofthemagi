using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Shield : NetworkBehaviour
{
	public Spell spell;
	public GameObject owner;
	public GameObject shieldHit;
	public float duration;
	public AudioClip cast;

	float amplifyAmount;
	bool reflectAim;
	float absorbAmount;

	// Use this for initialization
	void Start ()
    {
		AudioSource.PlayClipAtPoint(cast, transform.position);

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

        if (!isServer)
            return;
        
		owner.SendMessage("IsShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.Invoke ("KillSelf", duration);

        ActivateAmplify(spell.upgrades.shieldAmp);
        ActivateAbsorb(spell.upgrades.shieldAbsorb);
    }
    
	void ActivateAmplify(int ampLevel)
	{
		amplifyAmount = 1 + ampLevel * 0.1f;
	}
    
	void ActivateAbsorb(int absLevel)
	{
		absorbAmount = 0.5f * absLevel;
	}
    
	void Update ()
    {
        //if (!isServer)
        //    return;
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
					GameObject hitEffect = Instantiate(shieldHit, other.transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hitEffect);

					if(absorbAmount > 0)
					{
						owner.GetComponent<DamageSystem>().Damage (-otherSpell.damage * absorbAmount, 0, transform.position, spell.owner);
						Destroy(otherSpell.gameObject);
					}
					else
					{
						Vector3 normal =  Vector3.Normalize(other.transform.position - gameObject.transform.position);
						Vector3 reflected = Vector3.Reflect(otherSpell.aimDir, normal);
						
						otherSpell.aimDir = reflected;
						otherSpell.team = spell.team;
						otherSpell.damage *= amplifyAmount;
					}
				}
			}
		}
	}

}
