using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour 
{
	public float oldSpeed;
	public float speed = 50;
	public Spell spell;
	public GameObject fireballExplo;
	public AudioClip cast;

	public GameObject burnEffect;

	public float dotDamage;
	public float duration;

	bool finalBlast;

	// Use this for initialization
	void Start () {
		oldSpeed = speed;
		spell.SetColor();
		Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
		spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
		spell.Invoke ("KillSelf", 5);
		AudioSource.PlayClipAtPoint(cast, transform.position);
		if(GetComponent<NetworkView>().isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.fireballDot.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDot", RPCMode.All, upgrading.fireballDot.currentLevel);
				
				if(upgrading.fireballFinalBlast.currentLevel > 0)
				{
					GetComponent<NetworkView>().RPC ("ActivateFinalBlast", RPCMode.All);
				}
			}

			if(upgrading.fireballDmg.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDmg", RPCMode.All, upgrading.fireballDmg.currentLevel);
			}
		}
	}

	void SetSpeed(float speedBoost)
	{
		if(speed >= oldSpeed)
		{
			speed *= speedBoost;
			Invoke ("EndSpeedBoost", 0.2f);
		}
	}

	void EndSpeedBoost()
	{
		speed = oldSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
	}

	[RPC]
	void IncreaseDot(int level)
	{
		dotDamage += 0.05f * level;
		duration += 0.5f * level;
	}

	[RPC]
	void ActivateFinalBlast()
	{
		finalBlast = true;
	}

	
	[RPC]
	void IncreaseDmg(int level)
	{
		spell.damage += 0.6f * level;
		spell.knockFactor += 0.45f * level;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
			if(spell.team != damageSystem.Team())
			{
				if(other.GetComponent<NetworkView>().isMine && !other.GetComponent<SpellCasting>().isShielding && !other.GetComponent<DamageSystem>().invulnerable)
				{
					damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
					damageSystem.AddDot(dotDamage, duration, 0.5f, spell.owner, burnEffect);
					if(finalBlast)
					{
						Debug.Log ("Final blast!");
						damageSystem.AddDot (5, duration+1, duration, spell.owner, burnEffect); 
					}
					Network.Destroy (gameObject);
					Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
				}
			}
		}

		if(other.CompareTag ("Obstacle"))
		{
			if(other.name == "Collider")
			{
				if(!other.GetComponent<PrisonWall>().reflect)
				{
					other.attachedRigidbody.AddForce (spell.aimDir * spell.knockFactor * 400);
					other.SendMessage("Damage", spell.damage);
					if(GetComponent<NetworkView>().isMine)
					{
						Network.Destroy (gameObject);
						Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
					}
				}
			}
			else
			{
				other.attachedRigidbody.AddForce (spell.aimDir * spell.knockFactor * 400);
				other.SendMessage("Damage", spell.damage);
				if(GetComponent<NetworkView>().isMine)
				{
					Network.Destroy (gameObject);
					Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
				}
			}
		}
		else if(other.CompareTag ("Spell"))
		{	
			if(GetComponent<NetworkView>().isMine)
			{
				Spell otherSpell = (Spell)other.GetComponent("Spell");
				if(spell.team != otherSpell.team)
				{
					if(otherSpell.destroysSpells)
					{
						Network.Destroy (gameObject);
						Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
					}
				}
			}
		}
	}
}