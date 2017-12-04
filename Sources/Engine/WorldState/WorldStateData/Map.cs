using BloodAndBileEngine.WorldState;
using System;
using System.Collections.Generic;

namespace BloodAndBileEngine.WorldState
{
    public class Map : IWorldStateData
    {
        public int ID { get; private set; }
        private static int lastID = 0;
        public String MapPrefabPath { get; private set; }
        public float[] ConstructionData { get; private set; }
        public static List<Map> Maps { get; private set; }
        public int[] SpawnPoints { get; private set; }


        private Map(String prefab, float[] constrData, int[] spawns)
        {
            ID = lastID;
            lastID++;
            MapPrefabPath = prefab;
            ConstructionData = constrData;
            SpawnPoints = spawns;
            Maps.Add(this);
        }

        public void Simulate(float deltaTime) { }

        public static void LoadMaps()
        {
            Maps = new List<Map>();
            Map map1 = new Map("Prefabs/Maps/Terrain", new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, new int[] { 0 });
        }
    }
}