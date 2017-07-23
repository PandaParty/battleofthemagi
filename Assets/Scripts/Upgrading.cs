using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using System;

public class Upgrading : MonoBehaviour {

	#region UpgradeInfo
	public UpgradeInfo fireballDot;
	public UpgradeInfo fireballFinalBlast;
	public UpgradeInfo fireballDmg;
	public UpgradeInfo fireballCd;

	public UpgradeInfo healingDispel;
	public UpgradeInfo healingDispelHeal;
	public UpgradeInfo healingDuration;
	public UpgradeInfo healingDamageReduct;
	public UpgradeInfo healingDmg;
	public UpgradeInfo healingLifesteal;
	public UpgradeInfo healingBloom;
	public UpgradeInfo healingBurst;
	
	public UpgradeInfo magmaBlastCd;
	public UpgradeInfo magmaBlastSelfDispel;
	public UpgradeInfo magmaBlastBlackhole;
	public UpgradeInfo magmaBlastDmg;
	public UpgradeInfo magmaBlastAmplify;

	public UpgradeInfo iceRangSlow;
	public UpgradeInfo iceRangCd;
	public UpgradeInfo iceRangCd2;
	public UpgradeInfo iceRangDmg;
	public UpgradeInfo iceRangBounce;

	public UpgradeInfo bindingDuration;
	public UpgradeInfo bindingSilence;
	public UpgradeInfo bindingLength;
	public UpgradeInfo bindingAmplify;
	
	public UpgradeInfo frostPrisonHealth;
	public UpgradeInfo frostPrisonReflect;
	public UpgradeInfo frostPrisonDuration;
	public UpgradeInfo frostPrisonCircleWall;
	public UpgradeInfo frostPrisonRamp;
	public UpgradeInfo frostPrisonStorm;

	public UpgradeInfo shieldAmp;
	public UpgradeInfo shieldAim;
	public UpgradeInfo shieldCd;
	public UpgradeInfo shieldAbsorb;
	
	public UpgradeInfo windShieldDuration;
	public UpgradeInfo windShieldDamage;
	public UpgradeInfo windShieldCd;
	public UpgradeInfo windShieldInvis;
	
	public UpgradeInfo hookDmg;
	public UpgradeInfo hookPull;
	public UpgradeInfo hookCd;
	public UpgradeInfo hookInvu;

	public UpgradeInfo blinkDmg;
	public UpgradeInfo blinkThrust;
	public UpgradeInfo blinkCd;
	public UpgradeInfo blinkInstant;
	
	public UpgradeInfo placedShieldKnockImmune;
	public UpgradeInfo placedShieldAmp;
	public UpgradeInfo placedShieldCd;
	public UpgradeInfo placedShieldSpeed;
	
	public UpgradeInfo lifeGripCd;
	public UpgradeInfo lifeGripShield;

    public UpgradeInfo arcaneBoltDmg;
    public UpgradeInfo arcaneBoltKnock;
    public UpgradeInfo arcaneBoltHeal;
    public UpgradeInfo arcaneBoltCd;

    public UpgradeInfo corruptingBoltDmgRed;
    public UpgradeInfo corruptingBoltCd;
    public UpgradeInfo corruptingBoltAmplify;
    public UpgradeInfo corruptingBoltBlast;

    #endregion

    CooldownInfo cd = null;

	public SpellCasting spellCasting = null;
	List<SpellInfo> spells = new List<SpellInfo>();

	public Texture2D background;
	//public Texture button;

	public GUIStyle button;
	public GUIStyle tooltip;

	int spellcastSet = 0;

	string names = "";

	public bool isUpgrading;
    public Text goldText;
	//public dfLabel goldText;
    
