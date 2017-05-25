using UnityEngine;
using System.Collections;

public class RicochetBolt : MonoBehaviour 
{
	public float speed = 50;
	public Spell spell;
	public GameObject hitEffect;
	public AudioClip cast;
	public int castNumber = 1;

	// Use this for initialization
	void Start () {
		if(castNumber == 1)
		{
			Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
			spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		}
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed;
		spell.Invoke ("KillSelf", 5);
		AudioSource.PlayClipAtPoint(cast, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			if(castNumber <= 2)
			{
				DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
				if(spell.team != damageSystem.Team())
				{
					if(other.networkView.isMine && !other.GetComponent<SpellCasting>().isShielding)
					{
						damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
						Vector3 normal =  Vector3.Normalize(other.transform.position - gameObject.transform.position);
						Vector3 reflected = Vector3.Reflect(spell.aimDir, normal);
						Debug.Log (reflected);
						reflected = GlobalConstants.RotateZ(reflected, 20 * Mathf.Deg2Rad);
						networkView.RPC ("CastSpell", RPCMode.All, spell.name, spell.owner, spell.damage, spell.team, reflected.x, reflected.y, 0f, 0f, Network.AllocateViewID());
						reflected = GlobalConstants.RotateZ(reflected, -40 * Mathf.Deg2Rad);
						networkView.RPC ("CastSpell", RPCMode.All, spell.name, spell.owner, spell.damage, spell.team, reflected.x, reflected.y, 0f, 0f, Network.AllocateViewID());

						Network.Destroy (gameObject);
						Network.Instantiate(hitEffect, this.transform.position, Quaternion.identity, 0);
					}

				}
			}
		}

		if(other.CompareTag ("Obstacle"))
		{
			other.attachedRigidbody.AddForce (spell.aimDir * spell.knockFactor * 400);

			if(networkView.isMine)
			{
				if(castNumber <= 2)
				{
					CircleCollider2D coll = (CircleCollider2D)gameObject.GetComponent("CircleCollider2D");
					RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y) - (coll.radius + 2) * spell.aimDir, spell.aimDir, 4, 1<<LayerMask.NameToLayer("Obstacles"));
					Vector3 dirBetween = Vector3.Normalize(other.transform.position - gameObject.transform.position);
					if(!hit)
					{
						Debug.Log ("Only grazed, will raycast toward object");
						hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(dirBetween.x, dirBetween.y), 4, 1<<LayerMask.NameToLayer("Obstacles"));
					}

					//Vector3 normal =  Vector3.Normalize(other.transform.position - gameObject.transform.position);
					Vector3 reflected = Vector3.Reflect(spell.aimDir, hit.normal);
					//Vector2 reflected = new Vector2(hit.normal.x * spell.aimDir.x, hit.normal.y * spell.aimDir.y);
					reflected = GlobalConstants.RotateZ(reflected, 20 * Mathf.Deg2Rad);
					networkView.RPC ("CastSpell", RPCMode.All, spell.name, spell.owner, spell.damage, spell.team, reflected.x, reflected.y, hit.normal.x, hit.normal.y, Network.AllocateViewID());
					
					reflected = GlobalConstants.RotateZ(reflected, -40 * Mathf.Deg2Rad);
					networkView.RPC ("CastSpell", RPCMode.All, spell.name, spell.owner, spell.damage, spell.team, reflected.x, reflected.y, hit.normal.x, hit.normal.y, Network.AllocateViewID());
				}
				Network.Destroy (gameObject);
				Network.Instantiate(hitEffect, this.transform.position, Quaternion.identity, 0);
			}
		}
		else if(other.CompareTag ("Spell"))
		{	
			if(networkView.isMine)
			{
				Spell otherSpell = (Spell)other.GetComponent("Spell");
				if(spell.team != otherSpell.team)
				{
					if(otherSpell.destroysSpells)
					{
						Network.Destroy (gameObject);
						Network.Instantiate(hitEffect, this.transform.position, Quaternion.identity, 0);
					}
				}
			}
			else
			{
				Debug.Log("Hit a spell but who cares cuz this ain't my shizzle you dig?");
			}
		}
	}

	[RPC]
	void CastSpell(string spell, string owner, float damage, int spellTeam, float aimPointX, float aimPointY, float offsetX, float offsetY, NetworkViewID id)
	{
			GameObject newSpell = (GameObject)GameObject.Instantiate(gameObject, transform.position + new Vector3(offsetX, offsetY) / 4, transform.rotation);
			Spell spellScript = (Spell)newSpell.GetComponent("Spell");
			spellScript.owner = owner;
			spellScript.damage = damage / 2;
			spellScript.knockFactor = damage / 2;
			spellScript.team = spellTeam;
			spellScript.aimDir = new Vector2(aimPointX, aimPointY);
			((RicochetBolt)newSpell.GetComponent("RicochetBolt")).castNumber++;
			newSpell.networkView.viewID = id;

	}
}