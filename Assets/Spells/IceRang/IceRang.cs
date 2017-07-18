using UnityEngine;
using System.Collections;

public class IceRang : MonoBehaviour 
{
	public float speed = 50;
	private float oldSpeed;
	public Spell spell;
	public GameObject fireballExplo;
	public AudioClip cast;

	public Vector3 startPos;
	public float journeyTime = 0.5F;
	private float startTime;

	float slowAmount = 1;

	float startDist;

	Vector3 aimPoint;

	bool hasTurned = false;

	Vector3 dir;

	float arcAmount;

	bool bounces;

	// Use this for initialization
	void Start () {
		oldSpeed = speed;
		spell.SetColor();
		Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
		spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		aimPoint = new Vector3(spell.aimPoint.x, spell.aimPoint.y, 0);
		//transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed;
		spell.Invoke ("KillSelf", 6);
		AudioSource.PlayClipAtPoint(cast, transform.position);
		startTime = Time.time;
		startPos = transform.position;
		Vector3 dirBetween = startPos - aimPoint;
		float distance = Vector3.Distance(startPos, aimPoint);
		float angle = Mathf.Atan2(dirBetween.y, dirBetween.x) * Mathf.Rad2Deg;
		float startRot = -40;
		startDist = distance;
		//spell.aimDir = GlobalConstants.RotateZ(spell.aimDir, Mathf.Deg2Rad * startRot);

		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

		journeyTime = distance/7.5f;

		if(distance <= 5)
		{
			arcAmount = 3f;
		}
		else
		{
			arcAmount = (distance / Mathf.Pow(distance, 2) * 4);
		}

		if(GetComponent<NetworkView>().isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.iceRangDmg.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDmg", RPCMode.All, upgrading.iceRangDmg.currentLevel);
				
				if(upgrading.iceRangBounce.currentLevel > 0)
				{
					GetComponent<NetworkView>().RPC ("ActivateBounce", RPCMode.All);
				}
			}

			if(upgrading.iceRangSlow.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseSlow", RPCMode.All, upgrading.iceRangSlow.currentLevel);
			}
		}
	}

	[RPC]
	void IncreaseSlow(int level)
	{
		slowAmount -= 0.15f * level;
	}
	
	[RPC]
	void ActivateBounce()
	{
		bounces = true;
	}
	
	
	[RPC]
	void IncreaseDmg(int level)
	{
		spell.damage += 0.75f * level;
		spell.knockFactor += 0.7f * level;
	}


	// Update is called once per frame
	void Update () 
	{
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
	}

	void OldStuff()
	{
		//time += Time.deltaTime / 10;
		//transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed;
		//Debug.Log (Vector3.Slerp (spell.aimDir, -spell.aimDir, time));
		//transform.position += Vector3.Slerp (spell.aimDir, -spell.aimDir, time) * Time.deltaTime;
		//transform.position += transform.right * Mathf.Sin (time);
		
		if(!hasTurned)
		{
			transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			spell.aimDir = Vector3.RotateTowards(spell.aimDir, Vector3.Normalize(aimPoint - transform.position), arcAmount * Mathf.Deg2Rad, 1);
			arcAmount += (2.4f / startDist) / Vector3.Distance(transform.position, aimPoint);
			if(Vector3.Distance(transform.position, aimPoint) < 0.6f)
			{
				//Debug.Log ("Turntime");
				hasTurned = true;
			}
			/*
			Vector3 center = (startPos + aimPoint) * 0.5F;
			center -= arcAmount * transform.right;
			Vector3 riseRelCenter = startPos - center;
			Vector3 setRelCenter = aimPoint - center;
			float fracComplete = (Time.time - startTime) / journeyTime;
			transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
			transform.position += center;
			if(Vector3.Distance(transform.position, aimPoint) < 0.1f)
			{
				hasTurned = true;
				startTime = Time.time;
			}
			*/
		}
		else
		{
			transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			spell.aimDir = Vector3.RotateTowards(spell.aimDir, Vector3.Normalize(startPos - transform.position), arcAmount * Mathf.Deg2Rad, 1);
			//arcAmount -= Vector3.Distance(transform.position, startPos) / 10;
			if(Vector3.Distance(transform.position, startPos) < 0.6f)
			{
				if(GetComponent<NetworkView>().isMine)
				{
					Network.Destroy(gameObject);
				}
			}
			/*
			Vector3 center = (startPos + aimPoint) * 0.5F;
			center += arcAmount * transform.right;
			Vector3 riseRelCenter = startPos - center;
			Vector3 setRelCenter = aimPoint - center;
			float fracComplete = (Time.time - startTime) / journeyTime;
			transform.position = Vector3.Slerp(setRelCenter, riseRelCenter, fracComplete);
			transform.position += center;
			*/
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
			if(spell.team != damageSystem.Team())
			{
				if(other.GetComponent<NetworkView>().isMine && !other.GetComponent<SpellCasting>().isShielding)
				{
					damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
					//other.GetComponent<Movement>().SpeedBoost(slowAmount, 3);
					//Implement bouncing
					Network.Destroy (gameObject);
					Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
				}

			}
		}

		if(other.CompareTag ("Obstacle"))
		{
			if(!hasTurned)
			{
				other.attachedRigidbody.AddForce (spell.aimDir * spell.knockFactor * 400);
			}
			else
			{
				other.attachedRigidbody.AddForce (-spell.aimDir * spell.knockFactor * 400);
			}
			other.SendMessage("Damage", spell.damage);

			if(GetComponent<NetworkView>().isMine)
			{
				Network.Destroy (gameObject);
				Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
			}
		}
		else if(other.CompareTag ("Spell"))
		{	
			if(GetComponent<NetworkView>().isMine)
			{

				Spell otherSpell = (Spell)other.GetComponent("Spell");
				if(spell.team != otherSpell.team)
				{
					if(other.name == "NewShield(Clone)")
					{
						Debug.Log ("Hit a shield");
						//Network.Destroy (gameObject);
						//Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
					}
					else if(otherSpell.destroysSpells)
					{
						Network.Destroy (gameObject);
						Network.Instantiate(fireballExplo, this.transform.position, Quaternion.identity, 0);
					}
				}
			}
			else
			{
				Debug.Log("Not a spell");
			}
		}
	}
}