using System;
using System.Collections.Generic;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Module de match s'occupant de mettre à jour l'ensemble des entités du WorldState lié au match et de construire
/// le StateUpdate contenant les informations importantes sur les entitésà travers le système de synchronisation des entités.
/// S'occupe également d'envoyer la liste des ID des entités crées et détruites depuis le dernier StateUpdate.
/// </summary>
public class EntitiesStateModule : MatchModule, IStateUpdater
{
    List<uint> CreatedEntitiesID = new List<uint>(); // Identifiants des entités créées depuis le dernier StateUpdate.
    List<uint> DestroyedEntitiesID = new List<uint>(); // Identifiants des entités détruites depuis le dernier StateUpdate.

    public EntitiesStateModule(Match match) : base(match)
    {

    }

    BloodAndBileEngine.WorldState.WorldState GetWorldState()
    {
        return ModuleMatch.GetModule<MapStateModule>().GetWorldState();
    }

    public override void Initialise()
    {
        base.Initialise();
        // Create a test entity
        BloodAndBileEngine.Debugger.Log("Created entity !", UnityEngine.Color.red);
        BloodAndBileEngine.Entity testEntity = BloodAndBileEngine.EntityFactories.EntityFactory.BuildEntity(GetWorldState(), 0, new UnityEngine.Vector3(1f, 0f, 1f), UnityEngine.Quaternion.identity, 1f, 1f);
        CreatedEntitiesID.Add(testEntity.ID);
        testEntity.AddComponent<BloodAndBileEngine.TestController>(new BloodAndBileEngine.TestController(testEntity));
    }

    public override void Update(float deltaTime)
    {
        BloodAndBileEngine.Debugger.Log("EntitiesStateUpdater.Update()");
    }

    public override void Stop()
    {
        base.Stop();
    }

    // Lance une mise à jour de chaque EntitySynchroniserComponent et regroupe leurs
    // EntitySynchronizationDataObjects dans un objet StateUpdateObject portant le nom "EntitySynchronization".
    // Renvoi également un StateUpdateObject "CreatedEntities" et un StateUpdateObject "DestroyedEntities".
    public StateUpdateObject[] GetStateUpdateInformation()
    {
        StateUpdateObject EntitySyncObject = new StateUpdateObject("EntitySynchronization", null);
        List<BloodAndBileEngine.EntitySynchronizationDataObject> SyncDataObjectList = new List<BloodAndBileEngine.EntitySynchronizationDataObject>();
        foreach(BloodAndBileEngine.Entity entity in ModuleMatch.GetModule<MapStateModule>().GetWorldState().GetData<BloodAndBileEngine.WorldState.CellSystem>().GetAllEntities())
        {
            BloodAndBileEngine.EntitySynchroniserComponent syncComponent = entity.GetComponent<BloodAndBileEngine.EntitySynchroniserComponent>();
            if (syncComponent != null)
            {
                syncComponent.Update(0f);
                SyncDataObjectList.Add(syncComponent.GetSynchronizationData());
            }
        }

        EntitySyncObject.Information = SyncDataObjectList.ToArray();

        StateUpdateObject CreatedEntities = new StateUpdateObject("CreatedEntities", CreatedEntitiesID.ToArray());
        StateUpdateObject DestroyedEntities = new StateUpdateObject("DestroyedEntities", DestroyedEntitiesID.ToArray());
        CreatedEntitiesID.Clear();
        DestroyedEntitiesID.Clear();
        return new StateUpdateObject[] { EntitySyncObject, CreatedEntities, DestroyedEntities };

    }

    public StateUpdateObject[] GetConstructionStateInformation()
    {
        // Pas d'informations de construction.
        return new StateUpdateObject[0];
    }
}
