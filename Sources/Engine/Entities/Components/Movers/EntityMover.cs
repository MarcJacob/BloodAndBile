using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

namespace BloodAndBileEngine
{
    public class EntityMover : EntityComponent, IEntitySynchroniser
    {

        float BaseSpeed = 2f;
        float CurrentSpeed;

        float RootTime = 0f;
        public float GetRootTime()
        {
            return RootTime;
        }

        public void SetRootTime(float time)
        {
            RootTime = time;
        }

        public float GetBaseSpeed()
        {
            return BaseSpeed;
        }

        public float GetCurrentSpeed()
        {
            return CurrentSpeed;
        }


        WeakReference CellSystemWeakRef = new WeakReference(null);
        WeakReference WorldStateWeakRef = new WeakReference(null);
        public override void Initialise(BloodAndBileEngine.WorldState.WorldState worldState)
        {
            WorldStateWeakRef.Target = worldState;
            WorldState.CellSystem cellSystem = worldState.GetData<WorldState.CellSystem>();
            if (cellSystem != null)
            {
                CellSystemWeakRef.Target = cellSystem;
            }
            else
            {
                CellSystemWeakRef.Target = null;
            }

            CurrentSpeed = BaseSpeed;
        }

        public WorldState.CellSystem GetCellSystem()
        {
            if (CellSystemWeakRef.Target == null)
            {
                Initialise((WorldState.WorldState)WorldStateWeakRef.Target);
            }
            return (WorldState.CellSystem)CellSystemWeakRef.Target;
        }

        public override void Update(float deltaTime)
        {
            if (RootTime > 0f)
                RootTime -= deltaTime;
        }

        /// <summary>
        /// Mouvement dans une direction par rapport à la position actuelle.
        /// Détermine automatiquement la hauteur en fonction de la hauteur de la cellule actuelle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(float x, float y)
        {
            if (RootTime <= 0f)
            {
                UnityEngine.Vector3 newPos = LinkedEntity.Position;
                newPos.z += x;
                newPos.x += y;
                if (CellSystemWeakRef.Target != null)
                {
                    float height = GetCellSystem().GetCells()[LinkedEntity.CurrentCellID].GetHeightFrom2DCoordinates(newPos.z, newPos.x);
                    newPos.y = height;
                }
                LinkedEntity.Position = newPos;
            }
        }

        public void Teleport(float x, float y)
        {
            if (RootTime <= 0f)
            {
                SerializableVector3 newPos = new SerializableVector3();
                newPos.z = x;
                newPos.x = y;
                if (GetCellSystem() != null)
                {
                    WorldState.Cell cell = GetCellSystem().GetCells()[LinkedEntity.CurrentCellID];
                    if (cell == null)
                    {
                        Debugger.Log("ERREUR : Pas de cellules trouvées !");
                        newPos.y = LinkedEntity.Position.y;
                    }
                    else
                    {
                        float height = cell.GetHeightFrom2DCoordinates(newPos.z, newPos.x);
                        if (height > -500)
                            newPos.y = height;
                        else
                            newPos.y = LinkedEntity.Position.y;
                    }
                }
                else
                {
                    Debugger.Log("ERREUR : Pas de CellSystem !");
                    newPos.y = LinkedEntity.Position.y;
                }
                LinkedEntity.Position = newPos;
            }
        }

        public StateUpdateObject[] GetSynchInfo()
        {
            StateUpdateObject baseSpeedObject = new StateUpdateObject("BaseSpeed", BaseSpeed);
            StateUpdateObject currentSpeedObject = new StateUpdateObject("CurrentSpeed", CurrentSpeed);
            StateUpdateObject rootedTimeObject = new StateUpdateObject("RootedTime", RootTime);
            return new StateUpdateObject[]{ baseSpeedObject, currentSpeedObject, rootedTimeObject };
        }

        public void OnSynch(ComponentSynchronizationDataObject data)
        {
            BaseSpeed = (float)data.GetSynchInfo("BaseSpeed");
            CurrentSpeed = (float)data.GetSynchInfo("CurrentSpeed");
            RootTime = (float)data.GetSynchInfo("RootedTime");
        }
    }
}
