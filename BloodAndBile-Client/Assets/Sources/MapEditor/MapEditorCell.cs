using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class MapEditorCell : MonoBehaviour
{
    public const string CELL_PREFAB = "MapEditor/Cell";

    Transform EastCorner;
    Transform SouthCorner;

    public void Start()
    {
        EastCorner = transform.Find("EAST_CORNER");
        if (EastCorner == null)
        {
            EastCorner = new GameObject("EAST_CORNER").transform;
            EastCorner.Translate(0f, 0f, 2f);
        }

        SouthCorner = transform.Find("SOUTH_CORNER");
        if (SouthCorner == null)
        {
            SouthCorner = new GameObject("SOUTH_CORNER").transform;
            SouthCorner.Translate(2f, 0f, 0f);
        }

        EastCorner.transform.parent = transform;
        SouthCorner.transform.parent = transform;

    }

    private void Update()
    {
        EastCorner.transform.Translate(transform.position.x - EastCorner.transform.position.x, 0f, 0f);
        SouthCorner.transform.Translate(0f, 0f, transform.position.z - SouthCorner.transform.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one / 5f);
        Gizmos.DrawCube(EastCorner.position, Vector3.one / 5f);
        Gizmos.DrawCube(SouthCorner.position, Vector3.one / 5f);
        Gizmos.DrawCube(transform.position + EastCorner.localPosition + SouthCorner.localPosition, Vector3.one / 5f);

        Gizmos.DrawLine(transform.position, EastCorner.position);
        Gizmos.DrawLine(transform.position, SouthCorner.position);
        Gizmos.DrawLine(SouthCorner.position, SouthCorner.position + EastCorner.localPosition);
        Gizmos.DrawLine(EastCorner.position, SouthCorner.position + EastCorner.localPosition);
    }

    public float[] GetFloatArray()
    {
        float[] array = new float[7];

        // Position de l'angle nord ouest
        array[0] = transform.position.x;
        array[1] = transform.position.y;
        array[2] = transform.position.z;
        //

        // Dimensions

        array[3] = EastCorner.position.z - transform.position.z;
        array[4] = SouthCorner.position.x - transform.position.x;

        // Hauteurs

        array[5] = EastCorner.position.y - transform.position.y;
        array[6] = SouthCorner.position.y - transform.position.y;

        return array;
    }

    // "Tourne" la cellule à 90 degrés.
    public void Turn()
    {
        float xDim = EastCorner.localPosition.z;
        float yDim = SouthCorner.localPosition.x;

        EastCorner.Translate(0, 0, yDim - EastCorner.localPosition.z);
        SouthCorner.Translate(xDim - SouthCorner.localPosition.x, 0, 0);
    }
}
