using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Movement : NetworkBehaviour {
	#region Fields

	public float speed = 5.0f;
	private float maxSpeed = 20.0f;

	public SpellCasting spellCasting;
    public DamageSystem damageSystem;

	public Vector3 bound;

	public float length;

	public float oldSpeed;

	public Vector3 holePos = Vector3.zero;

	#endregion

	#region Properties
	public float MovementSpeed
	{
		set
		{ 
			if(speed <= maxSpeed)
				speed = value;
		}

		get
		{
			return speed;
		}

	}
	#endregion
	void Start ()
    {
		oldSpeed = speed;
	}

    [ClientRpc]
	public void RpcSpeedBoost(float boost, float duration)
	{
		CancelInvoke("EndSpeedBoost");
		Invoke ("EndSpeedBoost", duration);
		//oldSpeed = speed;
		speed = oldSpeed * boost;
	}

	void EndSpeedBoost()
	{
		speed = oldSpeed;
	}

	void BoundTo(Transform boundTo)
	{
		//bound = boundTo;
	}

    [ClientRpc]
	public void RpcBound(float duration, float length)
	{
		bound = transform.position;
		this.length = length;
		Invoke ("RemoveBound", duration);
	}
    
    [ClientRpc]
    public void RpcReset()
	{
        if (!isLocalPlayer)
            return;

		Vector3 spawnPos = Vector3.zero;
		switch(spellCasting.team)
		{
		case 1: spawnPos = new Vector3(-11, 0, 0);
			break;
		case 2: spawnPos = new Vector3(11, 0, 0);
			break;
		}
		transform.position = spawnPos;
	}
    
	void Update ()
    {
        if (!isLocalPlayer)
            return;

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (!GlobalConstants.isFrozen && GameHandler.state == GameHandler.State.Game)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            movement = Vector3.Normalize(movement);
            if (bound == Vector3.zero)
            {
                if (!spellCasting.isCasting)
                {
                    if (movement.magnitude > 0.1)
                    {
                        GetComponentInChildren<Wizard>().RunAni();
                    }
                    else
                    {
                        GetComponentInChildren<Wizard>().IdleAni();
                    }
                    transform.position += (movement * speed / GlobalConstants.unitScaling) * Time.deltaTime * 60;
                }
                transform.position += damageSystem.knockback / GlobalConstants.unitScaling / 2 * Time.deltaTime * 60;
            }
            else
            {
                if(!spellCasting.isCasting)
                {
                    GetComponentInChildren<Wizard>().StunAni();
                }
                Vector3 newPos = transform.position + (movement * speed / GlobalConstants.unitScaling) * Time.deltaTime * 60 + damageSystem.knockback / GlobalConstants.unitScaling / 2 * Time.deltaTime * 60; ;
                
                if (Vector3.Distance(bound, newPos) < length)
                {
                    transform.position = newPos;
                }
            }

            Vector3 aimDir = Input.mousePosition;
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            aimDir.x = aimDir.x - pos.x;
            aimDir.y = aimDir.y - pos.y;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 135 + 90));
        }

        //GameObject.Find ("CooldownInfo").SendMessage ("UpdatePlayer1Pos", transform.position);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
	}

    [ClientRpc]
    public void RpcMove(Vector3 velocity)
    {
        Debug.Log("I should move");
        transform.position += velocity;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    
	void RemoveBound()
	{
		bound = Vector3.zero;
	}
}

public static class GlobalConstants
{
	//Used to translate sensible values to the actual Unity sizes (usually by dividing)
	public static float unitScaling = 135.0f;
	public static Vector2 screenSize = new Vector2(12.8f, 7.1f);

	public static bool isFrozen = false;

	public static Vector3 RotateZ(Vector3 v, float angle )
		
	{
		
		float sin = Mathf.Sin( angle );
		
		float cos = Mathf.Cos( angle );
		
		
		
		float tx = v.x;
		
		float ty = v.y;
		
		v.x = (cos * tx) - (sin * ty);
		
		v.y = (cos * ty) + (sin * tx);

		return v;
		
	}
}
