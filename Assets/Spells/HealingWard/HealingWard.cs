using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HealingWard : NetworkBehaviour {
	public Spell spell;
	public float duration;
	public float bloomTime;
	
	public AudioClip cast;

	public GameObject effect;

	bool dispels = false;
	bool dispelHeals = false;
	float damages = 0;
	bool lifeSteals = false;
	bool damageReduct = false;
	bool instant = false;
    
	void Start ()
    {
		spell.SetColor();
		transform.position = spell.aimPoint;
		transform.position += new Vector3(0, 0, 1);
		AudioSource.PlayClipAtPoint(cast, transform.position);
		Invoke ("Bloom", bloomTime);

        Duration(spell.upgrades.healingDuration);
        if (spell.upgrades.healingDamageReduct > 0)
            DamageReduct();

        if(spell.upgrades.healingBloom > 0)
            BloomTime(spell.upgrades.healingBloom);
        if (spell.upgrades.healingBurst > 0)
            Instant();
	}

	void SetColor()
	{
		switch(spell.team)
		{
			case 2:
				gameObject.GetComponent<ParticleSystem>().startColor = new Color(0f, 1f, 1f);
				ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem system in systems)
				{
					if(system.gameObject.name == "CFX_CCW")
					{
						system.startColor = new Color(0.27f, 1f, 1f);
					}
				}
			break;
		}
	}
    
	void Duration(int level)
	{
		duration += 0.7f * level;
	}
    
	void DamageReduct()
	{
		damageReduct = true;
	}
    
	void BloomTime(int level)
	{
		bloomTime -= 0.2f * level;
		gameObject.GetComponent<ParticleSystem>().time = 0.7f - bloomTime;
	}
    
	void Instant()
	{
		instant = true;
	}

	void Bloom()
	{
        if (!isServer)
            return;

		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, ((CircleCollider2D)GetComponent<Collider2D>()).radius);

		foreach(Collider2D hitCollider in hitColliders)
		{
			if(hitCollider.CompareTag("Player"))
			{
				if(hitCollider.gameObject.GetComponent<SpellCasting>().team == spell.team)
				{
					if(!instant)
					{
						hitCollider.GetComponent<DamageSystem>().AddHot (spell.damage, duration, 0.0165f, effect);
						if(damageReduct)
						{
							hitCollider.GetComponent<DamageSystem>().Amplify(-0.5f, duration);
						}
					}
					else
					{
						hitCollider.GetComponent<DamageSystem>().Damage(-32f, 0, transform.position, spell.owner);
					}
				}
			}
		}
		spell.Invoke("KillSelf", bloomTime + 0.4f);
	}

	void OnTriggerStay2D(Collider2D other)
	{
		/*
		if(other.CompareTag("Player"))
		{
			DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
			if(spell.team == damageSystem.Team())
			{
				if(other.networkView.isMine)
				{
					damageSystem.Damage(-spell.damage, spell.knockFactor, transform.position);
				}
			}
		}
		*/
	}
}
