using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLinkTest : MonoBehaviour {

    BloodAndBileEngine.Entity entity;

	// Use this for initialization
	void Start () {
        entity = BloodAndBileEngine.EntitiesManager.GetEntityFromID(0);
	}
    public float value;
	// Update is called once per frame
	void Update () {
		if (!BloodAndBileEngine.EntitiesManager.GetEntityFromID(0).Destroyed)
        {
            transform.position = Vector3.Lerp(transform.position, entity.Position, Time.deltaTime);
            value = entity.CurrentCellID;
        }
	}
}
