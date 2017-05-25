using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour {
	public float castTime = 0.6f;
	public float cooldown = 1.5f;
	public float damage;
	public float knockFactor;
	public int team;
	public string owner;
	public enum spellType { Projectile, Area, Mimic, Other, TargetArea };
	public spellType type;
	public Vector2 aimPoint;
	public bool destroysSpells;
	public Vector2 aimDir;
	
	void Start()
	{
		//Invoke ("KillSelf", 5);
	}

	public void KillSelf()
	{
		Destroy (gameObject);
	}

	public void SetColor()
	{
		switch(team)
		{
		case 1:
			gameObject.GetComponent<ParticleSystem>().startColor = new Color(0.19f, 0.57f, 0.156f);
			break;
		case 2:
			gameObject.GetComponent<ParticleSystem>().startColor = new Color(0.28f, 0.77f, 0.84f);
			break;
		}
	}
	
	[RPC]
	void SetAim(float x, float y, int team, float amplifyAmount, Vector3 pos)
	{
		Debug.Log ("Set aim!");
		aimDir = new Vector2(x, y);
		team = team;
		transform.position = pos;
		//spell.damage *
	}
}
