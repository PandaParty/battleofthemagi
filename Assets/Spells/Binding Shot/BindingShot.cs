using UnityEngine;
using System.Collections;

public class BindingShot : MonoBehaviour 
{
	public float speed = 50;
	private float oldSpeed;
	public Spell spell;
	public GameObject bindingShotHit;
	public AudioClip cast;
	public AudioClip bind;

	public float duration;
	public float length;

	public GameObject bindRope;

	bool silences;
	bool amplifies;

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
			if(upgrading.bindingDuration.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDur", RPCMode.All, upgrading.bindingDuration.currentLevel);
				
				if(upgrading.bindingSilence.currentLevel > 0)
				{
					GetComponent<NetworkView>().RPC ("ActivateSilence", RPCMode.All);
				}
			}
			if(upgrading.bindingLength.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("DecreaseLength", RPCMode.All, upgrading.bindingLength.currentLevel);
				
				if(upgrading.bindingAmplify.currentLevel > 0)
				{
					GetComponent<NetworkView>().RPC ("ActivateAmplify", RPCMode.All);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
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

	[RPC]
	void IncreaseDur(int level)
	{
		duration += 0.5f * level;
	}

	[RPC]
	void DecreaseLength(int level)
	{
		length *= 1 - 0.25f * level;
	}

	[RPC]
	void ActivateSilence()
	{
		silences = true;
	}

	[RPC]
	void ActivateAmplify()
	{
		amplifies = true;
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
					if(amplifies)
					{
						other.GetComponent<DamageSystem>().Amplify(0.35f, duration);
					}
					damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
					bool foundBind = false;

					/*
					GameObject[] otherPlayers = GameObject.FindGameObjectsWithTag("Player");
					foreach(GameObject otherPlayer in otherPlayers)
					{
						if(otherPlayer != other.gameObject)
						{
							DamageSystem dmgSys = (DamageSystem)otherPlayer.GetComponent ("DamageSystem");
							if(dmgSys.Team() != damageSystem.Team ())
							{
								Debug.Log (Vector3.Distance(other.transform.position, otherPlayer.transform.position));
								if(Vector3.Distance(other.transform.position, otherPlayer.transform.position) <= 3.5f)
								{
									Debug.Log ("In range!");
									other.SendMessage("BoundTo", otherPlayer.transform);
									foundBind = true;
								}
							}
						}
					}
					*/
					/*
					if(!foundBind)
					{
						GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
						foreach(GameObject obstacle in obstacles)
						{
							if(Vector3.Distance (other.transform.position, obstacle.transform.position) <= 3.5f)
							{
								Debug.Log ("In range!");
								other.SendMessage("BoundTo", obstacle.transform);
								AudioSource.PlayClipAtPoint(bind, transform.position);
								foundBind = true;
								break;
							}
						}
					}
					*/
					other.GetComponent<Movement>().Bound(duration, length);
					if(silences)
					{
						other.SendMessage("Silence", 1.5);
					}

					GameObject rope = (GameObject) Network.Instantiate(bindRope, this.transform.position, Quaternion.identity, 0);
					rope.GetComponent<NetworkView>().RPC ("SetKill", RPCMode.All, duration);
					SpellCasting spellCasting = (SpellCasting)other.GetComponent("SpellCasting");
					rope.GetComponent<NetworkView>().RPC ("SetBinds", RPCMode.All, transform.position, spellCasting.playerName);
					Network.Destroy (gameObject);
					Network.Instantiate(bindingShotHit, this.transform.position, Quaternion.identity, 0);
				}
			}
		}

		if(other.CompareTag ("Obstacle"))
		{
			other.attachedRigidbody.AddForce (spell.aimDir * spell.knockFactor * 400);
			other.SendMessage("Damage", spell.damage);

			if(GetComponent<NetworkView>().isMine)
			{
				Network.Destroy (gameObject);
				Network.Instantiate(bindingShotHit, this.transform.position, Quaternion.identity, 0);
			}
		}

		if(other.CompareTag ("Spell"))
		{	
			if(GetComponent<NetworkView>().isMine)
			{
				Spell otherSpell = (Spell)other.GetComponent("Spell");
				if(spell.team != otherSpell.team)
				{
					if(otherSpell.destroysSpells)
					{
						Network.Destroy (gameObject);
						Network.Instantiate(bindingShotHit, this.transform.position, Quaternion.identity, 0);
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