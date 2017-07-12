using UnityEngine;
using System.Collections;

public class ScaleUp : MonoBehaviour {
	public GameObject parent;
	PlacedShield shield;
	float time = 0;

	bool goingUp = true;

	void Start()
	{
		shield = parent.GetComponent<PlacedShield>();
	}

	void Update () 
	{
		time += Time.deltaTime;
		if(goingUp)
		{
			transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 3.5f, time * 2);
			if(time >= shield.duration - 0.3f)
			{
				goingUp = false;
				time = 0;
			}
		}
		else
		{
			transform.localScale = Vector3.Lerp(Vector3.one * 3.5f, Vector3.zero, time * 4);
		}
	}
}
