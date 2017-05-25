using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	public Spell spell;
	public GameObject owner;
	public GameObject shieldHit;
	public float duration;
	public AudioClip cast;

	float amplifyAmount;
	bool reflectAim;
	float absorbAmount;

	// Use this for initialization
	void Start () {
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
		owner.SendMessage("IsShielding");
		owner.GetComponent<SpellCasting>().Invoke ("StopShielding", duration);
		spell.Invoke ("KillSelf", duration);

		
		if(networkView.isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.shieldAmp.currentLevel > 0)
			{
				networkView.RPC ("ActivateAmplify", RPCMode.All, upgrading.shieldAmp.currentLevel);
				
				if(upgrading.shieldAim.currentLevel > 0)
				{
					networkView.RPC ("ActivateAim", RPCMode.All);
				}
			}
			
			if(upgrading.shieldCd.currentLevel > 0)
			{
				//networkView.RPC ("DecreaseCd", RPCMode.All, upgrading.shieldCd.currentLevel);
				if(upgrading.shieldAbsorb.currentLevel > 0)
				{
					networkView.RPC ("ActivateAbsorb", RPCMode.All, upgrading.shieldAbsorb.currentLevel);
				}
			}
		}
	}

	[RPC]
	void ActivateAmplify(int ampLevel)
	{
		amplifyAmount = 1 + ampLevel * 0.1f;
	}

	[RPC]
	void ActivateAim()
	{
		reflectAim = true;
	}

	[RPC]
	void ActivateAbsorb(int absLevel)
	{
		absorbAmount = 0.5f * absLevel;
	}


	// Update is called once per frame
	void Update () {
		transform.position = owner.transform.position;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag ("Spell"))
		{
			Spell otherSpell = (Spell)other.GetComponent("Spell");
			if(spell.team != otherSpell.team)
			{
				if(otherSpell.type == Spell.spellType.Projectile)
				{
					Network.Instantiate(shieldHit, other.transform.position, Quaternion.identity, 0);
					if(absorbAmount > 0)
					{
						owner.GetComponent<DamageSystem>().Damage (-otherSpell.damage * absorbAmount, 0, transform.position, spell.owner);
						Network.Destroy(otherSpell.gameObject);
					}
					else
					{
						Vector3 normal =  Vector3.Normalize(other.transform.position - gameObject.transform.position);
						Vector3 reflected = Vector3.Reflect(otherSpell.aimDir, normal);
						
						otherSpell.aimDir = reflected;
						otherSpell.team = spell.team;
						otherSpell.damage *= amplifyAmount;

						if(reflectAim && networkView.isMine)
						{
							Vector3 aim = Camera.main.ScreenToWorldPoint (new Vector3((int)Input.mousePosition.x, (int)Input.mousePosition.y, 0));
							reflected = Vector3.Normalize(new Vector3(aim.x, aim.y) - transform.position);
							otherSpell.networkView.RPC ("SetAim", RPCMode.All, reflected.x, reflected.y, spell.team, amplifyAmount, transform.position);
						}
					}
				}
			}
		}
	}

}
