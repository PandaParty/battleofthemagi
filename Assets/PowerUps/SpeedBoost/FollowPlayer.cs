using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	public GameObject player;
	public AudioClip sound;
	// Use this for initialization
	void Start () {
		AudioSource.PlayClipAtPoint(sound, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position + new Vector3(0, 0, 0.1f);
	}

	public void SetFollow(GameObject play, float dura)
	{
		player = play;
		Invoke ("KillSelf", dura);
	}

	void KillSelf()
	{
		Destroy(gameObject);
	}
}
