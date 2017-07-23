using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlacedShield : NetworkBehaviour {

	public Spell spell;
	public GameObject owner;
	public GameObject shieldHit;
	public float duration;
	public AudioClip cast;

	float amplifyAmount;
	bool knockImmune;
	bool speedBoost;
    
	void Start ()
    {
        AudioSource.PlayClipAtPoint(cast, transform.position);
        SetColor();

        if (!isServer)
            return;

        transform.position = spell.aimPoint;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		string ownerName = spell.owner;
		foreach(GameObject player in players)
		{
			string playerName = player.GetComponent<SpellCasting>().playerName;
			
			if(ownerName == playerName)
			{
				owner = player;
				break;
			}
		}
		spell.Invoke ("KillSelf", duration);
	}

    void SetColor()
    {
        switch(spell.team)
        {
            case 1:
                GetComponentInChildren<SpriteRenderer>().color = new Color(0.57f, 0.19f, 0.156f);
                break;
            case 2:
                GetComponentInChildren<SpriteRenderer>().color = new Color(0.28f, 0.77f, 0.84f);
                break;
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
    
	void OnTriggerStay2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(other.CompareTag ("Spell"))
		{
			Spell otherSpell = (Spell)other.GetComponent("Spell");
			if(spell.team != otherSpell.team && otherSpell.type == Spell.spellType.Projectile)
            {
                GameObject hit = Instantiate(shieldHit, other.transform.position, Quaternion.identity);
                NetworkServer.Spawn(hit);
				Destroy(other.gameObject);
			}
		}
	}
}
