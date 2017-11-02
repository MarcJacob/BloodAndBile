using System;
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
        int ID; // ID de cette cellule. Est déterminé par sa place dans le tableau des cellules du CellSystem.


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

            Debugger.Log("Created cell at position " + Position + " and dimensions " + Dimensions);
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
                x = (x - Position.x) / Dimensions.x;
                y = (y - Position.y) / Dimensions.y;
                float xHeight = Heights.x * x;
                float yHeight = Heights.y * y;

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

        public void RemoveEntities(List<uint> ids) // Supprime toutes les entités de cette Cellule dont les IDs se trouvent dans la liste passé en paramètre.
        {
            List<Entity> destroyedEntities = new List<Entity>();
            foreach(Entity entity in EntitiesInCell)
            {
                if(ids.Contains(entity.ID))
                {
                    destroyedEntities.Add(entity);
                    ids.Remove(entity.ID);
                }
            }
            foreach(Entity entity in destroyedEntities)
            {
                EntitiesInCell.Remove(entity);
            }
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
            foreach(Entity e in DestroyedEntities)
            {
                EntitiesInCell.Remove(e);
            }
        }
    }
}
