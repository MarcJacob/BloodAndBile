using BloodAndBileEngine.WorldState;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BloodAndBileEngine.WorldState
{
    public class Map : IWorldStateData
    {
        const string CONSTRUCTION_DATA_PATH = "Assets/Resources/Prefabs/Maps/Bin/";

        public int ID { get; private set; }
        private static int lastID = 0;
        public string MapPrefabPath { get; private set; }
        public string MapName { get; private set; }
        public float[] ConstructionData { get; private set; }
        public static List<Map> Maps { get; private set; }
        public int[] SpawnPoints { get; private set; }


        private Map(string name, string prefab, int[] spawns)
        {
            ID = lastID;
            lastID++;
            MapPrefabPath = prefab;
            MapName = name;
            SpawnPoints = spawns;
            Maps.Add(this);
            LoadConstructionData();
        }

        void LoadConstructionData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            float[] data = new float[] { };
            FileStream file;
            try
            {
                file = File.Open(CONSTRUCTION_DATA_PATH + MapName + ".mapData", FileMode.Open, FileAccess.Read);
                data = (float[])formatter.Deserialize(file);
                file.Close();
                file.Dispose();
            } catch (IOException e)
            {
                Debugger.Log("WARNING : Pas de données pour la map " + MapName + " !", UnityEngine.Color.yellow);
            }
            ConstructionData = data;
        }

        public void Simulate(float deltaTime) { }

        public static void LoadMaps()
        {
            Maps = new List<Map>();
            Map map1 = new Map("TestArena", "Prefabs/Maps/TestArena", new int[] { 0, 1 });
        }
    }
}