using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapEditorCell))]
public class MapEditorCellEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Tourner"))
        {
            Turn();
        }
    }

    void Turn()
    {
        ((MapEditorCell)target).Turn();
    }
}
