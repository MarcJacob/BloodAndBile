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
        public int ID { get; private set; } // ID de cette cellule. Est déterminé par sa place dans le tableau des cellules du CellSystem.
        public bool PlayerSpawn { get; private set; }

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

        public Cell(CellSystem system, int id, UnityEngine.Vector3 pos, UnityEngine.Vector2 dim, UnityEngine.Vector2 h)
        {
            Position = pos;
            Dimensions = dim;
            Heights = h;
            ID = id;
            CellSystemRef = new WeakReference(system);
            PlayerSpawn = false;

            Debugger.Log("Created cell at position " + Position + " and dimensions " + Dimensions);
        }

        /// <summary>
        /// Renvoi la hauteur d'un point se trouvant aux coordonnées indiquées à vue d'oiseau.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetHeightFrom2DCoordinates(float x, float y)
        {
            if (Position.z <= x && Position.x <= y && Position.x + Dimensions.y > y && Position.z + Dimensions.x > x)
            {
                x = (x - Position.z) / Dimensions.x;
                y = (y - Position.x) / Dimensions.y;
                float xHeight = Heights.x * x;
                float yHeight = Heights.y * y;
                return Position.y + xHeight + yHeight; // ATTENTION : Pour Position, l'élément y est la hauteur !
            }
            else
            {
                return -500;
            }
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
            if (!EntitiesInCell.Contains(e)) { EntitiesInCell.Add(e); e.SetCellID(ID); }
        }

        public void RemoveEntity(Entity e)
        {
            EntitiesInCell.Remove(e);
        }

        public void RemoveEntity(int entityID)
        {
            int index = -1;
            int i = 0;
            while (index < 0 && i < EntitiesInCell.Count)
            {
                if (EntitiesInCell[i].ID == entityID)
                {
                    index = i;
                }
                else
                {
                    i++;
                }
            }

            if (index >= 0) EntitiesInCell.RemoveAt(index);
        } // Devrait être légèrement plus rapide que la surcharge prenant
        // une entité en paramètre.

        /// <summary>
        /// Enlève les entités dont l'ID est contenu dans la liste ids de la cellule.
        /// Renvoi un tableau contenant l'ensemble des IDs des entités enlevées.
        /// Si kill = true, alors les entités seront également détruites.
        /// </summary>
        public uint[] RemoveEntities(List<uint> ids, bool kill = false) // Supprime toutes les entités de cette Cellule dont les IDs se trouvent dans la liste passé en paramètre.
        {
            List<uint> removedEntityIDs = new List<uint>();

            List<Entity> removedEntities = new List<Entity>();
            foreach(Entity entity in EntitiesInCell)
            {
                if(ids.Contains(entity.ID))
                {
                    removedEntities.Add(entity);
                    ids.Remove(entity.ID);
                    if (kill) entity.Destroy();
                    removedEntityIDs.Add(entity.ID);
                }
            }
            foreach(Entity entity in removedEntities)
            {
                EntitiesInCell.Remove(entity);
            }
            return removedEntityIDs.ToArray();
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
                    if (e.Position.x < Position.x || e.Position.x > Position.x + Dimensions.y || e.Position.z < Position.z || e.Position.z > Position.z + Dimensions.x)
                    {
                        Cell newCell = GetCellSystem().GetCellFromPosition(e.Position.z, e.Position.x);
                        if (newCell != null)    
                        {
                            newCell.AddEntity(e);
                            DestroyedEntities.Add(e);
                        }
                        else
                        {
                            Debugger.Log("Entitée ID " + e.ID + " ne se trouve plus sur une cellule.", UnityEngine.Color.yellow);
                        }
                    }
                }
            }
            foreach(Entity e in DestroyedEntities)
            {
                EntitiesInCell.Remove(e);
            }
        }

        public void SetPlayerSpawn(bool spawn)
        {
            PlayerSpawn = spawn;
        }
    }
}
