using UnityEngine;
using BloodAndBileEngine.WorldState;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour
{
    void Awake()
    {
        Map.LoadMaps();
        Maps = Map.Maps.ToArray();
        TargetHeight = transform.position.y;

        FreeView = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    bool MapChosen = false;
    Map ChosenMap;
    Map[] Maps;
    MapEditorCellSystem CellSystem;
    GameObject MapGO;

    MapEditorCell SelectedCell;

    void Update()
    {
        CameraControl();
        CheckSelection();
        DisplayHandles();
    }

    void OnGUI()
    {
        if (!MapChosen)
        {
            int mapID = 0;
            foreach (Map m in Maps)
            {
                if (GUI.Button(new Rect(0, 100*mapID, 200, 100),m.MapName))
                {
                    ChosenMap = m;
                    MapChosen = true;
                    OnMapChosen();
                }
                mapID++;
            }
        }
        else
        {
            if (GUI.Button(new Rect(0, 0, 200, 100), "Sauvegarder"))
            {
                SaveCurrentMap();
            }
            else if (GUI.Button(new Rect(0, 100, 200, 100), "Quitter"))
            {
                OnExit();
            }
        }
    }

    void OnMapChosen()
    {
        MapGO = Instantiate(Resources.Load<GameObject>(ChosenMap.MapPrefabPath));
        CellSystem = Instantiate(Resources.Load<GameObject>("MapEditor/CellSystem")).GetComponent<MapEditorCellSystem>();
        LoadMap();
    }

    void OnExit()
    {
        DeselectCurrentCell();
        Destroy(MapGO);
        Destroy(CellSystem.gameObject);
        MapChosen = false;
    }

    void SaveCurrentMap()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        // Ouverture du fichier
        FileStream file = File.Open(MapEditorCellSystem.SAVE_FOLDER + ChosenMap.MapName + ".mapData", FileMode.Create, FileAccess.ReadWrite);
        // Code écriture...
        List<float> data = new List<float>();
        foreach (MapEditorCell cell in (CellSystem).GetCells())
        {
            foreach (float f in cell.GetFloatArray())
            {
                data.Add(f);
            }
        }

        formatter.Serialize(file, data.ToArray());

        //

        // Fermeture du fichier
        file.Close();
        file.Dispose();
    }

    void LoadMap()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        // Ouverture du fichier
        try
        {
            FileStream file = File.Open(MapEditorCellSystem.SAVE_FOLDER + ChosenMap.MapName + ".mapData", FileMode.Open, FileAccess.Read);
            // Code lecture

            float[] data;
            data = (float[])formatter.Deserialize(file);
            for (int cellID = 0; cellID < data.Length; cellID += 7)
            {
                MapEditorCell cell = CellSystem.AddCell();
                cell.transform.position = new Vector3(data[cellID], data[cellID + 1], data[cellID + 2]);
                cell.GetEastCorner().localPosition = new Vector3(data[cellID], data[cellID + 5], data[cellID + 3]);
                cell.GetSouthCorner().localPosition = new Vector3(data[cellID + 4], data[cellID + 6], data[cellID + 2]);
            }

            //

            // Fermeture du fichier
            file.Close();
            file.Dispose();
        } catch (IOException e)
        {
            BloodAndBileEngine.Debugger.Log("Pas de données trouvées pour cette map !", Color.yellow);
        }
    }

    void SelectCell(MapEditorCell cell)
    {
        if (SelectedCell != cell)
        {
            DeselectCurrentCell();
            SelectedCell = cell;
            SelectedCell.Select();
        }
        HandlesDisplayed = true;
    }

    void DeselectCurrentCell()
    {
        if (SelectedCell != null)
        {
            SelectedCell.Deselect();
            SelectedCell = null;
        }
        HandlesDisplayed = false;
    }

    public float speed = 5f;
    public float MaxHeight = 500f;
    public float MinHeight = -500f;

    public float TargetHeight;
    bool FreeView = true;
    void CameraControl()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            speed += 1f;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            speed -= 1f;
        }
        float effectiveSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            effectiveSpeed *= 2;
        }

        float hTranslation = Input.GetAxis("Horizontal") * effectiveSpeed * Time.deltaTime;
        float vTranslation = Input.GetAxis("Vertical") * effectiveSpeed * Time.deltaTime;

        Quaternion yRotation = transform.rotation;
        yRotation.eulerAngles = new Vector3(0f, yRotation.eulerAngles.y, 0f);

        Vector3 translationVect = yRotation * new Vector3(hTranslation, 0f, vTranslation);
        transform.Translate(translationVect, Space.World);

        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, TargetHeight, transform.position.z), Time.deltaTime);

        float middleMouseAxis = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log(middleMouseAxis);
        TargetHeight += -middleMouseAxis * 6;

        if (TargetHeight < MinHeight)
        {
            TargetHeight = MinHeight;
        }
        else if (TargetHeight > MaxHeight)
        {
            TargetHeight = MaxHeight;
        }

        if (FreeView)
        {
            transform.Rotate(0f, Input.GetAxis("Mouse X"), 0, Space.World);
            transform.Rotate(-Input.GetAxis("Mouse Y"), 0f, 0f, Space.Self);
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                FreeView = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                FreeView = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                SelectedCell = null;
            }
        }
    }
    void CheckSelection()
    {
        if (!FreeView)
        {
            if (Input.GetMouseButtonDown(0))
            {
                
                MapEditorCell[] cells = FindObjectsOfType<MapEditorCell>();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag != "TAG_HANDLE")
                    {
                        int cellID = 0;
                        bool cellFound = false;
                        while (SelectedCell == null && cellID < cells.Length)
                        {
                            if (cells[cellID].IsInCell(hit.point.x, hit.point.z))
                            {
                                SelectedCell = cells[cellID];
                                SelectedCell.Select();
                                HandlesDisplayed = true;
                                cellFound = true;
                            }
                            cellID++;
                        }
                        if (!cellFound)
                        {
                            DeselectCurrentCell();
                        }
                    }
                    else
                    {
                        OnHandleSelected(hit.collider.GetComponent<CellHandle>());
                    }
                }
                else
                {
                    DeselectCurrentCell();
                }
            }
        }
    }

    public GameObject HandlePrefab;
    bool HandlesDisplayed = false;
    CellHandle PosHandle;
    CellHandle EastHandle;
    CellHandle SouthHandle;
    void DisplayHandles()
    {
        if (PosHandle == null && HandlesDisplayed)
        {
            PosHandle = Instantiate(HandlePrefab).GetComponent<CellHandle>();
            EastHandle = Instantiate(HandlePrefab).GetComponent<CellHandle>();
            SouthHandle = Instantiate(HandlePrefab).GetComponent<CellHandle>();
            PosHandle.SetTransform(SelectedCell.transform);
            EastHandle.SetTransform(SelectedCell.GetEastCorner());
            SouthHandle.SetTransform(SelectedCell.GetSouthCorner());

            EastHandle.RestrictX = true;
            SouthHandle.RestrictZ = true;
        }
        else if (PosHandle != null && !HandlesDisplayed)
        {
            Destroy(PosHandle.gameObject);
            Destroy(EastHandle.gameObject);
            Destroy(SouthHandle.gameObject);
            PosHandle = null;
            EastHandle = null;
            SouthHandle = null;
        }
    }

    void OnHandleSelected(CellHandle handle)
    {
        Debug.Log("Handle selected !");
        handle.SelectHandle();
    }
}
