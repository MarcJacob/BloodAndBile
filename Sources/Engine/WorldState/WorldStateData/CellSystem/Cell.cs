﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Une cellule est une portion de carte rectangulaire pouvant contenir des entités. 
/// </summary>
namespace BloodAndBileEngine.WorldState
{
    [Serializable]
    public class Cell
    {
        UnityEngine.Vector3 Position; // Position de l'angle nord-ouest de cette cellule.
        UnityEngine.Vector2 Dimensions; // Longueur et largeur de la cellule.
        UnityEngine.Vector2 Heights; // Hauteurs de l'angle nord-est et sud-ouest. 0 = plat par rapport à l'angle nord-ouest.

        List<Entity> EntitiesInCell = new List<Entity>(); // Entités présentes dans cette cellule.

        WeakReference CellSystemRef; // Référence "faible" : n'empêchera pas le garbage collector de se débarasser du CellSystem lié.
        CellSystem GetCellSystem()
        {
            if (CellSystemRef.IsAlive)
            {
                return (CellSystem)CellSystemRef.Target;
            }
            else
            {
                return null;
            }
        }

        public Cell(CellSystem system, UnityEngine.Vector3 pos, UnityEngine.Vector2 dim, UnityEngine.Vector2 h)
        {
            Position = pos;
            Dimensions = dim;
            Heights = h;
            CellSystemRef = new WeakReference(system);
        }

        /// <summary>
        /// Renvoi la hauteur d'un point se trouvant aux coordonnées indiquées à vue d'oiseau.
        /// Si le point ne se trouve pas dans la cellule, on appelle récursivement la fonction sur la cellule que l'on
        /// trouve à l'aide de la référence au CellSystem.
        /// 
        /// Si la référence au CellSystem n'est pas valide, ou s'il n'y a pas de cellule aux coordonnées indiquées,
        /// alors la hauteur renvoyée est de -500 (hauteur minimum).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetHeightFrom2DCoordinates(float x, float y)
        {
            if (Position.x + Dimensions.x > x && Position.y + Dimensions.y > y)
            {
                float xHeight = Dimensions.x / Heights.x * x;
                float yHeight = Dimensions.y / Heights.y * y;

                return Position.y + xHeight + yHeight; // ATTENTION : Pour Position, l'élément y est la hauteur !
            }
            else if (CellSystemRef.IsAlive)
            {
                Cell c = ((CellSystem)CellSystemRef.Target).GetCellFromPosition(x, y);
                if (c != null)
                {
                    return c.GetHeightFrom2DCoordinates(x, y);
                }
            }

            return -500;
        }

        public UnityEngine.Vector3 GetPosition()
        {
            return Position;
        }

        public UnityEngine.Vector2 GetDimensions()
        {
            return Dimensions;
        }

        public UnityEngine.Vector2 GetHeights()
        {
            return Heights;
        }

        public void AddEntity(Entity e)
        {
            if (!EntitiesInCell.Contains(e)) EntitiesInCell.Add(e);
        }

        public Entity[] GetEntities()
        {
            return EntitiesInCell.ToArray();
        }

        public void UpdateEntities(float deltaTime)
        {
            // Mise à jour des entités
            foreach(Entity e in EntitiesInCell)
            {
                e.Update(deltaTime);
            }

            // On vérifie que les entités ne soient pas sorti de la cellule. Si c'est le cas, alors
            // on détermine quelle cellule doit maintenant la contenir. Si aucune cellule n'est trouvée alors l'entité
            // est considérée comme toujours dans cette cellule, et une vérification supplémentaire sera effectuée à
            // la prochaine Update. On vérifie également si elles sont toujours vivantes.
            List<Entity> DestroyedEntities = new List<Entity>();
            foreach(Entity e in EntitiesInCell)
            {
                if (e.Destroyed) DestroyedEntities.Add(e);
                else
                {
                    if (e.Position.x < Position.x || e.Position.x > Position.x + Dimensions.x || e.Position.z < Position.z || e.Position.z > Position.z + Dimensions.y)
                    {
                        Cell newCell = GetCellSystem().GetCellFromPosition(e.Position.z, e.Position.x);
                        if (newCell != null)
                        {
                            newCell.AddEntity(e);
                            EntitiesInCell.Remove(e);
                        }
                        else
                        {
                            Debugger.Log("Entitée ID " + e.ID + " ne se trouve plus sur une cellule.", UnityEngine.Color.yellow);
                        }
                    }
                }
            }
        }
    }
}
