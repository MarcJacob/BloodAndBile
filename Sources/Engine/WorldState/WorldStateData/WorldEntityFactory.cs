using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine.WorldState.WorldStateData
{
    public class WorldEntityFactory : IWorldStateData
    {
        WorldState World;
        public WorldEntityFactory(WorldState world)
        {
            World = world;
        }

        // Construit une entité dans le WorldState passé en paramètre, la place
        // dans la cellule appropriée si possible et renvoi une référence si tout s'est bien passé.
        public Entity BuildEntity(uint ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
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
            newEntity.SetWorldState(World);
            if (World == null) Debugger.Log("ERREUR : Pas de worldstate !", UnityEngine.Color.red);
            // Placement de l'entité sur une cellule.
            CellSystem cellSystem = World.GetData<CellSystem>();
            if (cellSystem != null)
            {
                Cell cell = cellSystem.GetCellFromPosition(pos.z, pos.x);
                if (cell != null)
                {
                    cell.AddEntity(newEntity);
                }
                else
                {
                    Debugger.Log("ERREUR : Pas de Cell trouvée lors de la création de l'entité " + ID);
                }
            }
            else
            {
                Debugger.Log("ERREUR : Pas de CellSystem lors de la création de l'entité " + ID);
            }
            newEntity.AddComponent(typeof(EntitySynchroniserComponent));
            newEntity.AddComponent(typeof(EntityMover));

            newEntity.Position = pos;
            newEntity.Rotation = rot;
            newEntity.Height = height;
            newEntity.Size = size;

            // Appel du callback
            if (OnEntityCreated != null)
            OnEntityCreated(newEntity);


            return newEntity;
        }
        public Entity BuildEntity(UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            return BuildEntity(EntitiesManager.GetNextID(), pos, rot, size, height);
        }


        // Construit un joueur dans le WorldState passé en paramètre, le place
        // dans la cellule appropriée si possible et renvoie une référence si tout s'est bien passé.
        public Entity BuildPlayer(uint ID, UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            Entity e = BuildEntity( ID, pos, rot, size, height);
            e.AddComponent(typeof(SpellComponent));
            e.AddComponent(typeof(HumorsComponent));
            e.GetComponent<HumorsComponent>().SetHumors(500, 500, 500, 500);
            return e;
        }

        public Entity BuildPlayer(UnityEngine.Vector3 pos, UnityEngine.Quaternion rot, float size, float height)
        {
            return BuildPlayer(EntitiesManager.GetNextID(), pos, rot, size, height);
        }

        Action<Entity> OnEntityCreated;

        public void RegisterOnEntityCreatedCallback(Action<Entity> cb)
        {
            OnEntityCreated += cb;
        }

        public void RemoveOnEntityCreatedCallback(Action<Entity> cb)
        {
            OnEntityCreated -= cb;
        }

        public void Simulate(float deltaTime)
        {
            // Rien ne se passe.
        }

    }
}
