using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour {
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

	// Use this for initialization
	void Start () {
		Vector2 aimPos = spell.aimPoint;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		string ownerName = spell.owner;
		foreach(GameObject player in players)
		{
			string playerName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
			
			if(ownerName == playerName)
			{
				owner = player;
				((SpellCasting)player.GetComponent ("SpellCasting")).isHooking = true;
				owner.SendMessage("SetHook", gameObject);
				break;
			}
		}
		spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * 2 * Time.deltaTime * 60;
		if(owner.GetComponent<NetworkView>().isMine)
		{
			Invoke ("TimeOut", 1f);
		}
		AudioSource.PlayClipAtPoint(cast, transform.position);

		if(GetComponent<NetworkView>().isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.hookDmg.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDmg", RPCMode.All, upgrading.hookDmg.currentLevel);
				
				if(upgrading.hookPull.currentLevel > 0)
				{
					GetComponent<NetworkView>().RPC ("ActivatePull", RPCMode.All);
				}
			}

			if(upgrading.hookInvu.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("ActivateInvu", RPCMode.All);
			}
		}
	}

	[RPC]
	void IncreaseDmg(int level)
	{
		spell.damage += 1.5f * level;
	}

	[RPC]
	void ActivatePull()
	{
		pulling = true;
	}

	[RPC]
	void ActivateInvu()
	{
		invulnerability = true;
	}

	void TimeOut()
	{
		if(GetComponent<NetworkView>().isMine)
		{
			((SpellCasting)owner.GetComponent ("SpellCasting")).isHooking = false;
			owner.SendMessage("ResetHook");
			Network.Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		lineRenderer.SetPosition (0, owner.transform.position);
		if(hookedObject == null)
		{
			transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			lineRenderer.SetPosition(1, transform.position);
		}
		else
		{
			if(pulling)
			{
				hookSpeed += 0.85f;
				Vector3 dir = Vector3.Normalize(owner.transform.position - hookedObject.transform.position);
				
				if(hookedObject.GetComponent<NetworkView>().isMine)
				{
					hookedObject.transform.position += dir / GlobalConstants.unitScaling * hookSpeed * Time.deltaTime * 60;
				}
				if(Vector3.Distance (owner.transform.position, hookedObject.transform.position) < 1.8f)
				{
					if(hookedObject.GetComponent<NetworkView>().isMine)
					{
						Debug.Log ("I am complete!");

						((SpellCasting)owner.GetComponent ("SpellCasting")).isHooking = false;
						Network.Destroy (gameObject);
					}
				}
			}
			else
			{
				if(invulnerability && !owner.GetComponent<DamageSystem>().invulnerable)
				{
					Debug.Log ("I should be invulnerable");
					owner.GetComponent<DamageSystem>().Invulnerability(0.5f);
				}
				hookSpeed += 0.85f;
				Vector3 dir = Vector3.Normalize(owner.transform.position - hookedObject.transform.position);
				
				if(owner.GetComponent<NetworkView>().isMine)
				{
					owner.transform.position -= dir / GlobalConstants.unitScaling * hookSpeed * Time.deltaTime * 60;
				}

				if(Vector3.Distance (owner.transform.position, hookedObject.transform.position) < 1.8f)
				{
					if(owner.GetComponent<NetworkView>().isMine)
					{
						Debug.Log ("I am complete!");
						if(hookedObject.tag == "Player")
						{
							hookedObject.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage, spell.knockFactor, owner.transform.position, spell.owner);
							//hookedObject.GetComponent<DamageSystem>().Damage (spell.damage, spell.knockFactor, owner.transform.position);
						}
						((SpellCasting)owner.GetComponent ("SpellCasting")).isHooking = false;
						Network.Destroy (gameObject);
					}
				}
			}
			lineRenderer.SetPosition(1, hookedObject.transform.position);
			transform.position = hookedObject.transform.position;
		}
		
		if(hasHooked && hookedObject == null && owner.GetComponent<NetworkView>().isMine)
		{
			TimeOut ();
		}
		
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(GetComponent<NetworkView>().isMine)
		{
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
						
						GetComponent<NetworkView>().RPC ("SyncHooked", RPCMode.All, playerName);
						if(pulling && hookedObject.tag == "Player")
						{
							hookedObject.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage, spell.knockFactor, owner.transform.position, spell.owner);
						}
					}
				}
				if(other.CompareTag("Obstacle"))
				{
					hookedObject = other.gameObject;
					CancelInvoke("TimeOut");
					AudioSource.PlayClipAtPoint(hit, transform.position);
					hasHooked = true;
				}
			}
		}
	}
	
	[RPC]
	void SyncHooked(string playerName)
	{
		Debug.Log ("Syncing hook! " + playerName);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			string otherName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
			Debug.Log ("Player name is: " + otherName);
			if(otherName == playerName)
			{
				Debug.Log ("Set hooked to: " + otherName);
				hookedObject = player;
				Debug.Log(hookedObject);
				return;
			}
		}
	}
	
}