	void SetShit ()
    {
		spellcastSet = 1;
		#region Fireball
		fireballDot = new UpgradeInfo("Damage \n over \n Time", 3, 40, null, button);
		fireballDot.tooltip = "Increases damage per second by 0.1/0.2/0.3, and duration by 0.5/1/1.5 (Total dot damage increases by 1.15/2.4/3.75)";
		fireballFinalBlast = new UpgradeInfo("Final blast", 1, 120, fireballDot, button);
		fireballFinalBlast.tooltip = "When the fireball DoT ends, deals a final 5 damage (this can be lethal)";


		fireballDmg = new UpgradeInfo("Damage", 3, 40, null, button);
		fireballDmg.tooltip = "Increases damage by 1/2/3 and knock factor by 0.8/1.6/2.4";
		fireballCd = new UpgradeInfo("Cone", 1, 120, fireballDmg, button);
		fireballCd.tooltip = "Shoots two additional smaller fireballs with half damage in a cone";

		fireballDot.relative = fireballDmg;
		fireballDmg.relative = fireballDot;
		#endregion

		#region Healing

		#region inactive
		healingDispel = new UpgradeInfo("Dispel", 1, 120, null, button);
		healingDispel.tooltip = "Your healing spell can now dispels negative effects from your teammates and some positive effects from opponents";
		healingDispelHeal = new UpgradeInfo("Dispel \n self \n heal", 1, 80, healingDispel, button);
		healingDispelHeal.tooltip = "When successfully dispelling something, you now heal yourself for 18 health over 3 seconds";

		healingDmg = new UpgradeInfo("Damage", 3, 40, null, button);
		healingDmg.tooltip = "Your healing spell can now place a DoT on enemy players, dealing 6/12/18 damage over 3 seconds";
		healingLifesteal = new UpgradeInfo("Lifesteal", 1, 80, healingDmg, button);
		healingLifesteal.tooltip = "Your healing spell DoT effect heals you for 50% of the damage dealt";

		#endregion
		healingDuration = new UpgradeInfo("Duration", 3, 40, null, button);
		healingDuration.tooltip = "Increases duration of heal by 0.7/1.4/2.1 seconds (total heal 26/31/36)";
		healingDamageReduct = new UpgradeInfo("Damage \n reduction", 1, 120, healingDuration, button);
		healingDamageReduct.tooltip = "Reduces damage taken on targets with the heal by 50%";

		healingBloom = new UpgradeInfo("Bloom Time", 3, 40, null, button);
		healingBloom.tooltip = "Heal now blooms in 0.5/0.3/0.1 seconds";
		healingBurst = new UpgradeInfo("Instant", 1, 120, healingBloom, button);
		healingBurst.tooltip = "Now instantly heals the targets for 32 health";

        healingDuration.relative = healingBloom;
        healingBloom.relative = healingDuration;
        healingDispel.relative = healingDmg;
		healingDmg.relative = healingDispel;
		healingDmg.relative = healingDuration;

		#endregion Healing

		#region Magma Blast
		#region Inactive
		magmaBlastSelfDispel = new UpgradeInfo("Self-Dispel", 1, 80, magmaBlastCd, button);
		magmaBlastSelfDispel.tooltip = "Dispels all negative effects from you on completion of cast";
		#endregion
		magmaBlastCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		magmaBlastCd.tooltip = "Decreases magma blast cooldown by 0.35/0.7/1.05";
		magmaBlastBlackhole = new UpgradeInfo("Gravity", 1, 120, magmaBlastCd, button);
		magmaBlastBlackhole.tooltip = "Magma blast is now channeled for 2 seconds, and instead of knocking players away, it pulls them toward you.";

		magmaBlastDmg = new UpgradeInfo("Damage", 3, 40, null, button);
		magmaBlastDmg.tooltip = "Increases damage by 1.5/3/4.5";
		magmaBlastAmplify = new UpgradeInfo("Strength", 1, 120, magmaBlastDmg, button);
		magmaBlastAmplify.tooltip = "Increases knock factor by 3";
		
		magmaBlastCd.relative = magmaBlastDmg;
		magmaBlastDmg.relative = magmaBlastCd;
		#endregion

		#region IceRang
		iceRangDmg = new UpgradeInfo("Damage", 3, 40, null, button);
		iceRangDmg.tooltip = "Increases damage by 0.75/1.5/2.25";
		iceRangBounce = new UpgradeInfo("Bounce", 1, 120, iceRangSlow, button);
		iceRangBounce.tooltip = "Ice-rang now bounces";
		
		iceRangSlow = new UpgradeInfo("Slow", 3, 40, null, button);
		iceRangSlow.tooltip = "Now slows by 15/30/45 percent for 3 seconds";
		iceRangCd = new UpgradeInfo("Cooldown", 1, 120, iceRangDmg, button);
		iceRangCd.tooltip = "Decreases cooldown by 1.5 seconds";

		iceRangCd2 = new UpgradeInfo("Cooldown", 1, 120, iceRangSlow, button);
		iceRangCd2.tooltip = "Decreases cooldown by 1.5 seconds";
		
		iceRangSlow.relative = iceRangDmg;
		iceRangDmg.relative = iceRangSlow;
		#endregion

		#region Binding Shot
		bindingDuration = new UpgradeInfo("Duration", 2, 60, null, button);
		bindingDuration.tooltip = "Increases duration by 0.5/1";
		bindingSilence = new UpgradeInfo("Silence", 1, 120, bindingDuration, button);
		bindingSilence.tooltip = "Silences for 1.5 seconds";
		
		bindingLength = new UpgradeInfo("Leash length", 2, 60, null, button);
		bindingLength.tooltip = "Decreases leash length by 25%/50%";
		bindingAmplify = new UpgradeInfo("Damage amplify", 1, 120, bindingDuration, button);
		bindingAmplify.tooltip = "Amplifies damage taken by 35%";
		
		bindingDuration.relative = bindingLength;
		bindingLength.relative = bindingDuration;
		#endregion

		#region Frost Prison
		#region Inactive
		frostPrisonHealth = new UpgradeInfo("Indestructible", 1, 120, null, button);
		frostPrisonHealth.tooltip = "Frost Prison is now indestructible";
		frostPrisonReflect = new UpgradeInfo("Reflects", 1, 120, frostPrisonHealth, button);
		frostPrisonReflect.tooltip = "Reflects projectiles";
		#endregion

		frostPrisonDuration = new UpgradeInfo("Duration", 2, 60, null, button);
		frostPrisonDuration.tooltip = "Duration increased by 0.5/1 seconds (damage remains the same)";
		frostPrisonCircleWall = new UpgradeInfo("Circle", 1, 120, frostPrisonDuration, button);
		frostPrisonCircleWall.tooltip = "Adds an extra wall and decreases the form time to 0.4 seconds, but no longer removes damage";

		frostPrisonRamp = new UpgradeInfo("Damage Ramp", 2, 60, null, button);
		frostPrisonRamp.tooltip = "Damage starts at 0.235/0.27 instead of 0.2";
		frostPrisonStorm = new UpgradeInfo("Glacial Storm", 1, 120, frostPrisonRamp, button);
		frostPrisonStorm.tooltip = "No longer has walls, instead slows and deals additional damage";
		
		frostPrisonDuration.relative = frostPrisonRamp;
		frostPrisonRamp.relative = frostPrisonDuration;
		#endregion

		#region Reflect Shield
		shieldAmp = new UpgradeInfo("Amplify", 3, 40, null, button);
		shieldAmp.tooltip = "Reflect amplifies by 10/20/30 percent";
		shieldAim = new UpgradeInfo("Aim\nreflection", 1, 120, shieldAmp, button);
		shieldAim.tooltip = "Allows you to aim where the reflection should go";
		
		shieldCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		shieldCd.tooltip = "Cooldown decreased by 0.5/1/1.5";
		shieldAbsorb = new UpgradeInfo("Absorb", 2, 80, shieldCd, button);
		shieldAbsorb.tooltip = "No longer reflects, instead absorbs and heals for 50/100% of the damage dealt";
		
		shieldAmp.relative = shieldCd;
		shieldCd.relative = shieldAmp;
		#endregion

		#region Invisibility shield
		windShieldDuration = new UpgradeInfo("Duration", 2, 60, null, button);
		windShieldDuration.tooltip = "Increases duration of shield and invisibility by 0.5/1 seconds";
		windShieldDamage = new UpgradeInfo("Damage boost", 1, 120, windShieldDuration, button);
		windShieldDamage.tooltip = "Increases damage of spells cast while invis by 35%";
		
		windShieldCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		windShieldCd.tooltip = "Cooldown decreased by 0.5/1/1.5";
		windShieldInvis = new UpgradeInfo("Instant Invisibility", 1, 120, windShieldInvis, button);
		windShieldInvis.tooltip = "No longer shields, instead becomes instantly invisible and can no longer be hit by spells while invisible";
		
		windShieldCd.relative = windShieldDuration;
		windShieldDuration.relative = windShieldCd;
		#endregion

		#region Placed Shield
		placedShieldAmp = new UpgradeInfo("Damage amplification", 3, 40, null, button);
		placedShieldAmp.tooltip = "Amplifies damage dealt by friendly targets in shield by 10/20/30%";
		placedShieldKnockImmune = new UpgradeInfo("Knockback Immunity", 1, 120, placedShieldCd, button);
		placedShieldKnockImmune.tooltip = "Friendly targets in shield can no longer be knocked";
		
		placedShieldCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		placedShieldCd.tooltip = "Decreases cooldown by 1.5/3/4.5";
		placedShieldSpeed = new UpgradeInfo("Speed boost", 1, 120, placedShieldAmp, button);
		placedShieldSpeed.tooltip = "Increase speed of friendly targets in the sphere by 100%";
		
		placedShieldAmp.relative = placedShieldCd;
		placedShieldCd.relative = placedShieldAmp;
		#endregion

		#region Hook
		hookDmg = new UpgradeInfo("Damage", 3, 40, null, button);
		hookDmg.tooltip = "Increases damage by 1.5/3/4.5";
		hookPull = new UpgradeInfo("Reverse hook", 1, 120, hookDmg, button);
		hookPull.tooltip = "Now pulls the target to you instead of the other way around, increasing damage by 3.5. Damage is dealt instantly";
		
		hookCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		hookCd.tooltip = "Cooldown decreased by 0.5/1/1.5";
		hookInvu = new UpgradeInfo("Invulnerability", 1, 120, hookCd, button);
		hookInvu.tooltip = "Now invulnerable while being pulled";
		
		hookCd.relative = hookDmg;
		hookDmg.relative = hookCd;
		#endregion

		#region Blink
		blinkDmg = new UpgradeInfo("Damage", 3, 40, null, button);
		blinkDmg.tooltip = "Increases damage to 3/6/9";
		blinkThrust = new UpgradeInfo("Thrust", 1, 120, blinkDmg, button);
		blinkThrust.tooltip = "Now rushes forward, no longer invulnerable but knocking back on collision";
		
		blinkCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		blinkCd.tooltip = "Cooldown decreased by 0.5/1/1.5";
		blinkInstant = new UpgradeInfo("Instant", 1, 120, blinkCd, button);
		blinkInstant.tooltip = "Cast time is now instant";
		
		blinkCd.relative = blinkDmg;
		blinkDmg.relative = blinkCd;
		#endregion

		#region Life Grip
		lifeGripCd = new UpgradeInfo("Cooldown", 3, 40, null, button);
		lifeGripCd.tooltip = "Cooldown decreased by 0.5/1/1.5";
		lifeGripShield = new UpgradeInfo("Absorb Shield", 1, 120, lifeGripCd, button);
		lifeGripShield.tooltip = "Gripped allies get a shield that absorbs 20 damage for 3 seconds";
		
		lifeGripCd.relative = bindingLength;
        #endregion

        #region Arcane Bolt
        arcaneBoltDmg = new UpgradeInfo("Damage", 3, 40, null, button);
        arcaneBoltDmg.tooltip = "Hitting an arcane bolt increases the damage for the next arcane bolt by 10/15/20% (this stacks up to two times)";
        arcaneBoltKnock = new UpgradeInfo("Knockback", 1, 120, arcaneBoltDmg, button);
        arcaneBoltKnock.tooltip = "Hitting all three arcane bolts in a row increases the knockback of your next spell by 50%";

        arcaneBoltHeal = new UpgradeInfo("Self heal", 3, 40, null, button);
        arcaneBoltHeal.tooltip = "Hitting an arcane bolt heals you for 3/6/9 health";
        arcaneBoltCd = new UpgradeInfo("Cooldown", 1, 120, arcaneBoltHeal, button);
        arcaneBoltCd.tooltip = "Hitting an arcane bolt lowers all your cooldowns by 10%";

        arcaneBoltDmg.relative = arcaneBoltHeal;
        arcaneBoltHeal.relative = arcaneBoltDmg;
        #endregion

        #region Corrupting Bolt
        corruptingBoltDmgRed = new UpgradeInfo("Damage debuff", 3, 40, null, button);
        corruptingBoltDmgRed.tooltip = "Affected targets deal 10/20/30% less damage and healing";
        corruptingBoltCd = new UpgradeInfo("Cooldown debuff", 1, 120, corruptingBoltDmgRed, button);
        corruptingBoltCd.tooltip = "Affected targets have 20% longer cooldowns";

        corruptingBoltAmplify = new UpgradeInfo("Amplify", 3, 40, null, button);
        corruptingBoltAmplify.tooltip = "Affected targets take 25% more damage from the next 1/2/3 direct damage spells";
        corruptingBoltBlast = new UpgradeInfo("AoE blast", 1, 120, arcaneBoltHeal, button);
        corruptingBoltBlast.tooltip = "Corrupting bolt now explodes in an AoE upon hit, affecting all targets around impact point";

        corruptingBoltAmplify.relative = corruptingBoltDmgRed;
        corruptingBoltDmgRed.relative = corruptingBoltAmplify;
        #endregion

        Invoke("SetSpellCasting", 2);
	}

