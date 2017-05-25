using UnityEngine;
using System.Collections;

public class LifeGrip : MonoBehaviour {
	public LineRenderer lineRenderer;
	public Spell spell;
	public float speed = 50;
	public float hookSpeed = 15;
	public GameObject owner;
	public GameObject hookedObject;
	public AudioClip cast;
	public AudioClip hit;

	bool absorb;
	bool hasHooked;

	public GameObject absorbShield;

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
			Invoke ("TimeOut", 1.5f);
		}
		AudioSource.PlayClipAtPoint(cast, transform.position);

		if(GetComponent<NetworkView>().isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();

			if(upgrading.lifeGripShield.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("ActivateAbsorb", RPCMode.All);
			}
		}
	}

	[RPC]
	void ActivateAbsorb()
	{
		absorb = true;
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
			Vector3 dir = Vector3.Normalize(owner.transform.position - hookedObject.transform.position);
			
			if(hookedObject.GetComponent<NetworkView>().isMine)
			{
				hookedObject.transform.position += dir / GlobalConstants.unitScaling * hookSpeed * Time.deltaTime * 60;
				if(!hookedObject.GetComponent<DamageSystem>().invulnerable)
				{
					hookedObject.GetComponent<DamageSystem>().Invulnerability(0.2f);
				}
			}
			if(Vector3.Distance (owner.transform.position, hookedObject.transform.position) < 1.8f)
			{
				if(hookedObject.GetComponent<NetworkView>().isMine)
				{
					Debug.Log ("I am complete!");
					if(absorb)
					{
						hookedObject.GetComponent<DamageSystem>().Absorb(30, 6);
					}
					hookedObject.GetComponent<DamageSystem>().Damage(-15, 0, transform.position, spell.owner);
					hookedObject.GetComponent<NetworkView>().RPC ("LowerCd", RPCMode.All, 4f);
					((SpellCasting)owner.GetComponent ("SpellCasting")).isHooking = false;

					Network.Destroy (gameObject);
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
					if(spell.team == damageSystem.Team())
					{
						string playerName = ((SpellCasting)other.gameObject.GetComponent ("SpellCasting")).playerName;
						if(playerName != spell.owner)
						{
							hookedObject = other.gameObject;
							CancelInvoke("TimeOut");
							AudioSource.PlayClipAtPoint(hit, transform.position);
							hasHooked = true;
							GetComponent<NetworkView>().RPC ("SyncHooked", RPCMode.All, playerName);
						}
					}
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

