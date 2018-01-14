using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contient un ensemble de cellules, et plusieurs méthodes utilitaires de lecture
/// et manipulation des cellules.
/// 
/// Un CellSystem est aussi, indirectement, le conteneur de l'ensemble des entités d'un WorldState.
/// 
/// Un CellSystem est construit à partir d'un tableau de floats : Pour chaque cellule, 3 floats pour les coordonnées
/// de son angle nord-ouest, 2 floats pour sa longueur(est-ouest) et sa largeur(nord-sud), 2 floats pour les hauteurs aux angles nord-est et sud-ouest.
/// </summary>
namespace BloodAndBileEngine.WorldState
{
    public class CellSystem : IWorldStateData
    {
        Cell[] Cells;
        Map CurrentMap;

        /// <summary>
        /// Initialise un CellSystem à partir d'un tableau de float.
        /// Taille du tableau : Nombre de cellules * (3+2+2) = Nombre de cellules * 7.
        /// Forme du tableau : Pour chaque cellules : 3 floats pour la position de l'angle nord-ouest, 2 floats
        /// pour la largeur et la longueur, 2 floats pour les hauteurs aux angles nord-est et sud-ouest.
        /// </summary>
        /// <param name="data"> Tableau en entrée </param>
        public CellSystem(Map map)
        {
            // On vérifie que le tableau soit d'une taille valide.
            if (map.ConstructionData.Length % 7 != 0)
            {
                Debugger.Log("ERREUR - le tableau donné en entrée du CellSystem n'est pas d'une forme valide !", UnityEngine.Color.red);
                // On n'initialise pas les cellules.
            }
            else
            {
                Debugger.Log("Initialisation du CellSystem.");
                List<Cell> cells = new List<Cell>();
                // Lecture du tableau
                for (int cellID = 0; cellID < map.ConstructionData.Length; cellID += 7)
                {
                    Cell newCell;
                    UnityEngine.Vector3 cellPos = new UnityEngine.Vector3(map.ConstructionData[cellID], map.ConstructionData[cellID + 1], map.ConstructionData[cellID + 2]);
                    UnityEngine.Vector2 cellDimensions = new UnityEngine.Vector2(map.ConstructionData[cellID + 3], map.ConstructionData[cellID + 4]);
                    UnityEngine.Vector2 cellHeights = new UnityEngine.Vector2(map.ConstructionData[cellID + 5], map.ConstructionData[cellID + 6]);
                    newCell = new Cell(this, cellID / 7, cellPos, cellDimensions, cellHeights);
                    if (map.SpawnPoints.Contains(cellID))
                        newCell.SetPlayerSpawn(true);
                    cells.Add(newCell);
                }

                // Initialisation du tableau Cells
                Cells = cells.ToArray();
                CurrentMap = map;
            }
        }

        public void Simulate(float deltaTime) // Met à jour les entités dans toutes les cellules pour le temps donné.
        {
            foreach(Cell c in Cells)
            {
                c.UpdateEntities(deltaTime);
            }
        }

        public Cell GetCellFromPosition(float x, float y)
        {
            Cell c = null;

            int cellID = 0;
            while (cellID < Cells.Length && c == null) // Complexité maximale : Cells.Length.
            {
                Cell cell = Cells[cellID];
                if (cell.GetPosition().x <= y && cell.GetPosition().x + cell.GetDimensions().y > y)
                {
                    if (cell.GetPosition().z <= x && cell.GetPosition().z + cell.GetDimensions().x > x)
                    {
                        c = cell;
                    }
                }
                cellID++;
            }

            return c;
        }

        public Cell[] GetCells()
        {
            return Cells;
        }

        public float[] GetCellConstructionData()
        {
            return CurrentMap.ConstructionData;
        }

        public Map GetMap()
        {
            return CurrentMap;
        }

        // Renvoi l'intégralité des Entités se trouvant dans ce CellSystem.
        public Entity[] GetAllEntities()
        {
            List<Entity> entities = new List<Entity>();
            foreach(Cell cell in Cells)
            {
                foreach(Entity entity in cell.GetEntities())
                {
                    entities.Add(entity);
                }
            }

            return entities.ToArray();
        }

        // Supprime les entités dont l'ID se trouve dans cette liste.
        public void RemoveEntitiesFromID(uint[] ids)
        {
            // Construire la liste des ID :
            List<uint> idList = new List<uint>();
            foreach(uint id in ids)
            {
                idList.Add(id);
            }

            int cellID = 0;
            while (idList.Count > 0 && cellID < Cells.Length)
            {
                Cells[cellID].RemoveEntities(idList);
                cellID++;
            }

            if (idList.Count > 0)
            {
                Debugger.Log("Certaines entités à détruire n'ont pas été trouvées !", UnityEngine.Color.red);
            }
        }

        public Cell FindSpawnPoint()
        {
            int cellId = CurrentMap.SpawnPoints[UnityEngine.Random.Range(0, CurrentMap.SpawnPoints.Length-1)];
            return Cells[cellId];
        }
        
        public Cell FindSpawnPoint(int[] unwantedIDs)
        {
            if (unwantedIDs.Count() == 0)
                return FindSpawnPoint();

            int i = 0;
            int cellId = CurrentMap.SpawnPoints[UnityEngine.Random.Range(0, CurrentMap.SpawnPoints.Length-1)];
            while (unwantedIDs.Contains(cellId) && i < 10)
            {
                cellId = CurrentMap.SpawnPoints[UnityEngine.Random.Range(0, CurrentMap.SpawnPoints.Length-1)];
                i++;
            }
            if (unwantedIDs.Contains(cellId)) return null;
            return Cells[cellId];
        }

        // A la destruction : mettre toutes les entités en "Destroyed" pour les rendre utilisable
        // par d'autres.
        ~CellSystem()
        {
            foreach(Entity entity in GetAllEntities())
            {
                entity.Destroy();
            }
        }

        public void InitializeCellsLinks()
        {
            foreach(Cell i in Cells)
            {
                foreach(Cell j in Cells)
                {
                    float[,] iCoordinates = i.GetCoordinates();
                    float[,] jCoordinates = j.GetCoordinates();
                    int cost = 1;
                    float margin = 1f;

                    if((i.GetPosition().x < j.GetPosition().x +j.GetDimensions().x + margin) && (i.GetPosition().x > j.GetPosition().x + j.GetDimensions().x - margin))
                    {
                        i.AddLink(j, cost);
                    }
                    else if()
                    {
                        i.AddLink(j, cost);
                    }

                    else if ()
                    {
                        i.AddLink(j, cost);
                    }
                    else if()
                    {
                        i.AddLink(j, cost);
                    }
                }
            }
        }
    }
}