	void Awake()
	{
		SetShit();
	}

	public void SetSpellCasting()
	{
		spellcastSet = 2;

		//GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		/*
		foreach(GameObject player in ConnectionHandler_2.players)
		{
			names += player.GetComponent<SpellCasting>().playerName + "\n";
			Debug.Log ("Asd!");
			if(player.networkView.isMine)
			{
				spellCasting = player.GetComponent<SpellCasting>();
			}
		}
		*/
		cd = GameObject.Find ("CooldownInfo").GetComponent<CooldownInfo>();

		spells.Add (spellCasting.off1);
		spells.Add (spellCasting.off2);
		spells.Add (spellCasting.off3);
		spells.Add (spellCasting.def);
		spells.Add (spellCasting.mob);
	}

	void OnGUI()
	{
		if(spellcastSet == 1)
		{
			//GUI.Label(new Rect(10, 200, 100, 100), "I HAVE SET MY SPELLCAST");
		}
		if(spellcastSet == 2)
		{
			//GUI.Label(new Rect(10, 200, 100, 100), "I HAVE SET IT FOR REALZ");
		}
		if(GameHandler.state == GameHandler.State.Upgrade)
		{
			//GUI.Label(new Rect(10, 310, 100, 100), names);
			string gold = "No gold";

			gold = spellCasting.gold.ToString();

			goldText.text = "Gold: " + gold;
            //if (GUI.Button(new Rect(140, 10, 20, 20), "GOLDHAXXX"))
            //{
            //    spellCasting.gold += 100;
            //}


            GUI.DrawTexture(new Rect(80, 45, 1120, 630), background);

            System.Type t = this.GetType();
			InvokeMethod (1, "Draw" + spellCasting.off1.spellName, t);
            InvokeMethod (2, "Draw" + spellCasting.off2.spellName, t);
            InvokeMethod (3, "Draw" + spellCasting.off3.spellName, t);
            InvokeMethod (4, "Draw" + spellCasting.def.spellName, t);
            InvokeMethod (5, "Draw" + spellCasting.mob.spellName, t);

			if(GUI.tooltip.Length != 0)
			{
				GUI.Label(new Rect((int)Input.mousePosition.x + 10, Screen.height - (int)Input.mousePosition.y, 150, 150), background);

				//GUI.Label(new Rect((int)Input.mousePosition.x + 14, Screen.height - (int)Input.mousePosition.y + 4, 150, 150), GUI.tooltip, tooltip);
				DrawOutline(new Rect((int)Input.mousePosition.x + 20, Screen.height - (int)Input.mousePosition.y + 8, 135, 135), GUI.tooltip, tooltip, Color.black);
            }
            //GUI.Label(new Rect(700, 400, 100, 20), "Gold: " + gold);
        }
        else
        {
            goldText.text = "";
        }

	}

