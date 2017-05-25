using UnityEngine;
using System.Collections;

public class BindRope : MonoBehaviour {
	Transform boundTo;
	Vector3 boundPos;
	public LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		lineRenderer.SetPosition (0, boundTo.position);
		lineRenderer.SetPosition (1, boundPos);
	}

	[RPC]
	void SetKill(float duration)
	{
		Invoke ("KillSelf", duration);
	}

	void KillSelf()
	{
		Network.Destroy(gameObject);
	}

	[RPC]
	void SetBinds(Vector3 bindPos, string bindTo)
	{
		boundPos = bindPos;

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			SpellCasting spell = player.GetComponent<SpellCasting>();
			if(spell.playerName == bindTo)
			{
				boundTo = player.transform;
			}
		}
	}

}
