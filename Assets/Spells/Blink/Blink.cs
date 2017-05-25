using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour {
	public Spell spell;
	public float speed = 50;
	public GameObject owner;
	public AudioClip cast;
	public AudioClip hit;

	public GameObject effect;

	float unitsTravelled = 0;
	bool thrusting = false;

	GameObject[] players;

	ArrayList playersHit = new ArrayList();

	// Use this for initialization
	void Start () {
		Vector2 aimPos = spell.aimPoint;
		string ownerName = spell.owner;
		players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			string playerName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
			Debug.Log (playerName);
			if(ownerName == playerName)
			{
				owner = player;
				break;
			}
		}
		if(GetComponent<NetworkView>().isMine)
		{
			Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
			if(upgrading.blinkDmg.currentLevel > 0)
			{
				GetComponent<NetworkView>().RPC ("IncreaseDmg", RPCMode.All, upgrading.blinkDmg.currentLevel);
				
				if(upgrading.blinkThrust.currentLevel > 0)
				{
					thrusting = true;
				}
			}

			if(!thrusting)
			{
				GetComponent<NetworkView>().RPC ("StartBlink", RPCMode.All);
			}
		}
		spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
		//transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed;
		AudioSource.PlayClipAtPoint(cast, transform.position);
	}

	[RPC]
	void IncreaseDmg(int level)
	{
		spell.damage = 5 + level * 2;
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<NetworkView>().isMine)
		{
			owner.transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y, 0) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			transform.position = owner.transform.position;
			unitsTravelled += 1 / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
			owner.GetComponent<DamageSystem>().Invulnerability(0.1f);

			if(thrusting)
			{
				foreach(GameObject player in players)
				{
					DamageSystem damageSystem = (DamageSystem)player.GetComponent ("DamageSystem");
					if(spell.team != damageSystem.Team())
					{
						if(Vector3.Distance(player.transform.position, owner.transform.position) < 1.5)
						{
							if(!damageSystem.invulnerable)
							{
								damageSystem.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage, 10.0f, owner.transform.position, spell.owner);
								GetComponent<NetworkView>().RPC ("EndBlink", RPCMode.All);
							}
						}
					}

				}
			}
			else if(spell.damage > 0)
			{
				foreach(GameObject player in players)
				{
					if(!playersHit.Contains(player))
					{
						DamageSystem damageSystem = (DamageSystem)player.GetComponent ("DamageSystem");
						if(spell.team != damageSystem.Team())
						{
							if(Vector3.Distance(player.transform.position, owner.transform.position) < 3)
							{
								if(!player.GetComponent<SpellCasting>().isShielding && !damageSystem.invulnerable)
								{
									Debug.Log ("Damage time!");
									damageSystem.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage, 0.0f, owner.transform.position, spell.owner);
									playersHit.Add (player);
									//networkView.RPC ("EndBlink", RPCMode.All);
								}
							}
						}
					}
				}
			}

			if(Vector3.Distance(owner.transform.position, spell.aimPoint) < 1f || unitsTravelled > 11)
			{
				GetComponent<NetworkView>().RPC ("EndBlink", RPCMode.All);
			}




		}

		//Should be moved back to owner dealing damage

		CreateEffect();
	}


		


	void CreateEffect()
	{
		GameObject.Instantiate(effect, transform.position, Quaternion.identity);
	}
	
	[RPC]
	void StartBlink()
	{
		Debug.Log ("Blinkin'");
		owner.GetComponent<Collider2D>().enabled = false;

		Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}
	}
	
	[RPC]
	void EndBlink()
	{
		Debug.Log ("Ending blink");
		owner.GetComponent<Collider2D>().enabled = true;

		Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}
		if(GetComponent<NetworkView>().isMine)
		{
			owner.GetComponent<DamageSystem>().knockback = Vector3.zero;
		}
		Destroy(gameObject);
	}
}
