using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class MapEditorCell : MonoBehaviour
{
    public const string CELL_PREFAB = "MapEditor/Cell";

    Transform EastCorner;
    Transform SouthCorner;

    public void Awake()
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

    Vector3 EastCornerLastPos;
    Vector3 SouthCornerLastPos;
    private void Update()
    {
        if (Neighbours.Length > 0)
        {
            foreach (MapEditorCell cell in Neighbours)
            {
                Debug.DrawLine(transform.position + EastCorner.localPosition / 2 + SouthCorner.localPosition / 2, cell.transform.position + cell.EastCorner.localPosition / 2 + cell.SouthCorner.localPosition / 2, Color.cyan);
            }
        }
        bool translated = false;
        if (EastCorner.position != EastCornerLastPos || SouthCorner.position != SouthCornerLastPos)
        {
            translated = true;
            UpdateCorners();
        }

        if (translated && Selected)
        {
            UpdateNeighbours();
            if (Neighbours.Length > 0)
            {
                foreach (MapEditorCell cell in Neighbours)
                {
                    OnPushBack(cell);
                }
            }
        }
    }

    MapEditorCell[] Neighbours = new MapEditorCell[0];

    void UpdateCorners()
    {
        EastCorner.Translate(transform.position.x - EastCorner.transform.position.x, 0f, 0f);
        SouthCorner.Translate(0f, 0f, transform.position.z - SouthCorner.transform.position.z);
        EastCornerLastPos = EastCorner.position;
        SouthCornerLastPos = SouthCorner.position;
        // On empêche les angles de se trouver à des position locales négatives sur leurs axes respectifs.
        if (EastCorner.position.z < transform.position.z)
        {
            EastCorner.position = transform.position;
        }
        if (SouthCorner.position.x < transform.position.x)
        {
            SouthCorner.position = transform.position;
        }
    }

    // Renvoi toutes les cellules chevauchant celle ci sur le plan X Z.
    public void UpdateNeighbours()
    {
        MapEditorCell[] allCells = GameObject.FindObjectsOfType<MapEditorCell>();
        List<MapEditorCell> neighbours = new List<MapEditorCell>();
        foreach(MapEditorCell cell in allCells)
        {
            if (cell != this && !neighbours.Contains(cell))
            {
                if (IsNeighbour(cell))
                {
                    neighbours.Add(cell);
                }
                else if (cell.IsNeighbour(this)) // IsNeighbours peut ne pas détecter les voisins plus petits qui ne chevauchent pas l'un des angles de cette cellule.
                {
                    neighbours.Add(cell);
                }
            }
        }

        Neighbours = neighbours.ToArray();
    }

    public Transform GetEastCorner()
    {
        return EastCorner;
    }

    public Transform GetSouthCorner()
    {
        return SouthCorner;
    }

    public bool IsNeighbour(MapEditorCell otherCell)
    {
        return otherCell.IsInCell(transform.position.x, transform.position.z)
            || otherCell.IsInCell(EastCorner.position.x, EastCorner.position.z)
            || otherCell.IsInCell(SouthCorner.position.x, SouthCorner.position.z)
            || otherCell.IsInCell((EastCorner.position + SouthCorner.localPosition).x, (EastCorner.position + SouthCorner.localPosition).z);
    }

    // Si target chevauche cette cellule, target voit sa position et la position de ses angles changer pour
    // éviter le chevauchement. Cette cellule garde ses coordonnées originales.
    void OnPushBack(MapEditorCell target)
    {
        // Si target a son angle nord ouest dans cette cellule
        if (IsInCell(target.transform, false))
        {
            // Déterminer s'il faudrait pousser target vers le sud ou l'est.
            float zDiff = EastCorner.position.z - target.transform.position.z;
            float xDiff = SouthCorner.position.x - target.transform.position.x;
            if (xDiff < zDiff)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    SouthCorner.Translate(-xDiff, 0f, 0f);
                }
                else
                {
                    target.MovePosition(xDiff, 0f, 0f);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    EastCorner.Translate(0f, 0f, -zDiff);
                }
                else
                    target.MovePosition(0f, 0f, zDiff);
            }
        }
    }

    // Permet de bouger la position de la cellule sans bouger ses angles
    public void MovePosition(float x, float y, float z)
    {
        Vector3 eastPos = EastCorner.position;
        Vector3 southPos = SouthCorner.position;
        transform.Translate(x, y, z);
        EastCorner.position = eastPos;
        SouthCorner.position = southPos;
        UpdateCorners();
    }

    // Est ce que cette position 2D se trouve dans cette cellule ?
    public bool IsInCell(float x, float z, bool countOver = true)
    {
        if (countOver)
        {
            if (x >= transform.position.x && x <= SouthCorner.position.x)
            {
                if (z >= transform.position.z && z <= EastCorner.position.z)
                {
                    return true;
                }
            }
        }
        else
        {
            if (x > transform.position.x && x < SouthCorner.position.x)
            {
                if (z > transform.position.z && z < EastCorner.position.z)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsInCell(Transform t, bool countOver = true)
    {
        return IsInCell(t.position.x, t.position.z, countOver);
    }


    bool Selected = false;
    public void Select()
    {
        Selected = true;
    }
    public void Deselect()
    {
        Selected = false;
    }

    private void OnDrawGizmos()
    {
        if (Selected) Gizmos.color = Color.red;
        else Gizmos.color = Color.white;
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
