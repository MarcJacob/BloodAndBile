using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Ce module assure la gestion de la carte liée au WorldState lié au Match.
/// 
/// </summary>
public class MapStateModule : MatchModule, IStateUpdater
{
    BloodAndBileEngine.WorldState.WorldState CurrentWorldState;
    public BloodAndBileEngine.WorldState.WorldState GetWorldState()
    {
        return CurrentWorldState;
    }

    public MapStateModule(Match m) : base(m)
    {
        CurrentWorldState = new BloodAndBileEngine.WorldState.WorldState();
    }

    public override void Initialise()
    {
        base.Initialise();
    }

    public override void Update(float deltaTime)
    {
        BloodAndBileEngine.Debugger.Log("MapStateModule.Update()");
        CurrentWorldState.Simulate(deltaTime);
    }

    public StateUpdateObject[] GetStateUpdateInformation()
    {
        List<StateUpdateObject> stateUpdates = new List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject>();

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

        stateUpdates.Add(new StateUpdateObject("ENTITY_POSITIONS", EntityPositions.ToArray()));

        //
        return stateUpdates.ToArray();
    }

    public StateUpdateObject[] GetConstructionStateInformation()
    {
        // Ajout des informations relative à la map.
        StateUpdateObject mapID;
        int id = CurrentWorldState.GetData<BloodAndBileEngine.WorldState.Map>().ID;
        mapID = new StateUpdateObject("MAP_ID", id);

        return new StateUpdateObject[] { mapID };
    }

}
