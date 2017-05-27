using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowObject : MonoBehaviour {
    public GameObject target;
    public Vector2 offset;

    private void Start()
    {

    }

    void Update ()
    {
        transform.position = target.transform.position + new Vector3(offset.x, offset.y, -0.43f);
    }
}