    void InvokeMethod(int argument, string name, System.Type type)
    {
        MethodInfo method = type.GetMethod(name);
        object[] parameters = new object[] { (object)argument };
        method.Invoke(this, parameters);
    }

    public static void DrawOutline(Rect pos, string text, GUIStyle style, Color color)
	{
		Color oldColor = style.normal.textColor;
		style.normal.textColor = color;
		pos.x--;
		GUI.Label(pos, text, style);
		pos.x += 2;
		GUI.Label(pos, text, style);
		pos.x--;
		pos.y--;
		GUI.Label(pos, text, style);
		pos.y += 2;
		GUI.Label(pos, text, style);
		pos.y--;
		style.normal.textColor = oldColor;
		GUI.Label(pos, text, style);
	}


	Rect CreateRect(int slot)
	{
		Rect rect = new Rect();
		switch(slot)
		{
			case 1:
				rect = new Rect(146, 145, 300, 400);
				break;
			case 2:
				rect = new Rect(506, 145, 300, 400);
				break;
			case 3:
				rect = new Rect(866, 145, 300, 400);
				break;
			case 4:
				rect = new Rect(146, 400, 300, 400);
				break;
			case 5:
				rect = new Rect(506, 400, 300, 400);
				break;
		}
		return rect;
	}

