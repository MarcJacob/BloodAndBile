using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    public class EntityMover : EntityComponent
    {

        WeakReference CellSystemWeakRef = new WeakReference(null);

        public override void Initialise(BloodAndBileEngine.WorldState.WorldState worldState)
        {
            WorldState.CellSystem cellSystem = worldState.GetData<WorldState.CellSystem>();
            if (cellSystem != null)
            {
                CellSystemWeakRef.Target = cellSystem;
            }
            else
            {
                CellSystemWeakRef = null;
            }
        }

        public WorldState.CellSystem GetCellSystem()
        {
            return (WorldState.CellSystem)CellSystemWeakRef.Target;
        }

        public override void Update(float deltaTime)
        {
            
        }

        /// <summary>
        /// Mouvement dans une direction par rapport à la position actuelle.
        /// Détermine automatiquement la hauteur en fonction de la hauteur de la cellule actuelle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(float x, float y)
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
}
