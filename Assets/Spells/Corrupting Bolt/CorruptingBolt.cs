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

    public float amplifyAmount = 1.25f;
    public float longerCdsAmount = 0.2f;
    public float blastAoe = 2.0f;

    private float damageReduc = 0.0f;
    private bool longerCds;

    private int amplifyCount = 0;

    private bool blast;

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

        damageReduc = spell.upgrades.corruptingBoltDmgRed * 0.1f;
        longerCds = spell.upgrades.corruptingBoltCd > 0;
        amplifyCount = spell.upgrades.corruptingBoltAmplify;
        blast = spell.upgrades.corruptingBoltBlast > 0;
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
                    if (blast)
                    {
                        Blast();
                    }
                    else
                    {
                        damageSystem.AddDot(dotDamage, duration + 0.1f, 1.0f, spell.owner, dotEffect);
                        other.GetComponent<SpellCasting>().RpcDamageBoost(1.0f - damageReduc, duration);
                        if (longerCds)
                            other.GetComponent<SpellCasting>().RpcCdSpeed(1.0f - longerCdsAmount, duration);
                        if (amplifyCount > 0)
                            damageSystem.DirectDamageAmp(amplifyAmount, amplifyCount, duration);
                    }
                    GameObject hit = Instantiate(boltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
            }
        }

        if (other.CompareTag("Obstacle"))
        {
            if (blast)
            {
                Blast();
            }
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
                    if (blast)
                    {
                        Blast();
                    }
                    GameObject hit = Instantiate(boltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
            }
        }
    }

    void Blast()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            DamageSystem dmgSys = player.GetComponent<DamageSystem>();
            if (spell.team != dmgSys.Team() && !dmgSys.isDead)
            {
                if (Vector3.Distance(player.transform.position, gameObject.transform.position) < blastAoe)
                {
                    dmgSys.AddDot(dotDamage, duration + 0.1f, 1.0f, spell.owner, dotEffect);
                }
                if (amplifyCount > 0)
                    dmgSys.DirectDamageAmp(amplifyAmount, amplifyCount, duration);
            }
        }
    }
}
