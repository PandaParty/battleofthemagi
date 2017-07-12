using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DamageBoost : NetworkBehaviour {
	bool someoneCapping = false;

	public GameObject effect;

	void OnTriggerStay2D(Collider2D other)
	{
        if (!isServer)
            return;

		if(other.CompareTag("Player"))
		{
			other.GetComponent<SpellCasting>().StartChannelingPowerUp(gameObject, 4);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
            other.GetComponent<SpellCasting>().EndChannelingPowerUp();
		}
	}

	void Capped(GameObject player)
	{
		player.GetComponent<DamageSystem>().Damage(-15, 0, transform.position, "world");
		player.GetComponent<SpellCasting>().RpcDamageBoost(1.5f, 10f);
        var newEffect = Instantiate(effect);
        newEffect.GetComponent<FollowPlayer>().SetFollow(player, 8f);
        NetworkServer.Spawn(newEffect);
        Destroy(gameObject);
	}
}
