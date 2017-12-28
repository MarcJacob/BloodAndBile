using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;

[CustomEditor(typeof(MapEditorCellSystem))]
public class MapEditorCellSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Ajouter cellule"))
        {
            AddCell();
        }
        if (GUILayout.Button("Sauvegarder"))
        {
            SaveToBinaryFile();
        }


    }

    void AddCell()
    {
        ((MapEditorCellSystem)target).AddCell();
    }

    void SaveToBinaryFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        // Ouverture du fichier
        FileStream file = File.Open(MapEditorCellSystem.SAVE_FOLDER + ((MapEditorCellSystem)target).MapName + ".mapData", FileMode.Create, FileAccess.Write);
        // Code écriture...
        List<float> data = new List<float>();
        foreach(MapEditorCell cell in ((MapEditorCellSystem)target).GetCells())
        {
            foreach(float f in cell.GetFloatArray())
            {
                data.Add(f);
            }
        }

        formatter.Serialize(file, data.ToArray());

        //

        // Fermeture du fichier
        file.Close();
    }
}
