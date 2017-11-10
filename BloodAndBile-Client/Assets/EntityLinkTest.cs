using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLinkTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public float value;
	// Update is called once per frame
	void Update () {
		if (!BloodAndBileEngine.EntitiesManager.GetEntityFromID(0).Destroyed)
        {
            transform.position = Vector3.Lerp(transform.position, BloodAndBileEngine.EntitiesManager.GetEntityFromID(0).Position, Time.deltaTime);
            value = ((BloodAndBileEngine.TestController)(BloodAndBileEngine.EntitiesManager.GetEntityFromID(0).GetComponent(typeof(BloodAndBileEngine.TestController)))).value;
        }
        else
        {
            Debug.Log("lol");
        }
	}
}
