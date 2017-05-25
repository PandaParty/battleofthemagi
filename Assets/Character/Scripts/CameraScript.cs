using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	private GameObject playerObject;

	private bool cameraLocked = true;

	public GameObject PlayerObject
	{
		set 
		{
			playerObject = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			cameraLocked = !cameraLocked;
		}
		if(playerObject != null)
		{
			if(cameraLocked)
			{
				transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -1);
			}
			else if(GameHandler.state == GameHandler.State.Game)
			{
				Vector3 newPos = Vector3.Lerp(playerObject.transform.position, this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition), 0.5f);
				transform.position = new Vector3(newPos.x, newPos.y, -1);
				//transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -1);
			}
		}
	}
}
