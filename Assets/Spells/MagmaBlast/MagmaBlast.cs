using UnityEngine;
using System.Collections;

public class MagmaBlast : MonoBehaviour {
	
	public Spell spell;
	public AudioClip cast;
	public bool amplify;
	public float selfDmg = 4;
	public GameObject blackHole;
	// Use this for initialization
	void Start () {
		spell.SetColor();
		AudioSource.PlayClipAtPoint(cast, transform.position);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		if(GetComponent<NetworkView>().isMine)
		{
			string ownerName = spell.owner;
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();

			if(upgrading.magmaBlastBlackhole.currentLevel > 0)
			{
				Debug.Log ("Black holing");
				upgrading.spellCasting.BlackHole(3);
				Network.Instantiate(blackHole, transform.position, Quaternion.identity, 0);
				GetComponent<NetworkView>().RPC ("BlackHole", RPCMode.All);
			}
			else
			{
				foreach(GameObject player in ConnectionHandler_2.players)
				{
					DamageSystem damageSystem = player.GetComponent<DamageSystem>();
					if(spell.team != damageSystem.Team() && !damageSystem.isDead)
					{
						Debug.Log ("Damage time");
						CircleCollider2D coll = (CircleCollider2D)player.GetComponent("CircleCollider2D");
						if(Vector3.Distance(player.transform.position, gameObject.transform.position) - coll.radius < 3.1f)
						{
							if(upgrading.magmaBlastAmplify.currentLevel > 0)
							{
								damageSystem.GetComponent<NetworkView>().RPC ("LavaAmplify", RPCMode.All, 0.5f, 5);
							}
							damageSystem.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage + upgrading.magmaBlastDmg.currentLevel, spell.knockFactor, transform.position, spell.owner);
							Debug.Log ("Damage time");
						}
					}

					string playerName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
					
					if(ownerName == playerName)
					{
						player.GetComponent<DamageSystem>().Damage(selfDmg - upgrading.magmaBlastCd.currentLevel, 0, transform.position, spell.owner);
						if(upgrading.magmaBlastSelfDispel.currentLevel > 0)
						{
							player.SendMessage("Dispel", spell.owner);
						}
					}
				}
			}
		}
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach(GameObject other in obstacles)
		{
			Collider2D coll = other.GetComponent<Collider2D>();
			float distance = 0;
			if(coll.GetType() == typeof(BoxCollider2D))
			{
				distance = ((BoxCollider2D)coll).size.x;
			}
			else if(coll.GetType() == typeof(CircleCollider2D))
			{
				distance = ((CircleCollider2D)coll).radius;
			}
			if(Vector3.Distance(other.transform.position, gameObject.transform.position) - distance / 2 < 3.1f)
			{
				other.GetComponent<Collider2D>().attachedRigidbody.AddForce(
					Vector3.Normalize(other.transform.position - gameObject.transform.position) 
					* 400 * spell.knockFactor);
			}
		}

		Invoke ("KillSelf", 1);
	}

	[RPC]
	void BlackHole()
	{
		GetComponent<ParticleSystem>().Stop(true);
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void KillSelf()
	{
		Destroy (gameObject);
	}
}
