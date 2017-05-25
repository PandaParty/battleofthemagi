using UnityEngine;
using System.Collections;

public class OnConnection : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    if(!GetComponent<NetworkView>().isMine){
			Movement movement = (Movement)transform.GetComponent("Movement");
			//movement.enabled = false;
			SpellCasting spellCasting = (SpellCasting)transform.GetComponent("SpellCasting");
			//spellCasting.enabled = false;
		}
	}
}
