using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class SpellCasting : MonoBehaviour {


	public GameObject blink;
	public GameObject fireball;
	public GameObject shield;
	public GameObject waterwave;
	public GameObject magmaBlast;
	public GameObject hook;
	public GameObject ricochetBolt;
	public GameObject bindingShot;
	public GameObject iceRang;
	public GameObject frostPrison;
	public GameObject windWalkShield;
	public GameObject healingWard;
	public GameObject placedShield;
	public GameObject lifeGrip;

	GameObject cooldownHandler;

	public ArrayList spells = new ArrayList();

	public int team;

	public GameObject mobSpell;
	public SpellInfo mob;

	public GameObject defSpell;
	public SpellInfo def;

	public GameObject offSpell1;
	public SpellInfo off1;

	public GameObject offSpell2;
	public SpellInfo off2;

	public GameObject offSpell3;
	public SpellInfo off3;

	public bool isCasting = false;
	SpellInfo currentCast;

	public bool isHooking = false;

	public bool isDead = false;

	public Texture castBar;
	public Texture castBg;

	Vector2 aimPoint;

	public string playerName;
	//public GameObject nameText;

	public GameObject myPortal;

	//GameObject myName;

	float channelTime;
	bool isChanneling;
	GameObject currentPowerUp;

	public bool isShielding = false;
	
	public float dmgBoost = 1;

	public AudioClip troll1;
	public AudioClip troll2;

	public int gold;

	public bool isSilenced = false;

	public bool blackHoling = false;

	Vector3 holePos = Vector3.zero;

	ArrayList holePlayers = new ArrayList();


	public float damageDone;


	public GoogleAnalyticsV3 analytics;

	public void Reset()
	{
		damageDone = 0;
		GlobalConstants.isFrozen = true;
		if(Network.isServer)
		{
			Invoke ("Unfreeze", 5.0f);
		}
		Silence(6);
	}

	public void BlackHole(float duration)
	{
		Debug.Log ("Black hole for " + duration.ToString() + " seconds");
		holePos = transform.position;
		Silence(duration);
		blackHoling = true;
		Debug.Log (blackHoling);
		gameObject.GetComponent<DamageSystem>().Amplify(0.5f, duration);
		Invoke ("EndBlackHole", duration);
	}

	void EndBlackHole()
	{
		blackHoling = false;
		foreach(GameObject player in holePlayers)
		{
			player.GetComponent<NetworkView>().RPC ("HolePos", RPCMode.All, Vector3.zero);
		}
		holePlayers.Clear();
	}

	void Unfreeze()
	{
		GetComponent<NetworkView>().RPC ("UnfreezeAll", RPCMode.All);
	}
	
	[RPC]
	void UnfreezeAll()
	{
		GlobalConstants.isFrozen = false;
	}

	void NewTeam()
	{
		GetComponent<NetworkView>().RPC ("UpdateTeam", RPCMode.OthersBuffered, team);
	}

	void SetTeam(int newTeam)
	{
		Invoke ("NewTeam", 1);
		team = newTeam;
	}

	[RPC]
	void IncreaseDamageDone(float amount)
	{
		damageDone += amount;
	}

	// Use this for initialization
	void Start () {
		cooldownHandler = GameObject.Find ("CooldownInfo");
		spells.Add (blink);
		spells.Add (fireball);
		spells.Add (shield);
		spells.Add (magmaBlast);
		spells.Add (hook);
		spells.Add (ricochetBolt);
		spells.Add (bindingShot);
		spells.Add (iceRang);
		spells.Add (frostPrison);
		spells.Add (windWalkShield);
		spells.Add (healingWard);
		spells.Add (placedShield);
		spells.Add (lifeGrip);
		if(GetComponent<NetworkView>().isMine)
		{
			playerName = PlayerPrefs.GetString ("Player Name");
			GetComponent<NetworkView>().RPC ("UpdateName", RPCMode.AllBuffered, playerName);
            //spells.Add(blink);
            //spells.Add(shield);
            //spells.Add(waterwave);

			GameObject spellChoices = GameObject.Find("SpellChoices");
			if(spellChoices == null)
			{
				mobSpell = blink;
				defSpell = shield;
				offSpell1 = bindingShot;
				offSpell2 = ricochetBolt;
				offSpell3 = frostPrison;
			}
			else
			{
				SpellChoices spellC = spellChoices.GetComponent<SpellChoices>();
                foreach (GameObject spell in spells)
				{
					if(spellC.offSpell1 == spell.name)
					{
						offSpell1 = spell;
					}
					if(spellC.offSpell2 == spell.name)
                    {
                        offSpell2 = spell;
					}
					if(spellC.offSpell3 == spell.name)
                    {
                        offSpell3 = spell;
					}
					if(spellC.defSpell == spell.name)
                    {
                        defSpell = spell;
					}
					if(spellC.mobSpell == spell.name)
                    {
                        mobSpell = spell;
					}
				}
			}
            
            mob = new SpellInfo(mobSpell, 1);
			def = new SpellInfo(defSpell, 2);
			off1 = new SpellInfo(offSpell1, 3);
			off2 = new SpellInfo(offSpell2, 4);
			off3 = new SpellInfo(offSpell3, 5);
			cooldownHandler.SendMessage ("SetSpell1MaxCD", off1.spellMaxCd);
			cooldownHandler.SendMessage ("SetSpell2MaxCD", off2.spellMaxCd);
			cooldownHandler.SendMessage ("SetSpell3MaxCD", off3.spellMaxCd);
			cooldownHandler.SendMessage ("SetSpell4MaxCD", def.spellMaxCd);
			cooldownHandler.SendMessage ("SetSpell5MaxCD", mob.spellMaxCd);
			Invoke ("ActivateUpgrading", 1);
		}
	}

	void ActivateUpgrading()
	{
		GameObject.Find("GameHandler").GetComponent<Upgrading>().spellCasting = this;
		GameObject.Find("GameHandler").GetComponent<Upgrading>().SetSpellCasting();
		//analytics = GameObject.Find ("GAv3").GetComponent<GoogleAnalyticsV3>();
		//analytics.StartSession();
		//analytics.LogScreen("Game");
		//Debug.Log (analytics);
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<NetworkView>().isMine)
		{
			//Cast(mob.Update());
		
			//Cast(def.Update());

			UpdateSpell(def);
			UpdateSpell(mob);
			UpdateSpell(off1);
			UpdateSpell(off2);
			UpdateSpell(off3);

			if(blackHoling)
			{
				foreach(GameObject player in ConnectionHandler_2.players)
				{
					if(player.GetComponent<SpellCasting>().team != team)
					{
						if(Vector3.Distance(player.transform.position, transform.position) < 4)
						{
							player.GetComponent<NetworkView>().RPC("HolePos", RPCMode.All, transform.position);
							player.GetComponent<DamageSystem>().Damage (0.15f, 0, transform.position, playerName);
							gameObject.GetComponent<DamageSystem>().knockback = Vector3.zero;
							holePlayers.Add(player);
						}
					}
				}
			}

			if(!isDead && !GlobalConstants.isFrozen && GameHandler.state == GameHandler.State.Game)
			{
				if(isCasting)
				{
					StartCasting (currentCast);
					if(Input.GetKeyDown(KeyCode.LeftControl))
					{
						StopCasting();
					}
				}

				if(isChanneling && !isShielding)
				{
					channelTime -= Time.deltaTime;
					if(channelTime <= 0)
					{
						FinishChannelingPowerUp();
					}
				}

				if(Input.GetKeyDown (KeyCode.Y))
				{
					GetComponent<NetworkView>().RPC ("Troll1", RPCMode.All);
				}

				if(Input.GetKeyDown (KeyCode.U))
				{
					GetComponent<NetworkView>().RPC ("Troll2", RPCMode.All);
				}
			}
			cooldownHandler.SendMessage ("SetSpell1CD", off1.spellCd);
			cooldownHandler.SendMessage ("SetSpell2CD", off2.spellCd);
			cooldownHandler.SendMessage ("SetSpell3CD", off3.spellCd);
			cooldownHandler.SendMessage ("SetSpell4CD", def.spellCd);
			cooldownHandler.SendMessage ("SetSpell5CD", mob.spellCd);
		}
	}

	void IsShielding()
	{
		isShielding = true;
	}

	public void Silence(float duration)
	{
		if(currentCast != null)
		{
			isSilenced = true;
			StopCasting ();
			Invoke("StopSilence", duration);
		}
	}

	void StopSilence()
	{
		isSilenced = false;
	}

	public void StopShielding()
	{
		isShielding = false;
	}

	public void StartChannelingPowerUp(GameObject powerUp, float duration)
	{
		channelTime = duration;
		isChanneling = true;
		currentPowerUp = powerUp;
	}

	void FinishChannelingPowerUp()
	{
		if(currentPowerUp != null)
		{
			currentPowerUp.SendMessage("Capped", gameObject);
		}
		EndChannelingPowerUp();
	}

	public void EndChannelingPowerUp()
	{
		isChanneling = false;
		currentPowerUp = null;
	}

	public void DamageBoost(float boost, float duration)
	{
		dmgBoost *= boost;
		Invoke ("EndDmgBoost", duration);
	}

	void EndDmgBoost()
	{
		dmgBoost = 1;
	}

	public void UpdateSpell(SpellInfo spell)
	{
		spell.spellCd -= Time.deltaTime;

		if(Input.GetAxis("Spell" + spell.slot) > 0.1f && !isCasting && !isDead && !isSilenced)
		{
			if(isChanneling)
			{
				EndChannelingPowerUp();
			}
			if(spell.spellCd <= 0)
			{
				isCasting = true;
				currentCast = spell;
				aimPoint = new Vector2((int)Input.mousePosition.x, (int)Input.mousePosition.y);
			}
		}
	}

	public void StartCasting(SpellInfo spell)
	{
		/*
		if(spell.spellName.Equals("Hook") && isHooking)
		{
			isCasting = false;
			currentCast = null;
			return;
		}
		*/
		spell.castTime -= Time.deltaTime;
		if(spell.castTime <= 0)
		{
			spell.castTime = spell.totalCastTime;
			spell.spellCd = spell.spellMaxCd;
			isCasting = false;
			Cast (spell.spellName);
		}
	}

	public void StopCasting()
	{
		isCasting = false;
		currentCast.castTime = currentCast.totalCastTime;
		if(blackHoling)
		{
			EndBlackHole();
		}
	}

	void OnGUI()
	{
		if(isCasting && GameHandler.state != GameHandler.State.Upgrade)
		{
			float castProgress = 1 - (currentCast.castTime / currentCast.totalCastTime);
			Vector3 playerPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
			GUI.DrawTexture (new Rect(playerPos.x - 25, Screen.height - playerPos.y - 45, 50, 8), castBg);
			GUI.DrawTexture (new Rect(playerPos.x - 24, Screen.height - playerPos.y - 44, castProgress * 48, 6), castBar);
		}

		if(isChanneling && GameHandler.state != GameHandler.State.Upgrade)
		{
			float castProgress = 1 - (channelTime / 4);
			Vector3 playerPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
			GUI.DrawTexture (new Rect(playerPos.x - 50, Screen.height - playerPos.y - 45, 100, 8), castBg);
			GUI.DrawTexture (new Rect(playerPos.x - 49, Screen.height - playerPos.y - 44, castProgress * 98, 6), castBar);
		}
	}

	void Cast(string spell)
	{
		if(spell != null && GetComponent<NetworkView>().isMine)
		{
			Vector3 aim = Camera.main.ScreenToWorldPoint (new Vector3((int)Input.mousePosition.x, (int)Input.mousePosition.y, 0));
			GetComponent<NetworkView>().RPC ("CastSpell", RPCMode.All, spell, playerName, team, aim.x, aim.y, dmgBoost, Network.AllocateViewID());
		}
	}
	
	[RPC]
	void CastSpell(string spell, string owner, int spellTeam, float aimPointX, float aimPointY, float damageBoost, NetworkViewID id)
	{
		if(spell != null)
		{
			GameObject whichSpell = null;
			foreach(GameObject s in spells)
			{
				if(s.name == spell)
				{
					whichSpell = s;
				}
			}
			if(whichSpell != null)
			{
				GameObject newSpell = (GameObject)GameObject.Instantiate(whichSpell, transform.position, transform.rotation);
				Spell spellScript = (Spell)newSpell.GetComponent("Spell");
				spellScript.owner = owner;
				spellScript.team = spellTeam;
				spellScript.damage *= damageBoost;
				spellScript.aimPoint = new Vector2(aimPointX, aimPointY);
				newSpell.GetComponent<NetworkView>().viewID = id;
			}
		}
	}

	void Dead()
	{
		isDead = true;
		StopCasting();
	}

	void Spawned()
	{
		isDead = false;
	}

	[RPC]
	void UpdateName(string name)
	{
		Debug.Log ("Updating name!");
		playerName = name;
		//GameObject root = GameObject.Find ("UI Root");
		//GameObject newText = (GameObject)GameObject.Instantiate(nameText);
		//newText.GetComponent<dfFollowObject>().enabled = false;
		//newText.transform.parent = root.transform;
		//newText.GetComponent<dfFollowObject>().attach = gameObject;
		//newText.GetComponent<dfFollowObject>().mainCamera = Camera.main;
		//newText.GetComponent<dfLabel>().Text = name;
		//newText.GetComponent<dfFollowObject>().enabled = true;
		//myName = newText;
	}

	void Invis()
	{
		//myName.GetComponent<dfLabel>().enabled = false;
	}

	void EndInvis()
	{
		//myName.GetComponent<dfLabel>().enabled = true;
	}
	
	[RPC]
	public void UpdateTeam(int newTeam)
	{
		Debug.Log ("Updating team to: " + newTeam);
		team = newTeam;
	}

	[RPC]
	public void Troll1()
	{
		AudioSource.PlayClipAtPoint(troll1, transform.position);
	}
	
	[RPC]
	public void Troll2()
	{
		AudioSource.PlayClipAtPoint(troll2, transform.position);
	}

	[RPC]
	public void LowerCd(float amount)
	{
		mob.spellCd -= 5;
		def.spellCd -= 5;
		off1.spellCd -= 5;
		off2.spellCd -= 5;
		off3.spellCd -= 5;
		/*
		CooldownInfo cd = GameObject.Find ("CooldownInfo").GetComponent<CooldownInfo>();
		System.Type t = cd.GetType();
		FieldInfo[] fields = t.GetFields();
		foreach(FieldInfo f in fields)
		{
			if(f.Name.Equals("spell1MaxCD"))
			{
				f.SetValue(cd, (float)f.GetValue(cd) - 4);
			}
			if(f.Name.Equals("spell2MaxCD"))
			{
				f.SetValue(cd, (float)f.GetValue(cd) - 4);
			}
			if(f.Name.Equals("spell3MaxCD"))
			{
				f.SetValue(cd, (float)f.GetValue(cd) - 4);
			}
			if(f.Name.Equals("spell4MaxCD"))
			{
				f.SetValue(cd, (float)f.GetValue(cd) - 4);
			}
			if(f.Name.Equals("spell5MaxCD"))
			{
				f.SetValue(cd, (float)f.GetValue(cd) - 4);
			}
		}
		*/
	}
}
public class SpellInfo
{
	public GameObject spell;
	public string spellName;
	public Spell spellScript;
	public float castTime;
	public float totalCastTime;
	public float spellCd;
	public float spellMaxCd;
	public int slot;
	public enum spellType { Projectile, Area, Other };
	public spellType type;
	
	public SpellInfo(GameObject s, int slotNumber)
	{
		spell = s;
		slot = slotNumber;
		spellScript = (Spell)spell.GetComponent("Spell");
		totalCastTime = spellScript.castTime;
		castTime = totalCastTime;
		spellMaxCd = spellScript.cooldown;
		spellCd = 0;
		spellName = s.name;
		type = (spellType) spellScript.type;
	}

	public void UpdateCd(float newCd)
	{
		spellMaxCd = newCd;
	}
}
