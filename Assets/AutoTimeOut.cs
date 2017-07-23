using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTimeOut : MonoBehaviour
{
    public float time;
	void Start ()
    {
        Invoke("TimeOut", time);
	}
	
    public void TimeOut()
    {
        Destroy(gameObject);
    }
}
