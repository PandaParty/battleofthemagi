using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MagmaBlast : NetworkBehaviour {
	
	public Spell spell;
	public AudioClip cast;
	public bool amplify;
	public float selfDmg = 4;
	// Use this for initialization
	void Start () {
		spell.SetColor();
		AudioSource.PlayClipAtPoint(cast, transform.position);
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (!isServer)
            return;

		string ownerName = spell.owner;
		//Upgrading upgrading = GameObject.Find ("GameHandler").GetComponent<Upgrading>();
            
		foreach(GameObject player in ConnectionHandler_2.players)
		{
			DamageSystem damageSystem = player.GetComponent<DamageSystem>();
			if(spell.team != damageSystem.Team() && !damageSystem.isDead)
			{
				CircleCollider2D coll = (CircleCollider2D)player.GetComponent("CircleCollider2D");
				if(Vector3.Distance(player.transform.position, gameObject.transform.position) - coll.radius < 3.1f)
				{
					//if(upgrading.magmaBlastAmplify.currentLevel > 0)
					//{
					//	damageSystem.GetComponent<NetworkView>().RPC ("LavaAmplify", RPCMode.All, 0.5f, 5);
					//}
                    damageSystem.Damage(spell.damage, spell.knockFactor, transform.position, spell.owner);
					//damageSystem.GetComponent<NetworkView>().RPC ("HookDamage", RPCMode.All, spell.damage + upgrading.magmaBlastDmg.currentLevel, spell.knockFactor, transform.position, spell.owner);
				}
			}

			string playerName = ((SpellCasting)player.GetComponent ("SpellCasting")).playerName;
					
			if(ownerName == playerName)
			{
				player.GetComponent<DamageSystem>().Damage(selfDmg, 0, transform.position, spell.owner);
				//if(upgrading.magmaBlastSelfDispel.currentLevel > 0)
				//{
				//	player.SendMessage("Dispel", spell.owner);
				//}
			}
		}
		Invoke ("KillSelf", 1);
	}

	public void KillSelf()
	{
		Destroy (gameObject);
	}
}
