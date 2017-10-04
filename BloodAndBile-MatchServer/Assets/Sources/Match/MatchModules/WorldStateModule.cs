using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Ce module assure la gestion du WorldState lié au Match. Il contient donc indirectement toutes les données
/// de la partie en cours.
/// 
/// Il sert également à construire le message StateUpdate.
/// </summary>
public class WorldStateModule : MatchModule, IStateUpdater
{
    BloodAndBileEngine.WorldState.WorldState CurrentWorldState;
    public BloodAndBileEngine.WorldState.WorldState GetWorldState()
    {
        return CurrentWorldState;
    }

    public WorldStateModule(Match m) : base(m)
    {
        CurrentWorldState = new BloodAndBileEngine.WorldState.WorldState();
    }

    public BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject[] GetStateUpdateInformations()
    {
        List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject> stateUpdates = new List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject>();

        // STATE UPDATE DU WORLDSTATE

        List<object> EntityPositions = new List<object>();

        BloodAndBileEngine.WorldState.CellSystem worldCellSystem = CurrentWorldState.GetData<BloodAndBileEngine.WorldState.CellSystem>();

        foreach(BloodAndBileEngine.WorldState.Cell cell in worldCellSystem.GetCells())
        {
            foreach(BloodAndBileEngine.Entity entity in cell.GetEntities())
            {
                EntityPositions.Add(entity.ID);
                EntityPositions.Add(entity.Position);
            }
        }

        stateUpdates.Add(new BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject(0, EntityPositions.ToArray())); // Type 0 = ENTITY_POSITIONS.

        //
        return stateUpdates.ToArray();
    }


}