	Vector2 rect1 = new Vector2(30, 30);
	Vector2 rect2 = new Vector2(30, 130);
	Vector2 rect3 = new Vector2(180, 30);
	Vector2 rect4 = new Vector2(180, 130);

	public void DrawFireball(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));
        //GUI.Label(new Rect(130, 0, 100, 30), "Fireball");
        DrawOutline(new Rect(130, 0, 100, 30), "Fireball", "label", Color.black);
		fireballDot.Draw (rect1, spellCasting, "fireballDot");
		fireballFinalBlast.Draw (rect2, spellCasting, "fireballFinalBlast");
		fireballDmg.Draw (rect3, spellCasting, "fireballDmg");
        fireballCd.Draw(rect4, spellCasting, "fireballCd");
		GUI.EndGroup();
	}

	public void DrawHealingWard(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));
        DrawOutline(new Rect(130, 0, 100, 30), "Healing", "label", Color.black);

        healingDuration.Draw (rect1, spellCasting, "healingDuration");
		healingDamageReduct.Draw (rect2, spellCasting, "healingDamageReduct");

		healingBloom.Draw (rect3, spellCasting, "healingBloom");
		healingBurst.Draw (rect4, spellCasting, "healingBurst");
		GUI.EndGroup();
	}

	public void DrawBindingShot(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Binding shot", "label", Color.black);

        bindingDuration.Draw (rect1 + new Vector2(75, 0), spellCasting, "bindingDuration");
		bindingSilence.Draw (rect2, spellCasting, "bindingSilence");
		
		//bindingLength.Draw (new Vector2 (180, 30), spellCasting);
		bindingAmplify.Draw (rect4, spellCasting, "bindingAmplify");
		GUI.EndGroup();
	}

	public void DrawBlink(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Blink", "label", Color.black);

        blinkDmg.Draw (rect1, spellCasting, "blinkDmg");
		blinkThrust.Draw (rect2, spellCasting, "blinkThrust");
		
		if(blinkCd.Draw(rect3, spellCasting, "blinkCd"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "Blink")
				{
					spells[i].spellMaxCd -= 0.5f;
					System.Type t = cd.GetType();
					FieldInfo[] fields = t.GetFields();
					foreach(FieldInfo f in fields)
					{
						if(f.Name.Equals("spell" + i + "MaxCD"))
						{
							f.SetValue(cd, (float)f.GetValue(cd) - 0.5f);
						}
					}
				}
			}
		}

		if(blinkInstant.Draw(rect4, spellCasting, "blinkInstant"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "Blink")
				{
					spells[i].totalCastTime = 0;
					spells[i].castTime = 0;
				}
			}
		}
		GUI.EndGroup();
	}

	public void DrawPrisonCenter(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Frost Prison", "label", Color.black);

        frostPrisonDuration.Draw (rect1, spellCasting, "frostPrisonDuration");
		frostPrisonCircleWall.Draw (rect2, spellCasting, "frostPrisonCircleWall");
		
		frostPrisonRamp.Draw (rect3, spellCasting, "frostPrisonRamp");
		frostPrisonStorm.Draw (rect4, spellCasting, "frostPrisonStorm");
		GUI.EndGroup();
	}

	public void DrawHook(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Hook", "label", Color.black);

        hookDmg.Draw (rect1, spellCasting, "hookDmg");
		hookPull.Draw (rect2, spellCasting, "hookPull");
		
		if(hookCd.Draw(rect3, spellCasting, "hookCd"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "Hook")
				{
					spells[i].spellMaxCd -= 0.5f;
					System.Type t = cd.GetType();
					FieldInfo[] fields = t.GetFields();
					foreach(FieldInfo f in fields)
					{
						if(f.Name.Equals("spell" + i + "MaxCD"))
						{
							f.SetValue(cd, (float)f.GetValue(cd) - 0.5f);
						}
					}
				}
			}
		}
		hookInvu.Draw (rect4, spellCasting, "hookInvu");
		GUI.EndGroup();
	}

	public void DrawMagmaBlastAlternative(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Magma Blast", "label", Color.black);

  //      if (magmaBlastCd.Draw(rect1, spellCasting, "magmaBlastCd"))
		//{
		//	for (int i = 0; i < spells.Count; i++)
		//	{
		//		if(spells[i].spellName == "MagmaBlastAlternative")
		//		{
		//			spells[i].spellMaxCd -= 0.35f;
		//			System.Type t = cd.GetType();
		//			FieldInfo[] fields = t.GetFields();
		//			foreach(FieldInfo f in fields)
		//			{
		//				if(f.Name.Equals("spell" + i + "MaxCD"))
		//				{
		//					f.SetValue(cd, (float)f.GetValue(cd) - 0.35f);
		//				}
		//			}
		//		}
		//	}
		//}
		//if(magmaBlastBlackhole.Draw (rect2, spellCasting, "magmaBlastBlackhole"))
		//{
		//	for (int i = 0; i < spells.Count; i++)
		//	{
		//		if(spells[i].spellName == "MagmaBlastAlternative")
		//		{
		//			spells[i].totalCastTime = 0.3f;
		//			spells[i].castTime = 0.3f;
		//		}
		//	}
		//}
		
		magmaBlastDmg.Draw (rect3, spellCasting, "magmaBlastDmg");
		magmaBlastAmplify.Draw (rect4, spellCasting, "magmaBlastAmplify");
		GUI.EndGroup();
	}

	public void DrawNewShield(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Reflect Shield", "label", Color.black);

        shieldAmp.Draw (rect1, spellCasting, "shieldAmp");
		shieldAim.Draw (rect2, spellCasting, "shieldAim");
		
		if(shieldCd.Draw (rect3, spellCasting, "shieldCd"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "NewShield")
				{
					spells[i].spellMaxCd -= 0.5f;
					System.Type t = cd.GetType();
					FieldInfo[] fields = t.GetFields();
					foreach(FieldInfo f in fields)
					{
						if(f.Name.Equals("spell" + i + "MaxCD"))
						{
							f.SetValue(cd, (float)f.GetValue(cd) - 0.5f);
						}
					}
				}
			}
		}
		shieldAbsorb.Draw (rect4, spellCasting, "shieldAbsorb");
		GUI.EndGroup();
	}

	public void DrawWindWalkShield(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Invisibility Shield", "label", Color.black);

        windShieldDuration.Draw (rect1, spellCasting, "windShieldDuration");
		windShieldDamage.Draw (rect2, spellCasting, "windShieldDamage");
		
		if(windShieldCd.Draw (rect3, spellCasting, "windShieldCd"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "WindWalkShield")
				{
					spells[i].spellMaxCd -= 0.5f;
					System.Type t = cd.GetType();
					FieldInfo[] fields = t.GetFields();
					foreach(FieldInfo f in fields)
					{
						if(f.Name.Equals("spell" + i + "MaxCD"))
						{
							f.SetValue(cd, (float)f.GetValue(cd) - 0.5f);
						}
					}
				}
			}
		}
		windShieldInvis.Draw (rect4, spellCasting, "windShieldInvis");
		GUI.EndGroup();
	}

	public void DrawPlacedShield(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Placed Shield", "label", Color.black);

        placedShieldAmp.Draw (rect3, spellCasting, "placedShieldAmp");
		placedShieldKnockImmune.Draw (rect2, spellCasting, "placedShieldKnockImmune");
		
		if(placedShieldCd.Draw (rect1, spellCasting, "placedShieldCd"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "PlacedShield")
				{
					spells[i].spellMaxCd -= 1.5f;
					System.Type t = cd.GetType();
					FieldInfo[] fields = t.GetFields();
					foreach(FieldInfo f in fields)
					{
						if(f.Name.Equals("spell" + i + "MaxCD"))
						{
							f.SetValue(cd, (float)f.GetValue(cd) - 1.5f);
						}
					}
				}
			}
		}
		placedShieldSpeed.Draw (rect4, spellCasting, "placedShieldSpeed");
		GUI.EndGroup();
	}

	public void DrawLifeGrip(int slot)
	{
		GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Life Grip", "label", Color.black);

        if (lifeGripCd.Draw (rect1, spellCasting, "lifeGripCd"))
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if(spells[i].spellName == "LifeGrip")
				{
					spells[i].spellMaxCd -= 0.5f;
					System.Type t = cd.GetType();
					FieldInfo[] fields = t.GetFields();
					foreach(FieldInfo f in fields)
					{
						if(f.Name.Equals("spell" + i + "MaxCD"))
						{
							f.SetValue(cd, (float)f.GetValue(cd) - 1.5f);
						}
					}
				}
			}
		}
		lifeGripShield.Draw (rect2, spellCasting, "lifeGripShield");
		GUI.EndGroup();
	}

    public void DrawArcaneBolt(int slot)
    {
        GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Arcane Bolt", "label", Color.black);

        arcaneBoltDmg.Draw(rect1, spellCasting, "arcaneBoltDmg");
        arcaneBoltKnock.Draw(rect2, spellCasting, "arcaneBoltKnock");

        arcaneBoltHeal.Draw(rect3, spellCasting, "arcaneBoltHeal");
        arcaneBoltCd.Draw(rect4, spellCasting, "arcaneBoltCd");
        GUI.EndGroup();
    }


    public void DrawCorruptingBolt(int slot)
    {
        GUI.BeginGroup(CreateRect(slot));

        DrawOutline(new Rect(130, 0, 100, 30), "Corrupting Bolt", "label", Color.black);

        corruptingBoltDmgRed.Draw(rect1, spellCasting, "corruptingBoltDmgRed");
        corruptingBoltCd.Draw(rect2, spellCasting, "corruptingBoltCd");

        corruptingBoltAmplify.Draw(rect3, spellCasting, "corruptingBoltAmplify");
        corruptingBoltBlast.Draw(rect4, spellCasting, "corruptingBoltBlast");
        GUI.EndGroup();
    }

}

