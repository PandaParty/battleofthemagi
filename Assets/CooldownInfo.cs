using UnityEngine;
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

	//public dfSprite spell1;
	//public dfSprite spell2;
	//public dfSprite spell3;
	//public dfSprite spell4;
	//public dfSprite spell5;

	//public dfSprite upgrade1;
	//public dfSprite upgrade2;
	//public dfSprite upgrade3;
	//public dfSprite upgrade4;
	//public dfSprite upgrade5;


	public Vector3 player1Pos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetSpell1CD(float cd)
	{
		spell1CD = cd / spell1MaxCD;
	}

	void SetSpell1MaxCD(float cd)
	{
		spell1MaxCD = cd;
	}

	public void SetSpell1(string name)
	{
		spell1Icon = name;
		//spell1.SpriteName = name;
		//upgrade1.SpriteName = name;
	}



	void SetSpell2CD(float cd)
	{
		spell2CD = cd / spell2MaxCD;
	}
	
	void SetSpell2MaxCD(float cd)
	{
		spell2MaxCD = cd;
	}
	
	public void SetSpell2(string name)
	{
		spell2Icon = name;
		//spell2.SpriteName = name;
		//upgrade2.SpriteName = name;
	}



	void SetSpell5CD(float cd)
	{
		spell5CD = cd / spell5MaxCD;
	}
	
	void SetSpell5MaxCD(float cd)
	{
		spell5MaxCD = cd;
	}
	
	public void SetSpell5(string name)
	{
		spell5Icon = name;
		//spell5.SpriteName = name;
		//upgrade5.SpriteName = name;
	}



	void SetSpell4CD(float cd)
	{
		spell4CD = cd / spell4MaxCD;
	}
	
	void SetSpell4MaxCD(float cd)
	{
		spell4MaxCD = cd;
	}
	
	public void SetSpell4(string name)
	{
		spell4Icon = name;
		//spell4.SpriteName = name;
		//upgrade4.SpriteName = name;
	}



	void SetSpell3CD(float cd)
	{
		spell3CD = cd / spell3MaxCD;
	}
	
	void SetSpell3MaxCD(float cd)
	{
		spell3MaxCD = cd;
	}
	
	public void SetSpell3(string name)
	{
		spell3Icon = name;
		//spell3.SpriteName = name;
		//upgrade3.SpriteName = name;
	}


	void UpdatePlayer1Pos(Vector3 pos)
	{
		player1Pos = pos;
	}
}
