using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	private GameObject playerObject;

	private bool cameraLocked = true;

    public float cameraDist = 15;

	public GameObject PlayerObject
	{
		set 
		{
			playerObject = value;
		}
	}
	
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			cameraLocked = !cameraLocked;
		}
		if(playerObject != null)
		{
			if(cameraLocked)
			{
				transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, 0) - transform.forward * cameraDist - transform.up;
			}
			else if(GameHandler.state == GameHandler.State.Game)
			{
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = cameraDist;
				Vector3 newPos = Vector3.Lerp(playerObject.transform.position, GetComponent<Camera>().ScreenToWorldPoint(mousePos), 0.5f);
				transform.position = new Vector3(newPos.x, newPos.y, 0) - transform.forward * cameraDist - transform.up;
				//transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -1);
			}
		}
	}
}
