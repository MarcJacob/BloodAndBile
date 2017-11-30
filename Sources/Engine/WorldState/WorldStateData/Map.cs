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

        private Map(String prefab, float[] constrData)
        {
            ID = lastID;
            lastID++;
            MapPrefabPath = prefab;
            ConstructionData = constrData;
            Maps.Add(this);
        }

        public void Simulate(float deltaTime) { }

        public static void LoadMaps()
        {
            Maps = new List<Map>();
            float[] cellData = new float[]
            {
                // Positions    // Dimensions   // Hauteurs
                0f, 0f, 0f,     10f, 10f,       0f, 0f ,     // Cellule dans l'angle de la map de 10 x 10 plate.
                0f, 0f, 10f,    10f, 10f,       3f, 0f ,     // Cellule de 10x10 pentue.   
            };
            Map map1 = new Map("Prefabs/Maps/Terrain", cellData);
        }
    }
}