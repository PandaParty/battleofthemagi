using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Fireball : NetworkBehaviour 
{
	public float oldSpeed;
	public float speed = 50;
	public Spell spell;
	public GameObject fireballExplo;
	public AudioClip cast;

	public GameObject burnEffect;

	public float dotDamage;
	public float duration;
    public GameObject minorFireball;

    bool finalBlast;
    
	void Start ()
    {
		oldSpeed = speed;
		spell.SetColor();
		Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
        if(spell.aimDir == null || spell.aimDir == Vector2.zero)
		    spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
        transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
		AudioSource.PlayClipAtPoint(cast, transform.position);

        if (!isServer)
            return;

        spell.Invoke("KillSelf", 5);
        if(spell.upgrades != null)
        {
            IncreaseDot(spell.upgrades.fireballDot);
            finalBlast = spell.upgrades.fireballFinalBlast > 0;
            IncreaseDmg(spell.upgrades.fireballDmg);
            if (spell.upgrades.fireballCd > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    float angle = Mathf.Atan2(spell.aimDir.y, spell.aimDir.x) * Mathf.Rad2Deg;
                    if (i == 0)
                        angle -= 35.0f;
                    else
                        angle += 35.0f;
                    angle *= Mathf.Deg2Rad;
                    Vector3 newAimDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                    GameObject newFireball = Instantiate(minorFireball, transform.position, Quaternion.identity);
                    Spell spellScript = newFireball.GetComponent<Spell>();
                    spellScript.owner = spell.owner;
                    spellScript.team = spell.team;
                    spellScript.aimDir = newAimDir;
                    NetworkServer.Spawn(newFireball);
                }
            }
        }
    }
	
	void Update ()
    {
		transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
	}
    
	void IncreaseDot(int level)
	{
		dotDamage += 0.05f * level;
		duration += 0.5f * level;
        Debug.Log("Dot increased");
	}
    
	void IncreaseDmg(int level)
	{
		spell.damage += 1.0f * level;
		spell.knockFactor += 0.8f * level;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(other.CompareTag("Player"))
		{
            DamageSystem damageSystem = (DamageSystem)other.GetComponent("DamageSystem");
            if (spell.team != damageSystem.Team())
            {
                if (!other.GetComponent<SpellCasting>().isShielding && !other.GetComponent<DamageSystem>().invulnerable)
                {
                    damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
                    damageSystem.AddDot(dotDamage, duration, 0.5f, spell.owner, burnEffect);
                    if (finalBlast)
                    {
                        Debug.Log("Final blast!");
                        damageSystem.AddDot(5, duration + 1, duration, spell.owner, burnEffect);
                    }
                    Destroy(gameObject);
                    GameObject explo = Instantiate(fireballExplo, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(explo);
                }
            }
        }
        else if (other.CompareTag("Spell"))
        {
            Spell otherSpell = (Spell)other.GetComponent("Spell");
            if (spell.team != otherSpell.team && otherSpell.type == Spell.spellType.Projectile)
            {
                if (spell.destroysSpells)
                {
                    GameObject explo = Instantiate(fireballExplo, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(explo);
                    Destroy(other.gameObject);
                }
                if (otherSpell.destroysSpells)
                {
                    GameObject explo = Instantiate(fireballExplo, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(explo);
                    Destroy(gameObject);
                }
            }
        }

        if (other.CompareTag ("Obstacle"))
		{
            other.SendMessage("Damage", spell.damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
            GameObject explo = Instantiate(fireballExplo, transform.position, Quaternion.identity);
            NetworkServer.Spawn(explo);
        }
		
	}
}