using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Hook : NetworkBehaviour
{
	public LineRenderer lineRenderer;
	public Spell spell;
	public float speed = 50;
	public float hookSpeed = 15;
	public GameObject owner;
	public GameObject hookedObject;
	public AudioClip cast;
	public AudioClip hit;
	
	bool hasHooked = false;
	bool pulling = false;
	bool invulnerability = false;
    
	void Start ()
    {
        AudioSource.PlayClipAtPoint(cast, transform.position);
        if (!isServer)
            return;

        Vector2 aimPos = spell.aimPoint;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		string ownerName = spell.owner;
		foreach(GameObject player in players)
		{
			string playerName = player.GetComponent<SpellCasting>().playerName;
			
			if(ownerName == playerName)
			{
				owner = player;
				player.GetComponent<SpellCasting>().isHooking = true;
				owner.SendMessage("SetHook", gameObject);
				break;
			}
		}
		spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * 2 * Time.deltaTime * 60;
		Invoke ("TimeOut", 1f);

        IncreaseDmg(spell.upgrades.hookDmg);
        if (spell.upgrades.hookPull > 0)
            ActivatePull();

        if (spell.upgrades.hookInvu > 0)
            ActivateInvu();
	}
    
	void IncreaseDmg(int level)
	{
		spell.damage += 1.5f * level;
	}
    
	void ActivatePull()
	{
		pulling = true;
        spell.damage += 3.5f;
	}
    
	void ActivateInvu()
	{
		invulnerability = true;
	}

	void TimeOut()
	{
		owner.GetComponent<SpellCasting>().isHooking = false;
		owner.SendMessage("ResetHook");
		Destroy (gameObject);
	}
	
	void Update ()
    {
        if (!isServer)
            return;

		lineRenderer.SetPosition (0, owner.transform.position);
        RpcLineRenderer(0, owner.transform.position);
        if (hookedObject == null)
		{
			transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			lineRenderer.SetPosition(1, transform.position);
            RpcLineRenderer(1, transform.position);
        }
		else
		{
			if(pulling)
			{
				hookSpeed += 0.85f;
				Vector3 dir = Vector3.Normalize(owner.transform.position - hookedObject.transform.position);
				
				hookedObject.GetComponent<Movement>().RpcMove(dir / GlobalConstants.unitScaling * hookSpeed * Time.deltaTime * 60);
				
				if(Vector3.Distance (owner.transform.position, hookedObject.transform.position) < 1.8f)
				{
					owner.GetComponent<SpellCasting>().isHooking = false;
					Destroy (gameObject);
				}
			}
			else
			{
				if(invulnerability && !owner.GetComponent<DamageSystem>().invulnerable)
				{
					owner.GetComponent<DamageSystem>().Invulnerability(0.5f);
				}
				hookSpeed += 0.85f;
				Vector3 dir = Vector3.Normalize(owner.transform.position - hookedObject.transform.position);
				
				owner.GetComponent<Movement>().RpcMove(-dir / GlobalConstants.unitScaling * hookSpeed * Time.deltaTime * 60);
				
				if(Vector3.Distance (owner.transform.position, hookedObject.transform.position) < 1.8f)
				{
					if(hookedObject.tag == "Player")
					{
						hookedObject.GetComponent<DamageSystem>().Damage (spell.damage, spell.knockFactor, owner.transform.position, spell.owner);
					}
					owner.GetComponent<SpellCasting>().isHooking = false;
					Destroy (gameObject);
				}
			}
			lineRenderer.SetPosition(1, hookedObject.transform.position);
            RpcLineRenderer(1, hookedObject.transform.position);
			transform.position = hookedObject.transform.position;
		}
		
		if(hasHooked && hookedObject == null)
		{
			TimeOut ();
		}
	}

    [ClientRpc]
    void RpcLineRenderer(int index, Vector3 pos)
    {
        lineRenderer.SetPosition(index, pos);
    }
	
	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(hookedObject == null)
		{
			if(other.CompareTag("Player"))
			{
				DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
				if(spell.team != damageSystem.Team() && !other.GetComponent<SpellCasting>().isShielding)
				{
					hookedObject = other.gameObject;
					CancelInvoke("TimeOut");
					AudioSource.PlayClipAtPoint(hit, transform.position);
					hasHooked = true;
					string playerName = ((SpellCasting)other.gameObject.GetComponent ("SpellCasting")).playerName;
						
					//GetComponent<NetworkView>().RPC ("SyncHooked", RPCMode.All, playerName);
					if(pulling && hookedObject.tag == "Player")
					{
                        hookedObject.GetComponent<DamageSystem>().Damage(spell.damage, spell.knockFactor, owner.transform.position, spell.owner);
						//hookedObject.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage, spell.knockFactor, owner.transform.position, spell.owner);
					}
				}
			}
			if(other.CompareTag("Obstacle"))
            {
                AudioSource.PlayClipAtPoint(hit, transform.position);
                if (!pulling)
                {
                    hookedObject = other.gameObject;
                    CancelInvoke("TimeOut");
                    hasHooked = true;
                }
                else
                {
                    TimeOut();
                }
			}
		}
	}
}

