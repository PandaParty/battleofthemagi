using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LifeGrip : NetworkBehaviour
{
	public LineRenderer lineRenderer;
	public Spell spell;
	public float speed = 50;
	public float hookSpeed = 15;
	public GameObject owner;
	public GameObject hookedObject;
	public AudioClip cast;
	public AudioClip hit;

    public Material fireMat;
    public Material iceMat;

    bool absorb;
	bool hasHooked;

	public GameObject absorbShield;
    
	void Start ()
    {
        AudioSource.PlayClipAtPoint(cast, transform.position);
        SetColor();
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
		
		Invoke ("TimeOut", 1.5f);

        if (spell.upgrades.lifeGripShield > 0)
            ActivateAbsorb();
        
	}


    void SetColor()
    {
        switch (spell.team)
        {
            case 1:
                gameObject.GetComponent<LineRenderer>().material = fireMat;
                break;
            case 2:
                gameObject.GetComponent<LineRenderer>().material = iceMat;
                break;
        }
    }

    void ActivateAbsorb()
	{
		absorb = true;
	}

	void TimeOut()
	{
		((SpellCasting)owner.GetComponent ("SpellCasting")).isHooking = false;
		owner.SendMessage("ResetHook");
		Destroy (gameObject);
	}
	
	// Update is called once per frame
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
			Vector3 dir = Vector3.Normalize(owner.transform.position - hookedObject.transform.position);

            hookedObject.GetComponent<Movement>().RpcMove(dir / GlobalConstants.unitScaling * hookSpeed * Time.deltaTime * 60);
   //         if (!hookedObject.GetComponent<DamageSystem>().invulnerable)
			//{
			//	hookedObject.GetComponent<DamageSystem>().Invulnerability(0.2f);
			//}
			if(Vector3.Distance (owner.transform.position, hookedObject.transform.position) < 1.8f)
			{
                owner.GetComponent<SpellCasting>().isHooking = false;
                Destroy(gameObject);
				if(absorb)
				{
					hookedObject.GetComponent<DamageSystem>().Absorb(20, 3);
				}
				hookedObject.GetComponent<DamageSystem>().Damage(-15, 0, transform.position, spell.owner);
			    //hookedObject.GetComponent<NetworkView>().RPC ("LowerCd", RPCMode.All, 4f);
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
	
	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(hookedObject == null)
		{
			if(other.CompareTag("Player"))
			{
				DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
				if(spell.team == damageSystem.Team())
				{
					string playerName = ((SpellCasting)other.gameObject.GetComponent ("SpellCasting")).playerName;
					if(playerName != spell.owner)
					{
						hookedObject = other.gameObject;
						CancelInvoke("TimeOut");
						AudioSource.PlayClipAtPoint(hit, transform.position);
						hasHooked = true;
						//GetComponent<NetworkView>().RPC ("SyncHooked", RPCMode.All, playerName);
					}
				}
			}
		}
	}

    [ClientRpc]
    void RpcLineRenderer(int index, Vector3 pos)
    {
        lineRenderer.SetPosition(index, pos);
    }
}

