using UnityEngine;
using System.Collections;

public class IceRangHit : MonoBehaviour {
	public AudioClip explo;
	// Use this for initialization
	void Start () {
		Invoke ("KillSelf", 0.75f);
		AudioSource.PlayClipAtPoint(explo, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void KillSelf()
	{
		GameObject.Destroy(gameObject);
	}
}
