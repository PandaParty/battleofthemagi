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
    [Command]
    public void CmdfireballFinalBlast(int lvl)
    {
        fireballFinalBlast = lvl;
    }
    [Command]
    public void CmdfireballDmg(int lvl)
    {
        fireballDmg = lvl;
    }
    [Command]
    public void CmdfireballCd(int lvl)
    {
        fireballCd = lvl;
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
    [Command]
    public void CmdhealingDamageReduct(int lvl)
    {
        healingDamageReduct = lvl;
    }
    [Command]
    public void CmdhealingBloom(int lvl)
    {
        healingBloom = lvl;
    }
    [Command]
    public void CmdhealingBurst(int lvl)
    {
        healingBurst = lvl;
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
    [Command]
    public void CmdmagmaBlastDmg(int lvl)
    {
        magmaBlastDmg = lvl;
    }
    [Command]
    public void CmdmagmaBlastAmplify(int lvl)
    {
        magmaBlastAmplify = lvl;
    }

    public int bindingDuration;
    public int bindingSilence;
    public int bindingAmplify;
    [Command]
    public void CmdbindingDuration(int lvl)
    {
        bindingDuration = lvl;
    }
    [Command]
    public void CmdbindingSilence(int lvl)
    {
        bindingSilence = lvl;
    }
    [Command]
    public void CmdbindingAmplify(int lvl)
    {
        bindingAmplify = lvl;
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
    [Command]
    public void CmdfrostPrisonCircleWall(int lvl)
    {
        frostPrisonCircleWall = lvl;
    }
    [Command]
    public void CmdfrostPrisonRamp(int lvl)
    {
        frostPrisonRamp = lvl;
    }
    [Command]
    public void CmdfrostPrisonStorm(int lvl)
    {
        frostPrisonStorm = lvl;
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
    [Command]
    public void CmdshieldAim(int lvl)
    {
        shieldAim = lvl;
    }
    [Command]
    public void CmdshieldCd(int lvl)
    {
        shieldCd = lvl;
    }
    [Command]
    public void CmdshieldAbsorb(int lvl)
    {
        shieldAbsorb = lvl;
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
    [Command]
    public void CmdwindShieldDamage(int lvl)
    {
        windShieldDamage = lvl;
    }
    [Command]
    public void CmdwindShieldCd(int lvl)
    {
        windShieldCd = lvl;
    }
    [Command]
    public void CmdwindShieldInvis(int lvl)
    {
        windShieldInvis = lvl;
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
    [Command]
    public void CmdhookPull(int lvl)
    {
        hookPull = lvl;
    }
    [Command]
    public void CmdhookCd(int lvl)
    {
        hookCd = lvl;
    }
    [Command]
    public void CmdhookInvu(int lvl)
    {
        hookInvu = lvl;
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
    [Command]
    public void CmdblinkThrust(int lvl)
    {
        blinkThrust = lvl;
    }
    [Command]
    public void CmdblinkCd(int lvl)
    {
        blinkCd = lvl;
    }
    [Command]
    public void CmdblinkInstant(int lvl)
    {
        blinkInstant = lvl;
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
    [Command]
    public void CmdplacedShieldAmp(int lvl)
    {
        placedShieldAmp = lvl;
    }
    [Command]
    public void CmdplacedShieldCd(int lvl)
    {
        placedShieldCd = lvl;
    }
    [Command]
    public void CmdplacedShieldSpeed(int lvl)
    {
        placedShieldSpeed = lvl;
    }

    public int lifeGripCd;
    public int lifeGripShield;
    [Command]
    public void CmdlifeGripCd(int lvl)
    {
        lifeGripCd = lvl;
    }
    [Command]
    public void CmdlifeGripShield(int lvl)
    {
        lifeGripShield = lvl;
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void InvokeMethod(int argument, string name, System.Type type)
    {
        MethodInfo method = type.GetMethod(name);
        object[] parameters = new object[] { (object)argument };
        method.Invoke(this, parameters);
    }
}
