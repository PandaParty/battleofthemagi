using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {
    public GameObject target;
    public Vector2 offset;
	void Update ()
    {
        transform.position = target.transform.position + new Vector3(offset.x, offset.y);
	}
}