public class UpgradeInfo
{
	public string upgradeName;
	public string tooltip = "No tooltip found";
	public int currentLevel = 0;
	public int maxLevel;
	public int cost;
	public UpgradeInfo preReq;
	public UpgradeInfo relative;

	GUIStyle button;

	public UpgradeInfo(string name, int max, int _cost, UpgradeInfo pre, GUIStyle _button)
	{
		upgradeName = name;
		maxLevel = max;
		preReq = pre;
		cost = _cost;
		button = _button;
	}

	public bool Draw(Vector2 offset, SpellCasting spellCasting, string thisName)
	{
		bool val = false;
		if(GUI.Button(new Rect((int)offset.x, (int)offset.y, 80, 80), new GUIContent("", tooltip)))
		{
			val = Upgrade(spellCasting, thisName);
		}
		Upgrading.DrawOutline (new Rect ((int)offset.x + 20, (int)offset.y + 20, 80, 80), upgradeName, "label", Color.black);
		Upgrading.DrawOutline (new Rect(offset.x + 10, offset.y + 5, 30, 30), cost.ToString(), "label", Color.black);
		Upgrading.DrawOutline (new Rect(offset.x + 60, offset.y + 50, 20, 20), currentLevel.ToString() + "/" + maxLevel.ToString(), "label", Color.black);
		return val;
	}

	public bool Upgrade(SpellCasting spellCasting, string thisName)
	{
		if(relative != null)
		{
			if(relative.currentLevel > 0)
			{
				return false;
			}
		}
		if(preReq != null)
		{
			if(preReq.currentLevel == preReq.maxLevel)
			{
				if(currentLevel < maxLevel && spellCasting.gold >= cost)
				{
					currentLevel++;
					spellCasting.gold -= cost;
                    Upgrades up = spellCasting.gameObject.GetComponent<Upgrades>();
                    Type t = typeof(Upgrades);
                    up.InvokeMethod(currentLevel, "Call" + thisName, t);
                    if (currentLevel == 1)
					{
						//GA.API.Design.NewEvent("Upgrade:" + upgradeName);
					}
					return true;
				}
			}
		}
		else if(currentLevel < maxLevel && spellCasting.gold >= cost)
		{
			currentLevel++;
			spellCasting.gold -= cost;
            Upgrades up = spellCasting.gameObject.GetComponent<Upgrades>();
            Type t = typeof(Upgrades);
            up.InvokeMethod(currentLevel, "Call" + thisName, t);
            //Invoke("Cmd" + thisName, 0);
            return true;
		}
		return false;
	}
}