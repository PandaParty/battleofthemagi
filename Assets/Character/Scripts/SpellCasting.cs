using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpellCasting : NetworkBehaviour
{
	public GameObject blink;
	public GameObject fireball;
    public GameObject arcaneBolt;
    public GameObject corruptingBolt;
	public GameObject shield;
	public GameObject magmaBlast;
	public GameObject hook;
	public GameObject bindingShot;
	public GameObject iceRang;
	public GameObject frostPrison;
	public GameObject windWalkShield;
	public GameObject healingWard;
	public GameObject placedShield;
	public GameObject lifeGrip;

	GameObject cooldownHandler;

	public ArrayList spells = new ArrayList();

    [SyncVar]
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

    [SyncVar]
	public bool isDead = false;

	public Texture castBar;
	public Texture castBg;

	Vector2 aimPoint;

    [SyncVar]
    public string playerName;
	public GameObject nameText;

	public GameObject myPortal;
    
	GameObject myName;

	float channelTime;

    [SyncVar]
	bool isChanneling;
	GameObject currentPowerUp;

    [SyncVar]
	public bool isShielding = false;
	
	public float dmgBoost = 1;

	public AudioClip troll1;
	public AudioClip troll2;
    
	public int gold;

	public bool isSilenced = false;

	Vector3 holePos = Vector3.zero;

	ArrayList holePlayers = new ArrayList();


	public float damageDone;


	public GoogleAnalyticsV3 analytics;

    public int arcaneBoltResets = 0;

    private float knockBoost = 1;

    private float cooldownSpeed = 1.0f;

    [ClientRpc]
    public void RpcReset()
	{
		damageDone = 0;
        ResetArcaneBoltResets();
        knockBoost = 1;
		GlobalConstants.isFrozen = true;
        //Silence(6);
        myName.GetComponent<TextMesh>().text = playerName;
        Invoke ("Unfreeze", 5.0f);
	}
    
	void Unfreeze()
	{
        GlobalConstants.isFrozen = false;
        isSilenced = false;
        StopCasting();
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

	void Start ()
    {
        if (team == 1)
        {
            transform.Find("FireGraphics").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("IceGraphics").gameObject.SetActive(true);
        }
        cooldownHandler = GameObject.Find ("CooldownInfo");
        spells.Add (blink);
		spells.Add (fireball);
		spells.Add (shield);
		spells.Add (magmaBlast);
		spells.Add (hook);
		spells.Add (bindingShot);
		spells.Add (iceRang);
		spells.Add (frostPrison);
		spells.Add (windWalkShield);
		spells.Add (healingWard);
		spells.Add (placedShield);
		spells.Add (lifeGrip);
        spells.Add(arcaneBolt);
        spells.Add(corruptingBolt);
		if(isLocalPlayer)
		{
			playerName = PlayerPrefs.GetString ("Player Name");
            CmdUpdateName(playerName);
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraScript camScript = camera.GetComponent<CameraScript>();
            camScript.PlayerObject = gameObject;
            //GetComponent<NetworkView>().RPC ("UpdateName", RPCMode.AllBuffered, playerName);
            

            //spells.Add(blink);
            //spells.Add(shield);
            //spells.Add(waterwave);

            GameObject spellChoices = GameObject.Find("SpellChoices");
			if(spellChoices == null)
			{
				mobSpell = blink;
				defSpell = shield;
				offSpell1 = fireball;
				offSpell2 = bindingShot;
				offSpell3 = magmaBlast;
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
            isSilenced = true;
            GlobalConstants.isFrozen = true;
			Invoke ("ActivateUpgrading", 1);
		}
	}

	void ActivateUpgrading()
	{
        GameObject handler = GameObject.Find("GameHandler");
        handler.GetComponent<Upgrading>().spellCasting = this;
        handler.GetComponent<Upgrading>().SetSpellCasting();
        //handler.GetComponent<GameHandler>().CmdNewPlayer(team);
    }
	
	void Update ()
    {
		if(isLocalPlayer)
		{
            UpdateSpell(def);
            UpdateSpell(mob);
            UpdateSpell(off1);
            UpdateSpell(off2);
            UpdateSpell(off3);
            if (!isDead && !GlobalConstants.isFrozen && GameHandler.state == GameHandler.State.Game)
            {
                if (isCasting)
                {
                    StartCasting(currentCast);
                    if (Input.GetKeyDown(KeyCode.LeftControl))
                    {
                        StopCasting();
                    }
                }

                if (isChanneling)
                {
                    if(isShielding)
                    {
                        CmdEndChannelingPowerUp();
                        return;
                    }
                    else
                    {
                        channelTime -= Time.deltaTime;
                        if (channelTime <= 0)
                        {
                            CmdFinishChannelingPowerUp();
                        }
                    }
                }

                //if (Input.GetKeyDown(KeyCode.Y))
                //{
                //    GetComponent<NetworkView>().RPC("Troll1", RPCMode.All);
                //}

                //if (Input.GetKeyDown(KeyCode.U))
                //{
                //    GetComponent<NetworkView>().RPC("Troll2", RPCMode.All);
                //}
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

    [ClientRpc]
	public void RpcSilence(float duration)
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
    
    [ClientRpc]
	public void RpcStartChannelingPowerUp(GameObject powerUp, float duration)
	{
        Debug.Log("Starting channel for a character");
        if (isChanneling || isCasting)
            return;
        Debug.Log("Passed return point");
        channelTime = duration;
		isChanneling = true;
		currentPowerUp = powerUp;
	}

    [Command]
	void CmdFinishChannelingPowerUp()
	{
		if(currentPowerUp != null)
		{
			currentPowerUp.SendMessage("Capped", gameObject);
		}
		RpcEndChannelingPowerUp();
	}

    [Command]
    public void CmdEndChannelingPowerUp()
    {
        RpcEndChannelingPowerUp();
    }

    [ClientRpc]
	public void RpcEndChannelingPowerUp()
	{
		isChanneling = false;
		currentPowerUp = null;
	}

    [ClientRpc]
	public void RpcDamageBoost(float boost, float duration)
	{
        if (!isLocalPlayer)
            return;
		dmgBoost *= boost;
		Invoke ("EndDmgBoost", duration);
	}

	void EndDmgBoost()
	{
		dmgBoost = 1;
	}

	public void UpdateSpell(SpellInfo spell)
	{
		spell.spellCd -= Time.deltaTime * cooldownSpeed;

		if(Input.GetAxis("Spell" + spell.slot) > 0.1f && !isCasting && !isDead && !isSilenced)
		{
			if(isChanneling)
			{
				CmdEndChannelingPowerUp();
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
        if(currentCast != null)
		    currentCast.castTime = currentCast.totalCastTime;
	}

	void OnGUI()
	{
		if(isCasting && GameHandler.state == GameHandler.State.Game)
		{
			float castProgress = 1 - (currentCast.castTime / currentCast.totalCastTime);
			Vector3 playerPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
			GUI.DrawTexture (new Rect(playerPos.x - 25, Screen.height - playerPos.y - 45, 50, 8), castBg);
			GUI.DrawTexture (new Rect(playerPos.x - 24, Screen.height - playerPos.y - 44, castProgress * 48, 6), castBar);
		}

		if(isLocalPlayer && isChanneling && GameHandler.state == GameHandler.State.Game)
		{
			float castProgress = 1 - (channelTime / 4);
			Vector3 playerPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
			GUI.DrawTexture (new Rect(playerPos.x - 50, Screen.height - playerPos.y - 45, 100, 8), castBg);
			GUI.DrawTexture (new Rect(playerPos.x - 49, Screen.height - playerPos.y - 44, castProgress * 98, 6), castBar);
		}
	}

	void Cast(string spell)
	{
		if(spell != null && isLocalPlayer)
		{
			Vector3 aim = Camera.main.ScreenToWorldPoint (new Vector3((int)Input.mousePosition.x, (int)Input.mousePosition.y, 0));
            Debug.Log("I cast: " + spell);
            CmdCastSpell(spell, playerName, team, aim.x, aim.y, dmgBoost);
			//GetComponent<NetworkView>().RPC ("CastSpell", RPCMode.All, spell, playerName, team, aim.x, aim.y, dmgBoost, Network.AllocateViewID());
		}
	}
	
	[Command]
	void CmdCastSpell(string spell, string owner, int spellTeam, float aimPointX, float aimPointY, float damageBoost)
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
				GameObject newSpell = Instantiate(whichSpell, transform.position, transform.rotation);
				Spell spellScript = (Spell)newSpell.GetComponent<Spell>();
				spellScript.owner = owner;
				spellScript.team = spellTeam;
				spellScript.damage *= damageBoost;
                spellScript.knockFactor *= damageBoost;
                spellScript.knockFactor *= knockBoost;
                if(knockBoost > 1)
                {
                    Debug.Log("Using knock boost");
                    knockBoost = 1;
                }
				spellScript.aimPoint = new Vector2(aimPointX, aimPointY);
                spellScript.upgrades = GetComponent<Upgrades>();
                NetworkServer.Spawn(newSpell);
			}
		}
	}

    [ClientRpc]
	public void RpcDead()
	{
		isDead = true;
		StopCasting();
        myName.GetComponent<TextMesh>().text = "";
    }

	public void Spawned()
	{
		isDead = false;
	}

	[Command]
	void CmdUpdateName(string name)
	{
		Debug.Log ("Updating name!");
		playerName = name;
		GameObject canvas = GameObject.Find ("Canvas");
		GameObject newText = Instantiate(nameText, Vector3.zero, Quaternion.identity);
        newText.GetComponent<FollowObject>().target = gameObject;
        newText.GetComponent<FollowObject>().text = name;
		myName = newText;
        NetworkServer.Spawn(newText);
        Invoke("SyncNames", 1);
	}

    void SyncNames()
    {
        RpcSyncNames();
    }

    [ClientRpc]
    void RpcSyncNames()
    {
        Debug.Log("Trying to sync name to: " + playerName);
        GameObject[] names = GameObject.FindGameObjectsWithTag("PlayerName");
        foreach (GameObject n in names)
        {
            Debug.Log("This contains: " + n.GetComponent<FollowObject>().text);
            if (n.GetComponent<FollowObject>().text.Equals(playerName))
            {
                Debug.Log("Found someone to sync name with");
                myName = n;
            }
        }
    }
    
	public void Invis()
	{
        myName.GetComponent<TextMesh>().text = "";
	}

    public void EndInvis()
	{
        myName.GetComponent<TextMesh>().text = playerName;
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
    
    public void ArcaneBoltHit(bool hasKnockBoost, bool hasCd)
    {
        if (hasCd)
        {
            RpcLowerAllCds(0.1f);
        }
        if (arcaneBoltResets == 2)
        {
            if (hasKnockBoost)
            {
                knockBoost = 1.5f;
                Debug.Log("Knock boost activated");
            }
        }
        if (arcaneBoltResets < 2)
        {
            arcaneBoltResets++;
            RpcLowerCd("ArcaneBolt", 10);
            if(arcaneBoltResets == 2)
            {
                Invoke("ResetArcaneBoltResets", 7);
            }
        }
    }

    void ResetArcaneBoltResets()
    {
        arcaneBoltResets = 0;
    }

    [ClientRpc]
    public void RpcLowerCd(string spellName, float amount)
    {
        if (!isLocalPlayer)
            return;

        if (mob.spellName == spellName)
        {
            mob.spellCd -= amount;
        }
        else if (def.spellName == spellName)
        {
            def.spellCd -= amount;
        }
        else if (off1.spellName == spellName)
        {
            off1.spellCd -= amount;
        }
        else if (off2.spellName == spellName)
        {
            off2.spellCd -= amount;
        }
        else if (off3.spellName == spellName)
        {
            off3.spellCd -= amount;
        }
    }

    [ClientRpc]
    public void RpcLowerAllCds(float amount)
    {
        mob.spellCd -= mob.spellMaxCd * amount;
        def.spellCd -= def.spellMaxCd * amount;
        off1.spellCd -= off1.spellMaxCd * amount;
        off2.spellCd -= off2.spellMaxCd * amount;
        off3.spellCd -= off3.spellMaxCd * amount;
    }

    [ClientRpc]
    public void RpcCdSpeed(float amount, float duration)
    {
        cooldownSpeed *= amount;
        CancelInvoke("ResetCdSpeed");
        Invoke("ResetCdSpeed", duration);
    }

    public void ResetCdSpeed()
    {
        cooldownSpeed = 1.0f;
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
