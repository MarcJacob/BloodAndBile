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

        //Création des joueurs, placement sur la map, et assignation d'un connexionID pour chaque

        List<int> usedCells = new List<int>();
        foreach (int coId in ModuleMatch.GetPlayerConnectionIDs())
        {
            BloodAndBileEngine.WorldState.Cell cell = ModuleMatch.GetModule<MapStateModule>().GetWorldState().GetData<BloodAndBileEngine.WorldState.CellSystem>().FindSpawnPoint(usedCells.ToArray());
            if (cell != null)
            {
                usedCells.Add(cell.ID);
                BloodAndBileEngine.Entity player = BloodAndBileEngine.EntityFactories.EntityFactory.BuildPlayer(ModuleMatch.GetModule<MapStateModule>().GetWorldState(), cell.GetPosition(), UnityEngine.Quaternion.identity, 0.5f, 2.0f);
                ModuleMatch.SetPlayerEntity(coId, (uint)player.ID);
                CreatedEntitiesID.Add(player.ID);
                BloodAndBileEngine.Debugger.Log("Entité joueur d'ID " + player.ID + " associée à la connexion d'ID " + coId);
            }
            else
                BloodAndBileEngine.Debugger.Log("Aucun point de spawn trouvé !");
        }
    }

    float ControlledEntityRefreshPeriod = 5f;
    float CurrentControlledEntityRefreshTimer = 0f;
    public override void Update(float deltaTime)
    {
        CurrentControlledEntityRefreshTimer += deltaTime;
        if (CurrentControlledEntityRefreshTimer > ControlledEntityRefreshPeriod)
        {
            BloodAndBileEngine.Debugger.Log("Mise à jour des entités contrôlées par les clients...");
            CurrentControlledEntityRefreshTimer = 0f;
            foreach(int coID in ModuleMatch.GetPlayerConnectionIDs())
            {
                BloodAndBileEngine.InputManager.SendCommand("NetCommand", coID, "SetControlledEntity", ModuleMatch.GetControlledEntityID(coID));
            }
        }
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
        foreach (BloodAndBileEngine.Entity entity in ModuleMatch.GetModule<MapStateModule>().GetWorldState().GetData<BloodAndBileEngine.WorldState.CellSystem>().GetAllEntities())
        {
            BloodAndBileEngine.EntitySynchroniserComponent syncComponent = (BloodAndBileEngine.EntitySynchroniserComponent)entity.GetComponent(typeof(BloodAndBileEngine.EntitySynchroniserComponent));
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
