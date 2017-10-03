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
/// de son angle nord-ouest, 2 floats pour sa largeur et sa longueur, 2 floats pour les hauteurs aux angles nord-est et sud-ouest.
/// </summary>
namespace BloodAndBileEngine.WorldState
{
    public class CellSystem : IWorldStateData
    {
        Cell[] Cells;

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
    }
}
