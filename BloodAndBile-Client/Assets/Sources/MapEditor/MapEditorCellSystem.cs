using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapEditorCellSystem : MonoBehaviour {

    public const string CELLSYSTEM_PREFAB = "MapEditor/CellSystem";
    public const string SAVE_FOLDER = "Assets/Resources/Prefabs/Maps/Bin/";
    GameObject CellPrefab;

    public string MapName;

    private void Start()
    {
        if (CellPrefab == null)
        {
            CellPrefab = Resources.Load<GameObject>(MapEditorCell.CELL_PREFAB);
        }
    }


    public void AddCell()
    {
        if (CellPrefab == null)
        {
            CellPrefab = Resources.Load<GameObject>(MapEditorCell.CELL_PREFAB);
        }
        GameObject newCell = Instantiate<GameObject>(CellPrefab);
        newCell.transform.parent = transform;
    }

    public MapEditorCell[] GetCells()
    {
        List<MapEditorCell> cells = new List<MapEditorCell>();
        foreach(MapEditorCell cell in GetComponentsInChildren<MapEditorCell>())
        {
            cells.Add(cell);
        }

        return cells.ToArray();
    }
}
