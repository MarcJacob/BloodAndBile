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
        float[] ConstructionData;
        /// <summary>
        /// Initialise un CellSystem à partir d'un tableau de float.
        /// Taille du tableau : Nombre de cellules * (3+2+2) = Nombre de cellules * 7.
        /// Forme du tableau : Pour chaque cellules : 3 floats pour la position de l'angle nord-ouest, 2 floats
        /// pour la largeur et la longueur, 2 floats pour les hauteurs aux angles nord-est et sud-ouest.
        /// </summary>
        /// <param name="data"> Tableau en entrée </param>
        public CellSystem(float[] data)
        {
            // On vérifie que le tableau soit d'une taille valide.
            if (data.Length % 7 != 0)
            {
                Debugger.Log("ERREUR - le tableau donné en entrée du CellSystem n'est pas d'une forme valide !", UnityEngine.Color.red);
                // On n'initialise pas les cellules.
            }
            else
            {
                Debugger.Log("Initialisation du CellSystem.");
                List<Cell> cells = new List<Cell>();
                // Lecture du tableau
                for (int cellID = 0; cellID < data.Length; cellID += 7)
                {
                    Cell newCell;
                    UnityEngine.Vector3 cellPos = new UnityEngine.Vector3(data[cellID], data[cellID + 1], data[cellID + 2]);
                    UnityEngine.Vector2 cellDimensions = new UnityEngine.Vector2(data[cellID + 3], data[cellID + 4]);
                    UnityEngine.Vector2 cellHeights = new UnityEngine.Vector2(data[cellID + 5], data[cellID + 6]);
                    newCell = new Cell(this, cellPos, cellDimensions, cellHeights);
                    cells.Add(newCell);
                }

                // Initialisation du tableau Cells
                Cells = cells.ToArray();
                ConstructionData = data;
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
                if (cell.GetPosition().x < x && cell.GetPosition().x + cell.GetDimensions().x > x)
                {
                    if (cell.GetPosition().y < y && cell.GetPosition().y + cell.GetDimensions().y > y)
                    {
                        c = cell;
                    }
                }
            }

            return c;
        }

        public Cell[] GetCells()
        {
            return Cells;
        }

        public float[] GetCellConstructionData()
        {
            return ConstructionData;
        }
    }
}
