using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class Upgrades : NetworkBehaviour
{
    public int fireballDot;
    public int fireballFinalBlast;
    public int fireballDmg;
    public int fireballCd;

    [Command]
    public void CmdfireballDot(int lvl)
    {
        fireballDot = lvl;
    }
    public void CallfireballDot(int lvl)
    {
        CmdfireballDot(lvl);
    }
    [Command]
    public void CmdfireballFinalBlast(int lvl)
    {
        fireballFinalBlast = lvl;
    }
    public void CallfireballFinalBlast(int lvl)
    {
        CmdfireballFinalBlast(lvl);
    }
    [Command]
    public void CmdfireballDmg(int lvl)
    {
        fireballDmg = lvl;
    }
    public void CallfireballDmg(int lvl)
    {
        CmdfireballDmg(lvl);
    }
    [Command]
    public void CmdfireballCd(int lvl)
    {
        fireballCd = lvl;
    }
    public void CallfireballCd(int lvl)
    {
        CmdfireballCd(lvl);
    }


    public int healingDuration;
    public int healingDamageReduct;
    public int healingBloom;
    public int healingBurst;

    [Command]
    public void CmdhealingDuration(int lvl)
    {
        healingDuration = lvl;
    }
    public void CallhealingDuration(int lvl)
    {
        CmdhealingDuration(lvl);
    }
    [Command]
    public void CmdhealingDamageReduct(int lvl)
    {
        healingDamageReduct = lvl;
    }
    public void CallhealingDamageReduct(int lvl)
    {
        CmdhealingDamageReduct(lvl);
    }
    [Command]
    public void CmdhealingBloom(int lvl)
    {
        healingBloom = lvl;
    }
    public void CallhealingBloom(int lvl)
    {
        CmdhealingBloom(lvl);
    }
    [Command]
    public void CmdhealingBurst(int lvl)
    {
        healingBurst = lvl;
    }
    public void CallhealingBurst(int lvl)
    {
        CmdhealingBurst(lvl);
    }


    public int magmaBlastCd;
    public int magmaBlastSelfDispel;
    public int magmaBlastDmg;
    public int magmaBlastAmplify;
    [Command]
    public void CmdmagmaBlastCd(int lvl)
    {
        magmaBlastCd = lvl;
    }
    public void CallmagmaBlastCd(int lvl)
    {
        CmdmagmaBlastCd(lvl);
    }
    [Command]
    public void CmdmagmaBlastDmg(int lvl)
    {
        magmaBlastDmg = lvl;
    }
    public void CallmagmaBlastDmg(int lvl)
    {
        CmdmagmaBlastDmg(lvl);
    }
    [Command]
    public void CmdmagmaBlastAmplify(int lvl)
    {
        magmaBlastAmplify = lvl;
    }
    public void CallmagmaBlastAmplify(int lvl)
    {
        CmdmagmaBlastAmplify(lvl);
    }

    public int bindingDuration;
    public int bindingSilence;
    public int bindingAmplify;
    [Command]
    public void CmdbindingDuration(int lvl)
    {
        bindingDuration = lvl;
    }
    public void CallbindingDuration(int lvl)
    {
        CmdbindingDuration(lvl);
    }
    [Command]
    public void CmdbindingSilence(int lvl)
    {
        bindingSilence = lvl;
    }
    public void CallbindingSilence(int lvl)
    {
        CmdbindingSilence(lvl);
    }
    [Command]
    public void CmdbindingAmplify(int lvl)
    {
        bindingAmplify = lvl;
    }
    public void CallbindingAmplify(int lvl)
    {
        CmdbindingAmplify(lvl);
    }

    public int frostPrisonDuration;
    public int frostPrisonCircleWall;
    public int frostPrisonRamp;
    public int frostPrisonStorm;
    [Command]
    public void CmdfrostPrisonDuration(int lvl)
    {
        frostPrisonDuration = lvl;
    }
    public void CallfrostPrisonDuration(int lvl)
    {
        CmdfrostPrisonDuration(lvl);
    }
    [Command]
    public void CmdfrostPrisonCircleWall(int lvl)
    {
        frostPrisonCircleWall = lvl;
    }
    public void CallfrostPrisonCircleWall(int lvl)
    {
        CmdfrostPrisonCircleWall(lvl);
    }
    [Command]
    public void CmdfrostPrisonRamp(int lvl)
    {
        frostPrisonRamp = lvl;
    }
    public void CallfrostPrisonRamp(int lvl)
    {
        CmdfrostPrisonRamp(lvl);
    }
    [Command]
    public void CmdfrostPrisonStorm(int lvl)
    {
        frostPrisonStorm = lvl;
    }
    public void CallfrostPrisonStorm(int lvl)
    {
        CmdfrostPrisonStorm(lvl);
    }

    public int shieldAmp;
    public int shieldAim;
    public int shieldCd;
    public int shieldAbsorb;
    [Command]
    public void CmdshieldAmp(int lvl)
    {
        shieldAmp = lvl;
    }
    public void CallshieldAmp(int lvl)
    {
        CmdshieldAmp(lvl);
    }
    [Command]
    public void CmdshieldAim(int lvl)
    {
        shieldAim = lvl;
    }
    public void CallshieldAim(int lvl)
    {
        CmdshieldAim(lvl);
    }
    [Command]
    public void CmdshieldCd(int lvl)
    {
        shieldCd = lvl;
    }
    public void CallshieldCd(int lvl)
    {
        CmdshieldCd(lvl);
    }
    [Command]
    public void CmdshieldAbsorb(int lvl)
    {
        shieldAbsorb = lvl;
    }
    public void CallshieldAbsorb(int lvl)
    {
        CmdshieldAbsorb(lvl);
    }

    public int windShieldDuration;
    public int windShieldDamage;
    public int windShieldCd;
    public int windShieldInvis;
    [Command]
    public void CmdwindShieldDuration(int lvl)
    {
        windShieldDuration = lvl;
    }
    public void CallwindShieldDuration(int lvl)
    {
        CmdwindShieldDuration(lvl);
    }
    [Command]
    public void CmdwindShieldDamage(int lvl)
    {
        windShieldDamage = lvl;
    }
    public void CallwindShieldDamage(int lvl)
    {
        CmdwindShieldDamage(lvl);
    }
    [Command]
    public void CmdwindShieldCd(int lvl)
    {
        windShieldCd = lvl;
    }
    public void CallwindShieldCd(int lvl)
    {
        CmdwindShieldCd(lvl);
    }
    [Command]
    public void CmdwindShieldInvis(int lvl)
    {
        windShieldInvis = lvl;
    }
    public void CallwindShieldInvis(int lvl)
    {
        CmdwindShieldInvis(lvl);
    }

    public int hookDmg;
    public int hookPull;
    public int hookCd;
    public int hookInvu;
    [Command]
    public void CmdhookDmg(int lvl)
    {
        hookDmg = lvl;
    }
    public void CallhookDmg(int lvl)
    {
        CmdhookDmg(lvl);
    }
    [Command]
    public void CmdhookPull(int lvl)
    {
        hookPull = lvl;
    }
    public void CallhookPull(int lvl)
    {
        CmdhookPull(lvl);
    }
    [Command]
    public void CmdhookCd(int lvl)
    {
        hookCd = lvl;
    }
    public void CallhookCd(int lvl)
    {
        CmdhookCd(lvl);
    }
    [Command]
    public void CmdhookInvu(int lvl)
    {
        hookInvu = lvl;
    }
    public void CallhookInvu(int lvl)
    {
        CmdhookInvu(lvl);
    }

    public int blinkDmg;
    public int blinkThrust;
    public int blinkCd;
    public int blinkInstant;
    [Command]
    public void CmdblinkDmg(int lvl)
    {
        blinkDmg = lvl;
    }
    public void CallblinkDmg(int lvl)
    {
        CmdblinkDmg(lvl);
    }
    [Command]
    public void CmdblinkThrust(int lvl)
    {
        blinkThrust = lvl;
    }
    public void CallblinkThrust(int lvl)
    {
        CmdblinkThrust(lvl);
    }
    [Command]
    public void CmdblinkCd(int lvl)
    {
        blinkCd = lvl;
    }
    public void CallblinkCd(int lvl)
    {
        CmdblinkCd(lvl);
    }
    [Command]
    public void CmdblinkInstant(int lvl)
    {
        blinkInstant = lvl;
    }
    public void CallblinkInstant(int lvl)
    {
        CmdblinkInstant(lvl);
    }

    public int placedShieldKnockImmune;
    public int placedShieldAmp;
    public int placedShieldCd;
    public int placedShieldSpeed;
    [Command]
    public void CmdplacedShieldKnockImmune(int lvl)
    {
        placedShieldKnockImmune = lvl;
    }
    public void CallplacedShieldKnockImmune(int lvl)
    {
        CmdplacedShieldKnockImmune(lvl);
    }
    [Command]
    public void CmdplacedShieldAmp(int lvl)
    {
        placedShieldAmp = lvl;
    }
    public void CallplacedShieldAmp(int lvl)
    {
        CmdplacedShieldAmp(lvl);
    }
    [Command]
    public void CmdplacedShieldCd(int lvl)
    {
        placedShieldCd = lvl;
    }
    public void CallplacedShieldCd(int lvl)
    {
        CmdplacedShieldCd(lvl);
    }
    [Command]
    public void CmdplacedShieldSpeed(int lvl)
    {
        placedShieldSpeed = lvl;
    }
    public void CallplacedShieldSpeed(int lvl)
    {
        CmdplacedShieldSpeed(lvl);
    }

    public int lifeGripCd;
    public int lifeGripShield;
    [Command]
    public void CmdlifeGripCd(int lvl)
    {
        lifeGripCd = lvl;
    }
    public void CalllifeGripCd(int lvl)
    {
        CmdlifeGripCd(lvl);
    }
    [Command]
    public void CmdlifeGripShield(int lvl)
    {
        lifeGripShield = lvl;
    }
    public void CalllifeGripShield(int lvl)
    {
        CmdlifeGripShield(lvl);
    }

    public int arcaneBoltDmg;
    public int arcaneBoltKnock;
    public int arcaneBoltHeal;
    public int arcaneBoltCd;

    [Command]
    public void CmdarcaneBoltDmg(int lvl)
    {
        arcaneBoltDmg = lvl;
    }
    public void CallarcaneBoltDmg(int lvl)
    {
        CmdarcaneBoltDmg(lvl);
    }
    [Command]
    public void CmdarcaneBoltKnock(int lvl)
    {
        arcaneBoltKnock = lvl;
    }
    public void CallarcaneBoltKnock(int lvl)
    {
        CmdarcaneBoltKnock(lvl);
    }
    [Command]
    public void CmdarcaneBoltHeal(int lvl)
    {
        arcaneBoltHeal = lvl;
    }
    public void CallarcaneBoltHeal(int lvl)
    {
        CmdarcaneBoltHeal(lvl);
    }
    [Command]
    public void CmdarcaneBoltCd(int lvl)
    {
        arcaneBoltCd = lvl;
    }
    public void CallarcaneBoltCd(int lvl)
    {
        CmdarcaneBoltCd(lvl);
    }


    public int corruptingBoltDmgRed;
    public int corruptingBoltCd;
    public int corruptingBoltAmplify;
    public int corruptingBoltBlast;

    [Command]
    public void CmdcorruptingBoltDmgRed(int lvl)
    {
        corruptingBoltDmgRed = lvl;
    }
    public void CallcorruptingBoltDmgRed(int lvl)
    {
        CmdcorruptingBoltDmgRed(lvl);
    }
    [Command]
    public void CmdcorruptingBoltCd(int lvl)
    {
        corruptingBoltCd = lvl;
    }
    public void CallcorruptingBoltCd(int lvl)
    {
        CmdcorruptingBoltCd(lvl);
    }
    [Command]
    public void CmdcorruptingBoltAmplify(int lvl)
    {
        corruptingBoltAmplify = lvl;
    }
    public void CallcorruptingBoltAmplify(int lvl)
    {
        CmdcorruptingBoltAmplify(lvl);
    }
    [Command]
    public void CmdcorruptingBoltBlast(int lvl)
    {
        corruptingBoltBlast = lvl;
    }
    public void CallcorruptingBoltBlast(int lvl)
    {
        CmdcorruptingBoltBlast(lvl);
    }


    public void InvokeMethod(int argument, string name, System.Type type)
    {
        MethodInfo method = type.GetMethod(name);
        object[] parameters = new object[] { (object)argument };
        method.Invoke(this, parameters);
    }
}
