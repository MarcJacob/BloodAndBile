using System;
using System.Collections.Generic;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Module de match s'occupant de mettre à jour l'ensemble des entités du WorldState lié au match et de construire
/// le StateUpdate contenant les informations importantes sur les entités comme :
/// - Leur position
/// - Leur rotation
/// - Ce que les modules d'entité souhaitent ajouter.
/// (Ajouter à la liste au fur et à mesure).
/// </summary>
public class EntitiesStateModule : MatchModule, IStateUpdater
{
    List<int> CreatedEntitiesID; // Identifiant des entités créées depuis le dernier StateUpdate.


    public EntitiesStateModule(Match match) : base(match)
    {

    }

    public override void Initialise()
    {
        base.Initialise();
    }

    public override void Update()
    {
        base.Update();
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
        return new StateUpdateObject[] { EntitySyncObject };

    }

    public StateUpdateObject[] GetConstructionStateInformation()
    {
        // Pas d'informations de construction.
        return new StateUpdateObject[0];
    }
}
