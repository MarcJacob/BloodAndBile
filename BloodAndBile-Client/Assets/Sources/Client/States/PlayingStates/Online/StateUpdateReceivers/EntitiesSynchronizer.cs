using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class EntitiesSynchronizer : IStateUpdateReceiver
{
    BloodAndBileEngine.WorldState.WorldState LocalWorldState;

    public EntitiesSynchronizer(BloodAndBileEngine.WorldState.WorldState worldState)
    {
        LocalWorldState = worldState;
    }

    /// <summary>
    /// Création des entités créées, destruction des entités détruites,
    /// et synchronisation des autres.
    /// </summary>
    /// <param name="stateUpdate"></param>
    public void OnStateUpdate(BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateMessage stateUpdate)
    {
        // Destruction des entités :
        // La liste des entités détruites est passé en paramètre de la fonction "RemoveEntitiesFromID" du CellSystem
        // du WorldState actuel.
        // Création des entités :

        uint[] createdEntities = (uint[])stateUpdate.GetStateUpdateInfo("CreatedEntities")[0].Information;
        foreach(uint id in createdEntities)
        {
            BloodAndBileEngine.EntityFactories.EntityFactory.BuildEntity(LocalWorldState, id, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, 1f, 1f);
            BloodAndBileEngine.Debugger.Log("Created entity : " + id);
        }

        // Destruction des entités
        uint[] destroyedEntities = (uint[])stateUpdate.GetStateUpdateInfo("DestroyedEntities")[0].Information;
        LocalWorldState.GetData<BloodAndBileEngine.WorldState.CellSystem>().RemoveEntitiesFromID(destroyedEntities);

        // Synchronisation
        // Chaque EntitySynchronizationDataObject a un identifiant d'entité à synchroniser, il suffit donc
        // d'appliquer la synchronisation à l'entité en question.
        foreach(BloodAndBileEngine.EntitySynchronizationDataObject SynchData in (BloodAndBileEngine.EntitySynchronizationDataObject[])(stateUpdate.GetStateUpdateInfo("EntitySynchronization")[0].Information))
        {
            BloodAndBileEngine.Entity entity = BloodAndBileEngine.EntitiesManager.GetEntityFromID(SynchData.GetEntityID());
            BloodAndBileEngine.EntitySynchroniserComponent synchComponent = (BloodAndBileEngine.EntitySynchroniserComponent)entity.GetComponent(typeof(BloodAndBileEngine.EntitySynchroniserComponent));
            if (synchComponent != null)
            {
                synchComponent.GetSynchronizationData().SetSynchInfoFromSynchObject(SynchData);
                
                synchComponent.OnSynch();
            }
        }
    }

    public void OnStateConstruction(BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateConstructionMessage stateConstruction)
    {

    }
}

