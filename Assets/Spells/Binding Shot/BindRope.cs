using UnityEngine;
using System.Collections;

public class BindRope : MonoBehaviour {
	Transform boundTo;
	Vector3 boundPos;
	public LineRenderer lineRenderer;
    
	public void SetKill(float duration)
	{
		Invoke ("KillSelf", duration);
	}

	void KillSelf()
	{
		Destroy(gameObject);
	}
    
	public void SetBinds(Vector3 bindPos, string bindTo)
	{
		boundPos = bindPos;

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			SpellCasting spell = player.GetComponent<SpellCasting>();
			if(spell.playerName == bindTo)
			{
				boundTo = player.transform;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, player.transform.position);
			}
		}
	}
}
