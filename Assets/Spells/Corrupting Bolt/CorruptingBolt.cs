using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CorruptingBolt : NetworkBehaviour
{
    public float oldSpeed;
    public float speed = 50;
    public Spell spell;
    public GameObject boltHit;
    public AudioClip cast;

    public GameObject dotEffect;

    public float dotDamage;
    public float duration;

    void Start()
    {
        oldSpeed = speed;
        //spell.SetColor();
        Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
        spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
        transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
        AudioSource.PlayClipAtPoint(cast, transform.position);

        if (!isServer)
            return;

        spell.Invoke("KillSelf", 5);
        //IncreaseDot(spell.upgrades.fireballDot);
        //if (spell.upgrades.fireballFinalBlast > 0)
        //{
        //    ActivateFinalBlast();
        //}
        //IncreaseDmg(spell.upgrades.fireballDmg);
    }

    void Update()
    {
        transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
            return;

        if (other.CompareTag("Player"))
        {
            DamageSystem damageSystem = (DamageSystem)other.GetComponent("DamageSystem");
            if (spell.team != damageSystem.Team())
            {
                if (!other.GetComponent<SpellCasting>().isShielding && !other.GetComponent<DamageSystem>().invulnerable)
                {
                    damageSystem.AddDot(dotDamage, duration + 0.1f, 1.0f, spell.owner, dotEffect);
                    
                    GameObject hit = Instantiate(boltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
            }
        }

        if (other.CompareTag("Obstacle"))
        {
            GameObject hit = Instantiate(boltHit, transform.position, Quaternion.identity);
            NetworkServer.Spawn(hit);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Spell"))
        {
            Spell otherSpell = (Spell)other.GetComponent("Spell");
            if (spell.team != otherSpell.team && otherSpell.type == Spell.spellType.Projectile)
            {
                if (spell.destroysSpells)
                {
                    GameObject hit = Instantiate(boltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(other.gameObject);
                }
                if (otherSpell.destroysSpells)
                {
                    GameObject hit = Instantiate(boltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
            }
        }
    }
}
