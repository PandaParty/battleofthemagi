using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FollowObject : NetworkBehaviour {
    [SyncVar]
    public GameObject target;
    [SyncVar]
    public string text;
    public Vector2 offset;

    private void Start()
    {
        GetComponent<TextMesh>().text = text;
    }

    void Update ()
    {
        transform.position = target.transform.position + new Vector3(offset.x, offset.y, -0.43f);
    }
}
