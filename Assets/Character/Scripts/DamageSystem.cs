﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageSystem : MonoBehaviour {
	public SpellCasting spellCasting;
	public Movement movement;
	float health = 150;
	float maxHealth;
	public Vector3 knockback;
	
	public float damageHealed = 0;
	public float damageTaken = 0;
	
	public Texture healthBar;
	public Texture healthFill;
	public Texture vertLine;
	
	public bool inLava = false;
	
	public bool isDead = false;
	
	int lives = 1;
	
	public GameObject hook;
	
	public bool isInvis = false;
	
	public AudioClip lastWord;
	public AudioClip dead;
	bool playedLastword = false;
	
	List<Dot> dotList = new List<Dot>();
	List<Hot> hotList = new List<Hot>();
	
	public bool invulnerable = false;
	
	string lastDamagedBy = "";
	
	float damagedByCountdown = 3;
	
	float lavaAmp = 1;
	
	float amp = 1;

	float absorb = 0;

	public GameObject absorbShield;

	GameObject currentShieldEffect;
	
	public int Team()
	{
		return spellCasting.team;
	}
	
	[RPC]
	public void StartReset()
	{
		Reset ();
		spellCasting.Reset();
		movement.Reset();
	}
	
	void Reset()
	{
		Debug.Log ("Resetting");
		health = maxHealth;

		damageTaken = 0;
		damageHealed = 0;
		
		SpriteRenderer[] sRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer renderer in sRenderers)
		{
			Debug.Log ("Setting renderer color");
			renderer.color = new Color(1, 1, 1, 1);
		}
		
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			Debug.Log ("Enabling renderer");
			renderer.enabled = true;
		}

		PreRespawn();
	}
	
	// Use this for initialization
	void Start () {
		maxHealth = health;
	}
	
	void Dispel(string owner)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) 
		{
			if(player.GetComponent<SpellCasting>().playerName == owner)
			{
				if(player.GetComponent<SpellCasting>().team == Team())
				{
					dotList.Clear();
					movement.bound = Vector3.zero;
					spellCasting.isSilenced = false;
				}
				else
				{
					hotList.Clear();
				}
			}
		}
	}

	public void Absorb(float amount, float duration)
	{
		CancelInvoke("EndAbsorb");
		Invoke ("EndAbsorb", duration);
		absorb += amount;
		Network.Destroy(currentShieldEffect);
		currentShieldEffect = (GameObject)Network.Instantiate(absorbShield, transform.position, Quaternion.identity, 0);
	}

	void EndAbsorb()
	{
		absorb = 0;
		Network.Destroy(currentShieldEffect);
	}

	// Update is called once per frame
	void Update () {
		if(networkView.isMine && !isDead && !invulnerable)
		{
			if(absorb > 0)
			{
				knockback = Vector3.zero;
			}
			if(currentShieldEffect != null)
			{
				currentShieldEffect.transform.position = transform.position;
			}
			if(!inLava && knockback == Vector3.zero)
			{
				damagedByCountdown -= Time.deltaTime;
			}
			if(damagedByCountdown <= 0)
			{
				lastDamagedBy = "";
			}
			if(movement.bound == Vector3.zero)
			{
				transform.position += knockback / GlobalConstants.unitScaling / 2 * Time.deltaTime * 60;
			}
			else
			{
				Vector3 newPos = transform.position + knockback / GlobalConstants.unitScaling / 2 * Time.deltaTime * 60;
				if(Vector3.Distance(newPos, movement.bound) < movement.length)
				{
					transform.position = newPos;
				}
			}
			
			if(knockback.magnitude > 80)
			{
				knockback *= 0.945f;
			}
			else if(knockback.magnitude > 60)
			{
				knockback *= 0.955f;
			}
			else if(knockback.magnitude > 40)
			{
				knockback *= 0.965f;
			}
			else
			{
				knockback *= 0.97f;
			}
			
			if(knockback.magnitude < 1)
			{
				knockback = Vector3.zero;
			}
			
			if(inLava)
			{
				Damage (0.12f * lavaAmp * Time.deltaTime * 60, 0, Vector3.zero, "world");
			}
			
			
			//Damage over time management
			List<Dot> removeList = new List<Dot>();
			
			foreach(Dot dot in dotList)
			{
				dot.duration -= Time.deltaTime;
				if(dot.duration > 0)
				{
					dot.tickCounter += Time.deltaTime;
					if(dot.tickCounter >= dot.tickTime)
					{
						Damage (dot.damage, 0, Vector3.zero, dot.owner);
						dot.tickCounter -= dot.tickTime;
					}
					if(dot.effect != null && !isInvis)
					{
						dot.effect.transform.position = transform.position + new Vector3(0, 0, 0.5f);
					}
				}
				else
				{
					removeList.Add (dot);
				}
			}
			
			foreach(Dot dot in removeList)
			{
				Network.Destroy(dot.effect);
				dotList.Remove(dot);
			}
			
			
			
			//Heal over time management
			List<Hot> removeHots = new List<Hot>();
			
			foreach(Hot hot in hotList)
			{
				hot.duration -= Time.deltaTime;
				if(hot.duration > 0)
				{
					hot.tickCounter += Time.deltaTime;
					if(hot.tickCounter >= hot.tickTime)
					{
						Damage (-hot.damage, 0, Vector3.zero, "world");
						hot.tickCounter -= hot.tickTime;
					}
					if(hot.effect != null && !isInvis)
					{
						hot.effect.transform.position = transform.position + new Vector3(0, 0, 0.5f);
					}
				}
				else
				{
					removeHots.Add (hot);
				}
			}
			
			foreach(Hot hot in removeHots)
			{
				Network.Destroy(hot.effect);
				hotList.Remove(hot);
			}
		}
	}
	
	[RPC]
	void LavaAmplify(float damageIncrease, float duration)
	{
		lavaAmp = 1 + damageIncrease;
		Invoke("EndLavaAmplify", duration);
	}
	
	void EndLavaAmplify()
	{
		lavaAmp = 1;
	}
	
	public void Amplify(float damageIncrease, float duration)
	{
		amp = 1 + damageIncrease;
		Invoke("EndAmplify", duration);
	}
	
	void EndAmplify()
	{
		amp = 1;
	}
	
	void OnGUI()
	{
		if(!isInvis && GameHandler.state != GameHandler.State.Upgrade)
		{
			float currentHealth = health / maxHealth;
			float currentDamageTaken = (damageTaken * 0.5f) / maxHealth;
			Vector3 playerPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
			GUI.DrawTexture (new Rect(playerPos.x - 60, Screen.height - playerPos.y - 60, 120, 12), healthBar);
			GUI.DrawTexture (new Rect(playerPos.x - 60, Screen.height - playerPos.y - 59, currentHealth * 120, 10), healthFill);
			//GUI.DrawTexture (new Rect(playerPos.x + 60 - currentDamageTaken * 120, Screen.height - playerPos.y - 59, 2, 10), vertLine);
		}
	}

	[RPC]
	void DmgInvis()
	{
		Debug.Log ("Damage system invis!");
		isInvis = true;
		foreach(Hot hot in hotList)
		{
			Debug.Log ("Heres a hot!");
			hot.effect.SetActive(false);
		}
		foreach(Dot dot in dotList)
		{
			Debug.Log ("Heres a dot!");
			dot.effect.SetActive(false);
		}
	}

	void EndInvis()
	{
		isInvis = false;
		foreach(Hot hot in hotList)
		{
			hot.effect.SetActive(true);
		}
		foreach(Dot dot in dotList)
		{
			dot.effect.SetActive(true);
		}
	}
	
	void SetHook(GameObject _hook)
	{
		hook = _hook;
	}
	
	void ResetHook()
	{
		hook = null;
	}
	
	public void AddDot(float dmg, float dur, float tick, string owner, GameObject eff)
	{
		GameObject newEffect = (GameObject)Network.Instantiate(eff, transform.position, Quaternion.identity, 0);
		dotList.Add (new Dot(dmg, dur, tick, owner, newEffect));
	}
	
	public void AddHot(float heal, float dur, float tick, GameObject eff)
	{
		GameObject newEffect = (GameObject)Network.Instantiate(eff, transform.position, Quaternion.identity, 0);
		hotList.Add (new Hot(heal, dur, tick, newEffect));
	}
	
	public void Invulnerability(float duration)
	{
		invulnerable = true;
		Invoke("EndInvulnerable", duration);
	}
	
	public void EndInvulnerable()
	{
		invulnerable = false;
	}
	
	
	
	public void Damage(float damage, float knockFactor, Vector3 position, string damagedBy)
	{
		if(!invulnerable && GameHandler.state == GameHandler.State.Game)
		{
			if(damage < 0 && damageHealed >= damageTaken * 0.5f)
			{
				//return;
			}
			if(damage > 0)
			{
				if(absorb > damage)
				{
					absorb -= damage;
				}
				else if(absorb > 0)
				{
					damage -= absorb;
					damageTaken += damage * amp;
					health = Mathf.Clamp (health - damage * amp, 0, maxHealth);
				}
				else
				{
					damageTaken += damage * amp;
					health = Mathf.Clamp (health - damage * amp, 0, maxHealth);
				}

				if(damagedBy != "world")
				{
					lastDamagedBy = damagedBy;
					damagedByCountdown = 3;
				}
			}
			else
			{
				health = Mathf.Clamp (health - damage, 0, maxHealth);
			}
			Vector3 knockDir = Vector3.Normalize(transform.position - position);
			knockback += knockDir * knockFactor * amp * (8f + (maxHealth - health) / (maxHealth/25)) / 1.8f;
			networkView.RPC ("UpdateHealth", RPCMode.OthersBuffered, health, damageTaken);
			
			if(health <= 40 && !playedLastword)
			{
				AudioSource.PlayClipAtPoint(lastWord, transform.position);
				playedLastword = true;
			}
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject player in players)
			{
				if(player.GetComponent<SpellCasting>().playerName == damagedBy)
				{
					player.networkView.RPC("IncreaseDamageDone", RPCMode.All, damage);
					break;
				}
			}
			if(damage >= 5)
			{
				spellCasting.EndChannelingPowerUp();
				if(movement.bound != Vector3.zero)
				{
					movement.bound = Vector3.zero;
				}
				if(hook != null)
				{
					hook.SendMessage("TimeOut");
					spellCasting.isHooking = false;
					hook = null;
				}
			}
			
			if(damage >= 5 || inLava)
			{
				if(health <= 0)
				{
					if(inLava)
					{
						GA.API.Design.NewEvent("Dead:Lava:In");
					}
					else
					{
						GA.API.Design.NewEvent("Dead:Lava:Out");
					}
					GA.API.Design.NewEvent("DeathCount:Player:" + spellCasting.playerName);
					GA.API.Design.NewEvent("Player:" + spellCasting.playerName + ":KD", 0);
					GA.API.Design.NewEvent("Player:" + lastDamagedBy + ":KD", 1);
					GA.API.Design.NewEvent("KillCount:Player:" + lastDamagedBy);
					isDead = true;
					spellCasting.isDead = true;
					knockback = Vector3.zero;
					lives --;
					spellCasting.SendMessage("Dead");
					dotList.Clear();
					damageTaken = 0;
					damageHealed = 0;
					invulnerable = true;
					networkView.RPC ("Hide", RPCMode.AllBuffered);
					AudioSource.PlayClipAtPoint(dead, transform.position);
					if(lives > 0)
					{
						Invoke ("SelfRespawn", 5);
					}
					else
					{
						GameObject.Find ("GameHandler").SendMessage("PlayerDead", Team ());
					}
					Debug.Log (lastDamagedBy);
					foreach(GameObject player in players)
					{
						if(player.GetComponent<SpellCasting>().playerName == lastDamagedBy)
						{
							//player.networkView.RPC("IncreaseGold", RPCMode.All, 20);
							break;
						}
					}
				}
			}
			
			if(damage < 0)
			{
				damageHealed -= damage;
			}
		}
	}
	
	[RPC]
	void IncreaseGold(int amount)
	{
		if(networkView.isMine)
		{
			spellCasting.gold += amount;
		}
	}
	
	[RPC]
	void HookDamage(float damage, float knockFactor, Vector3 position, string owner)
	{
		if(networkView.isMine)
		{
			Debug.Log ("Time to take some damage!");
			/*
			if(health > 0)
			{
				health = Mathf.Clamp (health - damage, 0, maxHealth);
				Vector3 knockDir = Vector3.Normalize(transform.position - position);
				knockback += knockDir * knockFactor * (0.5f + (maxHealth - health) / (maxHealth/5));
				networkView.RPC ("UpdateHealth", RPCMode.OthersBuffered, health);
				if(health <= 0)
				{
					isDead = true;
					lives --;
					spellCasting.SendMessage("Dead");
					knockback = Vector3.zero;
					networkView.RPC ("Hide", RPCMode.AllBuffered);
					if(lives > 0)
					{
						Invoke ("SelfRespawn", 5);
					}
					else
					{
						GameObject.Find ("GameHandler").SendMessage("PlayerDead", Team ());
					}
				}
			}
			*/
			Damage (damage, knockFactor, position, owner);
		}
	}
	
	void SelfRespawn()
	{
		transform.position = Vector3.zero;
		networkView.RPC ("PreRespawn", RPCMode.AllBuffered);
	}
	
	[RPC]
	public void PreRespawn()
	{
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = true;
		}
		
		SpriteRenderer[] sRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer renderer in sRenderers)
		{
			renderer.color = new Color(1, 1, 1, 0.5f);
		}
		
		health = maxHealth;
		
		Invoke ("Respawn", 3);
	}
	
	public void Respawn()
	{
		isDead = false;
		invulnerable = false;
		spellCasting.SendMessage ("Spawned");
		Debug.Log ("Respawned!");
		collider2D.enabled = true;
		
		SpriteRenderer[] sRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer renderer in sRenderers)
		{
			renderer.color = new Color(1, 1, 1, 1);
		}
		
		spellCasting.EndChannelingPowerUp();
		
	}
	
	[RPC]
	public void Hide()
	{
		collider2D.enabled = false;
		//Check dis out yo can't hide this shizzle
		
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer renderer in renderers)
		{
			renderer.enabled = false;
		}
	}
	
	[RPC]
	public void UpdateHealth(float currentHealth, float dmg)
	{
		health = currentHealth;
		damageTaken = dmg;
	}
}

public class Dot
{
	public float damage;
	public float duration;
	public float tickTime;
	public float tickCounter;
	public GameObject effect;
	public string owner;
	
	public Dot(float dmg, float dur, float tick, string _owner, GameObject eff)
	{
		damage = dmg;
		duration = dur;
		tickTime = tick;
		tickCounter = 0;
		owner = _owner;
		effect = eff;
	}
}

public class Hot
{
	public float damage;
	public float duration;
	public float tickTime;
	public float tickCounter;
	public GameObject effect;
	
	public Hot(float dmg, float dur, float tick, GameObject eff)
	{
		damage = dmg;
		duration = dur;
		tickTime = tick;
		tickCounter = 0;
		effect = eff;
	}
}
