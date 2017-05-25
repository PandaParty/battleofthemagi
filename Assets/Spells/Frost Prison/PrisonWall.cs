using UnityEngine;
using System.Collections;

public class PrisonWall : MonoBehaviour {
	public int health;
	public GameObject wall1;
	public GameObject wall2;
	public GameObject wall3;
	public GameObject wall4;
	public bool reflect = false;
	public int team;

	// Use this for initialization
	void Start () 
	{
		
	}

	// Update is called once per frame
	void Update () 
	{
		
	}

	void Damage(float amount)
	{
		if(amount >= 5)
		{
			health --;
			if(health <= 0)
			{
				GameObject.Destroy(wall1);
				GameObject.Destroy(wall2);
				GameObject.Destroy(wall3);
				GameObject.Destroy(wall4);
				GetComponent<Collider2D>().enabled = false;
			}
		}
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if(reflect)
		{
			if(other.GetComponent<NetworkView>().isMine)
			{
				if(other.CompareTag ("Spell"))
				{
					Spell otherSpell = (Spell)other.GetComponent("Spell");
					if(team != otherSpell.team)
					{
						if(otherSpell.type == Spell.spellType.Projectile)
						{
							Vector3 normal =  Vector3.Normalize(other.transform.position - gameObject.transform.position);
							Vector3 reflected = Vector3.Reflect(otherSpell.aimDir, normal);
							otherSpell.aimDir = reflected;
							otherSpell.team = team;
							//Network.Instantiate(shieldHit, other.transform.position, Quaternion.identity, 0);
						}
					}
				}
			}
		}
	}
}