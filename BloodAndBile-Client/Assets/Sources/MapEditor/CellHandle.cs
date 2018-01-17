using System;
using System.Collections.Generic;
using UnityEngine;

public class CellHandle : MonoBehaviour
{
    Transform TargetTransform;
    bool Selected = false;
    public bool RestrictX = false;
    public bool RestrictY = false;
    public bool RestrictZ = false;

    static Color OriginalColor;

    private void Start()
    {
        if (OriginalColor != null)
        {
            OriginalColor = GetComponent<MeshRenderer>().material.color;
        }
    }

    public void SetTransform(Transform t)
    {
        TargetTransform = t;
    }

    public void SelectHandle()
    {
        Selected = true;
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void DeselectHandle()
    {
        Selected = false;
        GetComponent<MeshRenderer>().material.color = OriginalColor;
    }
    void Update()
    {
        if (!Selected)
        {
            transform.position = TargetTransform.position + Vector3.up /2;
        }
        else
        {
            
            TargetTransform.position = transform.position - Vector3.up /2;
            if (Input.GetMouseButtonUp(0))
            {
                DeselectHandle();
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    if (hit.collider.tag != "TAG_HANDLE")
                    {
                        Vector3 newPos = transform.position;
                        if (!RestrictX)
                        {
                            newPos.x = hit.point.x;
                        }
                        if (!RestrictY)
                        {
                            newPos.y = hit.point.y;
                        }
                        if (!RestrictZ)
                        {
                            newPos.z = hit.point.z;
                        }
                        newPos += Vector3.up / 2;
                        transform.position = newPos;
                    }
                }
            }
        }
    }
}
