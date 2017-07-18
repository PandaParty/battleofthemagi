using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpeedBoost : NetworkBehaviour {
	bool someoneCapping = false;

	public GameObject effect;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isServer)
            return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<SpellCasting>().RpcStartChannelingPowerUp(gameObject, 4);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isServer)
            return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<SpellCasting>().RpcEndChannelingPowerUp();
        }
    }

    void Capped(GameObject player)
    {
        if (!isServer)
            return;

        player.GetComponent<Movement>().RpcSpeedBoost(2f, 8f);
        player.GetComponent<DamageSystem>().Damage(-15, 0, transform.position, "world");
        var newEffect = Instantiate(effect);
        newEffect.GetComponent<FollowPlayer>().SetFollow(player, 8f);
        NetworkServer.Spawn(newEffect);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.GetComponent<SpellCasting>().RpcEndChannelingPowerUp();
        }

        Destroy(gameObject);
    }
}
