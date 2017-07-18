using UnityEngine;
using System.Collections;

public class FireballExplo : MonoBehaviour
{
	public AudioClip explo;
	void Start ()
    {
		Invoke ("KillSelf", 0.75f);
		AudioSource.PlayClipAtPoint(explo, transform.position);
	}

	void KillSelf()
	{
		Destroy(gameObject);
	}
}
