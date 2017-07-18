using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CooldownInfo : MonoBehaviour {
	public string spell1Icon;
	public float spell1CD;
	public float spell1MaxCD;

	public string spell2Icon;
	public float spell2CD;
	public float spell2MaxCD;

	public string spell3Icon;
	public float spell3CD;
	public float spell3MaxCD;
	
	public string spell4Icon;
	public float spell4CD;
	public float spell4MaxCD;

	public string spell5Icon;
	public float spell5CD;
	public float spell5MaxCD;

    public Image spell1;
    public Image spell2;
    public Image spell3;
    public Image spell4;
    public Image spell5;

    public Image sp1Cd;
    public Image sp2Cd;
    public Image sp3Cd;
    public Image sp4Cd;
    public Image sp5Cd;

    public Sprite fireball;
    public Sprite arcaneBolt;
    public Sprite bindingShot;
    public Sprite corruptingBolt;
    public Sprite blink;
    public Sprite healingWard;
    public Sprite hook;
    public Sprite lifeGrip;
    public Sprite magmaBlast;
    public Sprite newShield;
    public Sprite placedShield;
    public Sprite frostPrison;
    public Sprite windWalkShield;
    

    //public dfSprite upgrade1;
    //public dfSprite upgrade2;
    //public dfSprite upgrade3;
    //public dfSprite upgrade4;
    //public dfSprite upgrade5;


    public Vector3 player1Pos;

	void SetSpell1CD(float cd)
	{
		spell1CD = cd / spell1MaxCD;
        sp1Cd.fillAmount = spell1CD;
	}

	void SetSpell1MaxCD(float cd)
	{
		spell1MaxCD = cd;
	}

	public void SetSpell1(string name)
	{
		spell1Icon = name;
        spell1.sprite = LoadSpriteFromName(name);
		//spell1.SpriteName = name;
		//upgrade1.SpriteName = name;
	}



	void SetSpell2CD(float cd)
	{
		spell2CD = cd / spell2MaxCD;
        sp2Cd.fillAmount = spell2CD;
    }
	
	void SetSpell2MaxCD(float cd)
	{
		spell2MaxCD = cd;
	}
	
	public void SetSpell2(string name)
	{
		spell2Icon = name;
        spell2.sprite = LoadSpriteFromName(name);
        //spell2.SpriteName = name;
        //upgrade2.SpriteName = name;
    }



	void SetSpell5CD(float cd)
	{
		spell5CD = cd / spell5MaxCD;
        sp5Cd.fillAmount = spell5CD;
    }
	
	void SetSpell5MaxCD(float cd)
	{
		spell5MaxCD = cd;
	}
	
	public void SetSpell5(string name)
	{
		spell5Icon = name;
        spell5.sprite = LoadSpriteFromName(name);
        //spell5.SpriteName = name;
        //upgrade5.SpriteName = name;
    }



	void SetSpell4CD(float cd)
	{
		spell4CD = cd / spell4MaxCD;
        sp4Cd.fillAmount = spell4CD;
    }
	
	void SetSpell4MaxCD(float cd)
	{
		spell4MaxCD = cd;
	}
	
	public void SetSpell4(string name)
	{
		spell4Icon = name;
        spell4.sprite = LoadSpriteFromName(name);
        //spell4.SpriteName = name;
        //upgrade4.SpriteName = name;
    }



	void SetSpell3CD(float cd)
	{
		spell3CD = cd / spell3MaxCD;
        sp3Cd.fillAmount = spell3CD;
    }
	
	void SetSpell3MaxCD(float cd)
	{
		spell3MaxCD = cd;
	}
	
	public void SetSpell3(string name)
	{
		spell3Icon = name;
        spell3.sprite = LoadSpriteFromName(name);
        //spell3.SpriteName = name;
        //upgrade3.SpriteName = name;
    }

    Sprite LoadSpriteFromName(string name)
    {
        switch(name)
        {
            case "Fireball":
                return fireball;
            case "ArcaneBolt":
                return arcaneBolt;
            case "CorruptingBolt":
                return corruptingBolt;
            case "Blink":
                return blink;
            case "BindingShot":
                return bindingShot;
            case "HealingWard":
                return healingWard;
            case "Hook":
                return hook;
            case "LifeGrip":
                return lifeGrip;
            case "MagmaBlastAlternative":
                return magmaBlast;
            case "NewShield":
                return newShield;
            case "WindWalkShield":
                return windWalkShield;
            case "PlacedShield":
                return placedShield;
            case "PrisonCenter":
                return frostPrison;
            default:
                return fireball;
        }
    }

	void UpdatePlayer1Pos(Vector3 pos)
	{
		player1Pos = pos;
	}
}
