using UnityEngine;
using System.Collections;

public class SpellChoices : MonoBehaviour {

	public string mobSpell;
	public string defSpell;
	public string offSpell1;
	public string offSpell2;
	public string offSpell3;

	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnLevelWasLoaded (int level)
	{

		CooldownInfo cooldownInfo = GameObject.Find ("CooldownInfo").GetComponent<CooldownInfo>();
		cooldownInfo.SetSpell1(offSpell1);
		cooldownInfo.SetSpell2(offSpell2);
		cooldownInfo.SetSpell3(offSpell3);
		cooldownInfo.SetSpell4(defSpell);
		cooldownInfo.SetSpell5(mobSpell);

	}
}
