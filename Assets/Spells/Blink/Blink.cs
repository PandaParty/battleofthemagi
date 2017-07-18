using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Blink : NetworkBehaviour {
	public Spell spell;
	public float speed = 50;
	public GameObject owner;
	public AudioClip cast;
	public AudioClip hit;

	public GameObject effect;

	float unitsTravelled = 0;
	bool thrusting = false;

	GameObject[] players;

	ArrayList playersHit = new ArrayList();

    private bool stopped = false;
    
	void Start ()
    {
        AudioSource.PlayClipAtPoint(cast, transform.position);
        if (!isServer)
            return;

		Vector2 aimPos = spell.aimPoint;
		string ownerName = spell.owner;
		players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			string playerName = player.GetComponent<SpellCasting>().playerName;
			if(ownerName.Equals(playerName))
			{
				owner = player;
                RpcSetOwner(owner);
				break;
			}
        }
        IncreaseDmg(spell.upgrades.blinkDmg);
        if(spell.upgrades.blinkThrust > 0)
        {
            thrusting = true;
        }
        if (!thrusting)
        {
            RpcStartBlink();
        }
        spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
	}
    
	void IncreaseDmg(int level)
	{
		spell.damage = 5 + level * 2;
	}
	
	void Update ()
    {
		if(isServer && !stopped)
		{
            Vector3 velocity = new Vector3(spell.aimDir.x, spell.aimDir.y, 0) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
            owner.GetComponent<Movement>().RpcMove(velocity);
			transform.position += velocity;
			unitsTravelled += 1 / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			owner.GetComponent<DamageSystem>().Invulnerability(0.1f);

			if(thrusting)
			{
				foreach(GameObject player in players)
				{
					DamageSystem damageSystem = (DamageSystem)player.GetComponent ("DamageSystem");
					if(spell.team != damageSystem.Team())
					{
						if(Vector3.Distance(player.transform.position, owner.transform.position) < 1.5)
						{
							if(!damageSystem.invulnerable)
							{
                                damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
                                RpcEndBlink();
                                owner.GetComponent<DamageSystem>().knockback = Vector3.zero;
                                spell.Invoke("KillSelf", 0.5f);
                                stopped = true;
                                //Destroy(gameObject);
							}
						}
					}
				}
			}
			else if(spell.damage > 0)
			{
				foreach(GameObject player in players)
				{
					if(!playersHit.Contains(player))
					{
						DamageSystem damageSystem = (DamageSystem)player.GetComponent ("DamageSystem");
						if(spell.team != damageSystem.Team())
						{
							if(Vector3.Distance(player.transform.position, owner.transform.position) < 3)
							{
								if(!damageSystem.invulnerable)
								{
                                    Debug.Log ("Damage time!");
                                    damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
                                    playersHit.Add (player);
								}
							}
						}
					}
				}
			}

			if(Vector3.Distance(owner.transform.position, spell.aimPoint) < 1f || unitsTravelled > 11)
			{
                RpcEndBlink();
                owner.GetComponent<DamageSystem>().knockback = Vector3.zero;
                spell.Invoke("KillSelf", 1);
                stopped = true;
            }

            CreateEffect();
        }
	}

	void CreateEffect()
	{
		GameObject groundEffect = Instantiate(effect, transform.position, Quaternion.identity);
        NetworkServer.Spawn(groundEffect);
	}
	
    [ClientRpc]
    void RpcSetOwner(GameObject o)
    {
        Debug.Log("setting owner");
        owner = o;
    }

	[ClientRpc]
	void RpcStartBlink()
	{
		Debug.Log ("Blinkin'");
		owner.GetComponent<Collider2D>().enabled = false;

		Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}
	}
	
	[ClientRpc]
	void RpcEndBlink()
	{
        if (owner.GetComponent<DamageSystem>().isDead)
            return;

		Debug.Log ("Ending blink");
		owner.GetComponent<Collider2D>().enabled = true;

		Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}
	}
}
