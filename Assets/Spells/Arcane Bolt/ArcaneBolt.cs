using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArcaneBolt : NetworkBehaviour
{
    public float oldSpeed;
    public float speed = 50;

    public Spell spell;
    public GameObject arcaneBoltHit;
    public AudioClip cast;

    public Material fireMat;
    public Material iceMat;

    private float arcaneBoltDmgBoost = 1;
    private bool arcaneBoltKnockBoost;
    private float arcaneBoltHeal = 0;
    private bool arcaneBoltCd;

    void Start ()
    {
        oldSpeed = speed;
        Vector2 aimPos = ((Spell)gameObject.GetComponent("Spell")).aimPoint;
        spell.aimDir = Vector3.Normalize(new Vector3(aimPos.x, aimPos.y) - transform.position);
        transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
        AudioSource.PlayClipAtPoint(cast, transform.position);
        SetColor();
        if (!isServer)
            return;

        spell.Invoke("KillSelf", 5);
        IncreaseDmgBoost(spell.upgrades.arcaneBoltDmg);
        if (spell.upgrades.arcaneBoltKnock > 0)
            arcaneBoltKnockBoost = true;
        arcaneBoltHeal = spell.upgrades.arcaneBoltHeal * 3;
        if (spell.upgrades.arcaneBoltCd > 0)
            arcaneBoltCd = true;
        
    }
	
    void SetColor()
    {
        switch(spell.team)
        {
            case 1:
                gameObject.GetComponent<TrailRenderer>().material = fireMat;
                break;
            case 2:
                gameObject.GetComponent<TrailRenderer>().material = iceMat;
                break;
        }
    }

	void Update ()
    {
        transform.position += new Vector3(spell.aimDir.x, spell.aimDir.y) / GlobalConstants.unitScaling * speed * Time.deltaTime * 60;
    }

    void IncreaseDmgBoost(int level)
    {
        if (level > 0)
            arcaneBoltDmgBoost = 1.05f + 0.05f * level;
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
                    int boltHits = 0;
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players)
                    {
                        if (player.GetComponent<SpellCasting>().playerName == spell.owner)
                        {
                            boltHits = player.GetComponent<SpellCasting>().arcaneBoltResets;
                            player.GetComponent<SpellCasting>().ArcaneBoltHit(arcaneBoltKnockBoost, arcaneBoltCd);
                            player.GetComponent<DamageSystem>().Damage(-arcaneBoltHeal, 0, player.transform.position, player.GetComponent<SpellCasting>().playerName);
                        }
                    }
                    float dmgBoost = Mathf.Pow(arcaneBoltDmgBoost, boltHits);
                    damageSystem.Damage(spell.damage * dmgBoost, spell.knockFactor * dmgBoost, transform.position, spell.owner);

                    Destroy(gameObject);
                    GameObject hit = Instantiate(arcaneBoltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
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
                    GameObject hit = Instantiate(arcaneBoltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(other.gameObject);
                }
                if (otherSpell.destroysSpells)
                {
                    GameObject hit = Instantiate(arcaneBoltHit, transform.position, Quaternion.identity);
                    NetworkServer.Spawn(hit);
                    Destroy(gameObject);
                }
            }
        }

        if (other.CompareTag("Obstacle"))
        {
            other.SendMessage("Damage", spell.damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
            GameObject hit = Instantiate(arcaneBoltHit, transform.position, Quaternion.identity);
            NetworkServer.Spawn(hit);
        }
        
    }
}
