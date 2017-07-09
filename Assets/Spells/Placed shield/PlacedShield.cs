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

	// Use this for initialization
	void Start ()
    {
        AudioSource.PlayClipAtPoint(cast, transform.position);
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


		//if(GetComponent<NetworkView>().isMine)
		//{
		//	Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
		//	if(upgrading.placedShieldAmp.currentLevel > 0)
		//	{
		//		GetComponent<NetworkView>().RPC ("ActivateAmplify", RPCMode.All, upgrading.placedShieldAmp.currentLevel);
				
		//		if(upgrading.placedShieldKnockImmune.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("ActivateKnockImmune", RPCMode.All);
		//		}
		//	}
			
		//	if(upgrading.placedShieldCd.currentLevel > 0)
		//	{
		//		if(upgrading.placedShieldSpeed.currentLevel > 0)
		//		{
		//			GetComponent<NetworkView>().RPC ("ActivateSpeedBoost", RPCMode.All);
		//		}
		//	}
		//}

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


	// Update is called once per frame
	void Update () {
		
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
