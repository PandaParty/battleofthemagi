using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FrostPrison : NetworkBehaviour {
	public Spell spell;
	public float duration;
	public float formTime;
	
	public AudioClip cast;
	public AudioClip form;

	public GameObject prison;
	public GameObject extraWall;
	public GameObject wall1;
	public GameObject wall2;
	public GameObject wall3;
	public GameObject wall4;
	public GameObject spawn;
	public GameObject extraSpawn;
	public GameObject storm;

	float currentTime;
	float baseDamage = 0.45f;

	bool formed;

	bool stormActive;
	bool reflects;


	float totalDamage = 0;

	bool circleWall = false;
    
	void Start ()
    {
		SetColor();
		spell.aimDir = Vector3.Normalize(new Vector3(spell.aimPoint.x, spell.aimPoint.y) - transform.position);

		float angle = Mathf.Atan2(spell.aimDir.y, spell.aimDir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		
		transform.position = spell.aimPoint;
		transform.position += transform.up * -0.5f;
		spell.Invoke ("KillSelf", duration + formTime);
		Invoke ("Spawn", formTime);
		AudioSource.PlayClipAtPoint(cast, transform.position);

        if (!isServer)
            return;

        IncreaseDuration(spell.upgrades.frostPrisonDuration);
        if (spell.upgrades.frostPrisonCircleWall > 0)
            CircleWall();

        IncreaseDmg(spell.upgrades.frostPrisonRamp);
        if (spell.upgrades.frostPrisonStorm > 0)
            ActivateStorm();
	}

	void SetColor()
	{
		switch(spell.team)
		{
		    case 1:
			    SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
			    foreach(SpriteRenderer renderer in renderers)
			    {
				    renderer.color = new Color(1.0f, 0.43f, 0.46f);
			    }

			    ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>();
			    foreach(ParticleSystem system in systems)
			    {
				    system.startColor = new Color(0.57f, 0.19f, 0.156f);
			    }

			break;
		}
	}

	void IncreaseDuration(int level)
	{
		duration += 0.5f * level;
	}
    
	void CircleWall()
	{
		formTime = 0.7f;
		spell.damage = 0;
		Invoke ("Spawn", formTime);
		circleWall = true;
		extraSpawn.SetActive (true);
	}
	
	void IncreaseDmg(int level)
	{
		spell.damage += 0.035f * level;
	}
    
	void ActivateStorm()
	{
		spell.damage += 0.04f;
		stormActive = true;
		spawn.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(formed)
		{
			currentTime += Time.deltaTime;
			totalDamage += spell.damage * (baseDamage + (currentTime/(duration * 4.4f)));
			//Debug.Log (totalDamage);
		}
	}

	void Spawn()
	{
		storm.SetActive(true);
		if(!stormActive)
		{
			spawn.SetActive(false);
			prison.SetActive(true);
			AudioSource.PlayClipAtPoint(form, transform.position);
			if(circleWall)
			{
				extraWall.SetActive(true);
				extraSpawn.SetActive(false);
				storm.SetActive(false);
			}
			SetColor();
		}
		formed = true;
	}

	void OnTriggerStay2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(formed)
		{
			if(other.CompareTag("Player"))
			{
				DamageSystem damageSystem = (DamageSystem)other.GetComponent ("DamageSystem");
				if(spell.team != damageSystem.Team())
				{
					damageSystem.Damage(spell.damage * (baseDamage + (currentTime/(duration * 4.4f))), spell.knockFactor, transform.position, spell.owner);
					if(stormActive)
					{
						other.GetComponent<Movement>().RpcSpeedBoost(0.5f, 0.2f);
					}
				}
			}
		}
	}
}
