using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.WorldState;

namespace BloodAndBileEngine.EntityFactories
{
    public class EntityFactory
    {
        // Construit une entité dans le WorldState passé en paramètre, la place
        // dans la cellule appropriée si possible et renvoi une référence si tout s'est bien passé.
        static public Entity BuildEntity(WorldState.WorldState world, uint ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            Debugger.Log("Building entity ID " + ID);
            Entity newEntity = EntitiesManager.GetEntityFromID(ID);
            if (!newEntity.Destroyed) // Cet ID est déjà prit par une entité en vie !
            {
                return null;
            }
            newEntity.Destroyed = false;
            // Destruction des components existants
            newEntity.Reset();

            // Assignation du WorldState à l'entité
            newEntity.SetWorldState(world);

            // Placement de l'entité sur une cellule.
            CellSystem cellSystem = world.GetData<CellSystem>();
            if (cellSystem != null)
            {
                Cell cell = cellSystem.GetCellFromPosition(pos.z, pos.x);
                if (cell != null)
                {
                    cell.AddEntity(newEntity);
                }
                newEntity.AddComponent(typeof(EntitySynchroniserComponent));
                newEntity.AddComponent(typeof(EntityMover));

                newEntity.Position = pos;
                newEntity.Rotation = rot;
                newEntity.Height = height;
                newEntity.Size = size;

                return newEntity;
            }
            
            // Si ce code est atteint, c'est qu'il y a eu un problème lors de la création de l'entité.
            return null;
        }
        static public Entity BuildEntity(WorldState.WorldState world, UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            return BuildEntity(world, EntitiesManager.GetNextID(), pos, rot, size, height);
        }


        // Construit un joueur dans le WorldState passé en paramètre, le place
        // dans la cellule appropriée si possible et renvoie une référence si tout s'est bien passé.
        static public Entity BuildPlayer(WorldState.WorldState world, uint ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            Debugger.Log("Building entity ID " + ID);
            Entity newEntity = EntitiesManager.GetEntityFromID(ID);
            if (!newEntity.Destroyed) // Cet ID est déjà prit par une entité en vie !
            {
                return null;
            }
            newEntity.Destroyed = false;
            // Destruction des components existants
            newEntity.Reset();

            // Assignation du WorldState à l'entité
            newEntity.SetWorldState(world);

            // Placement de l'entité sur une cellule.
            CellSystem cellSystem = world.GetData<CellSystem>();
            if (cellSystem != null)
            {
                Cell cell = cellSystem.GetCellFromPosition(pos.z, pos.x);
                if (cell != null)
                {
                    cell.AddEntity(newEntity);
                    BloodAndBileEngine.Debugger.Log("Test2", UnityEngine.Color.cyan);

                }
                newEntity.AddComponent(typeof(EntitySynchroniserComponent));
                newEntity.AddComponent(typeof(EntityMover));

                newEntity.Position = pos;
                newEntity.Rotation = rot;
                newEntity.Height = height;
                newEntity.Size = size;

                return newEntity;
            }

            // Si ce code est atteint, c'est qu'il y a eu un problème lors de la création de l'entité.
            return null;
        }
        static public Entity BuildPlayer(WorldState.WorldState world, UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            return BuildPlayer(world, EntitiesManager.GetNextID(), pos, rot, size, height);
        }
    }
}
