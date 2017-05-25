﻿using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	#region Fields

	public float speed = 5.0f;
	private float maxSpeed = 20.0f;

	public SpellCasting spellCasting;

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
	// Use this for initialization
	void Start () {
		oldSpeed = speed;
	}

	public void SpeedBoost(float boost, float duration)
	{
		CancelInvoke("EndSpeedBoost");
		Invoke ("EndSpeedBoost", duration);
		//oldSpeed = speed;
		speed *= boost;
	}

	void EndSpeedBoost()
	{
		speed = oldSpeed;
	}

	void BoundTo(Transform boundTo)
	{
		//bound = boundTo;
	}

	public void Bound(float duration, float length)
	{
		bound = transform.position;
		length = length;
		Invoke ("RemoveBound", duration);
	}

	[RPC]
	void HolePos(Vector3 pos)
	{
		holePos = pos;
		if(!spellCasting.isSilenced)
		{
			spellCasting.Silence(0.2f);
		}
	}

	public void Reset()
	{
		Vector3 spawnPos = Vector3.zero;
		switch(spellCasting.team)
		{
		case 1: spawnPos = new Vector3(-9, 0, 0);
			break;
		case 2: spawnPos = new Vector3(9, 0, 0);
			break;
		case 3: spawnPos = new Vector3(0, -9, 0);
			break;
		case 4: spawnPos = new Vector3(0, 9, 0);
			break;
		}
		transform.position = spawnPos;
	}

	// Update is called once per frame
	void Update () {
		if(networkView.isMine)
		{
			if(holePos != Vector3.zero)
			{
				transform.position += Vector3.Normalize(holePos - transform.position) / GlobalConstants.unitScaling * 3;
			}
			else
			{
				if(spellCasting.blackHoling)
				{
					Debug.Log("Currently black holing");
				}
				if(!spellCasting.isCasting && !GlobalConstants.isFrozen && GameHandler.state == GameHandler.State.Game && !spellCasting.blackHoling)
				{
					Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
					movement = Vector3.Normalize(movement);
					if(bound == Vector3.zero)
					{
						transform.position += (movement * speed / GlobalConstants.unitScaling) * Time.deltaTime * 60;
					}
					else
					{
						Vector3 newPos = transform.position + (movement * speed / GlobalConstants.unitScaling) * Time.deltaTime * 60;
						
						if(Vector3.Distance(bound, newPos) < length)
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
				else if(Input.GetKeyDown (KeyCode.LeftShift))
				{
					spellCasting.StopCasting();
				}
			}
//			else
//			{
//				if(Input.GetKeyDown(KeyCode.W)
//				   || Input.GetKeyDown (KeyCode.A)
//				   || Input.GetKeyDown (KeyCode.S)
//				   || Input.GetKeyDown (KeyCode.D))
//				{
//					spellCasting.StopCasting();
//				}
//			}
		}
		GameObject.Find ("CooldownInfo").SendMessage ("UpdatePlayer1Pos", transform.position);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);
	}

	void LimitScreen()
	{
		//Horizontal screen limits
		if(transform.position.x < -GlobalConstants.screenSize.x)
		{
			float offset = transform.position.x + GlobalConstants.screenSize.x;
			transform.position += new Vector3(-offset, 0, 0);
		}
		else if(transform.position.x > GlobalConstants.screenSize.x)
		{
			float offset = transform.position.x - GlobalConstants.screenSize.x;
			transform.position += new Vector3(-offset, 0, 0);
		}

		//Vertical screen limits
		if(transform.position.y < -GlobalConstants.screenSize.y)
		{
			float offset = transform.position.y + GlobalConstants.screenSize.y;
			transform.position += new Vector3(0, -offset, 0);
		}
		else if(transform.position.y > GlobalConstants.screenSize.y)
		{
			float offset = transform.position.y - GlobalConstants.screenSize.y;
			transform.position += new Vector3(0, -offset, 0);
		}
	}

	void RemoveBound()
	{
		bound = Vector3.zero;
	}

	void Invis()
	{
		SpeedBoost(1.5f, 4f);
	}

	void EndInvis()
	{
		EndSpeedBoost();
	}

}

public static class GlobalConstants
{
	//Used to translate sensible values to the actual Unity sizes (usually by dividing)
	public static float unitScaling = 135.0f;
	public static Vector2 screenSize = new Vector2(12.8f, 7.1f);

	public static bool isFrozen = true;

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
