using System;
using System.Collections.Generic;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Module de match s'occupant de mettre à jour l'ensemble des entités du WorldState lié au match et de construire
/// le StateUpdate contenant les informations importantes sur les entités à travers le système de synchronisation des entités.
/// S'occupe également de gérer l'envoi aux clients des entités détruites et créées.
/// </summary>
public class EntitiesManagerModule : MatchModule, IStateUpdater
{
    // Liste de toutes les entités en vie dans ce match.
    List<BloodAndBileEngine.Entity> LivingEntities = new List<BloodAndBileEngine.Entity>();
    List<PlayerToEntityLink> PlayerControlledEntities = new List<PlayerToEntityLink>();
    
    // Structure symbolisant le lien de contrôle entre un joueur (ID de connexion) et une entité.
    public class PlayerToEntityLink
    {
        public int ConnectionID;
        public BloodAndBileEngine.Entity Entity;
        public PlayerToEntityLink(int coID, BloodAndBileEngine.Entity e)
        {
            ConnectionID = coID;
            Entity = e;
        }
    }
    
    public EntitiesManagerModule(Match match) : base(match)
    {
    }

    BloodAndBileEngine.WorldState.WorldState GetWorldState()
    {
        return ModuleMatch.GetModule<MapStateModule>().GetWorldState();
    }

    // Gestion de la création des entités

    BloodAndBileEngine.WorldState.WorldStateData.WorldEntityFactory EntityFactory;

    public BloodAndBileEngine.Entity SpawnPlayer(int playerConnectionID, BloodAndBileEngine.WorldState.Cell cell)
    {
        BloodAndBileEngine.Debugger.Log("SpawnPlayer aux coordonnées " + cell.GetPosition());
        BloodAndBileEngine.Entity player = EntityFactory.BuildPlayer(cell.GetPosition(), UnityEngine.Quaternion.identity, 0.5f, 2.0f);
        return player;
    }


    void SpawnPlayers()
    {

        //Création des joueurs, placement sur la map, et assignation d'un connexionID pour chaque

        List<int> usedCells = new List<int>();
        foreach (int coId in ModuleMatch.GetPlayerConnectionIDs())
        {
            BloodAndBileEngine.WorldState.Cell cell = ModuleMatch.GetModule<MapStateModule>().GetWorldState().GetData<BloodAndBileEngine.WorldState.CellSystem>().FindSpawnPoint(usedCells.ToArray());
            if (cell != null)
            {
                usedCells.Add(cell.ID);
                BloodAndBileEngine.Entity player = SpawnPlayer(coId, cell);
                SetControlledEntity(coId, player);
                BloodAndBileEngine.Debugger.Log("Entité joueur d'ID " + player.ID + " associée à la connexion d'ID " + coId);
            }
            else
                BloodAndBileEngine.Debugger.Log("Aucun point de spawn trouvé !");
        }
    }
    
    //

    public BloodAndBileEngine.Entity GetControlledEntity(int connectionID)
    {
        foreach(PlayerToEntityLink link in PlayerControlledEntities)
        {
            if (link.ConnectionID == connectionID)
            {
                return link.Entity;
            }
        }
        return null;
    }

    public void SetControlledEntity(int connectionID, BloodAndBileEngine.Entity entity)
    {
        BloodAndBileEngine.Debugger.Log("Le joueur " + connectionID + " contrôle désormais l'entité " + entity.ID);
        bool linkExists = false;
        foreach(PlayerToEntityLink link in PlayerControlledEntities)
        {
            if (link.ConnectionID == connectionID) { link.Entity = entity; linkExists = true; }
        }
        if (!linkExists)
        {
            BloodAndBileEngine.Debugger.Log("Lien crée...");
            PlayerControlledEntities.Add(new PlayerToEntityLink(connectionID, entity));
        }
    }

    public PlayerToEntityLink[] GetPlayerControlledEntityLinks()
    {
        return PlayerControlledEntities.ToArray();
    }

    public override void Initialise()
    {
        base.Initialise();
        EntityFactory = ModuleMatch.GetModule<MapStateModule>().GetWorldState().GetData<BloodAndBileEngine.WorldState.WorldStateData.WorldEntityFactory>();
        EntityFactory.RegisterOnEntityCreatedCallback(OnEntitySpawned);
        SpawnPlayers();
    }

    float ControlledEntityRefreshPeriod = 5f;
    float CurrentControlledEntityRefreshTimer = 0f;
    public override void Update(float deltaTime)
    {
        CurrentControlledEntityRefreshTimer += deltaTime;
        if (CurrentControlledEntityRefreshTimer > ControlledEntityRefreshPeriod)
        {
            CurrentControlledEntityRefreshTimer = 0f;
            foreach(PlayerToEntityLink link in PlayerControlledEntities)
            {
                BloodAndBileEngine.InputManager.SendCommand("NetCommand", link.ConnectionID, "SetControlledEntity", link.Entity.ID);
            }
        }
    }

    public override void Stop()
    {
        base.Stop();
        EntityFactory.RemoveOnEntityCreatedCallback(OnEntitySpawned);
    }

    // Lance une mise à jour de chaque EntitySynchroniserComponent et regroupe leurs
    // EntitySynchronizationDataObjects dans un objet StateUpdateObject portant le nom "EntitySynchronization".
    // Renvoi également un StateUpdateObject "CreatedEntities" et un StateUpdateObject "DestroyedEntities".
    public StateUpdateObject[] GetStateUpdateInformation()
    {
        StateUpdateObject EntitySyncObject = new StateUpdateObject("EntitySynchronization", null);
        List<BloodAndBileEngine.EntitySynchronizationDataObject> SyncDataObjectList = new List<BloodAndBileEngine.EntitySynchronizationDataObject>();
        foreach (BloodAndBileEngine.Entity entity in LivingEntities)
        {
            BloodAndBileEngine.EntitySynchroniserComponent syncComponent = (BloodAndBileEngine.EntitySynchroniserComponent)entity.GetComponent(typeof(BloodAndBileEngine.EntitySynchroniserComponent));
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
        return new StateUpdateObject[0];
    }

    /// <summary>
    /// Provoque la destruction de l'entité contrôlée par le joueur.
    /// </summary>
    public override void OnPlayerDisconnected(int coID)
    {
        BloodAndBileEngine.Entity entity = GetControlledEntity(coID);
        if (entity != null) entity.Destroy();
    }

    /// <summary>
    /// Exécutée dès qu'une entité est instanciée dans ce match.
    /// </summary>
    void OnEntitySpawned(BloodAndBileEngine.Entity entity)
    {
        entity.RegisterOnEntityDestroyedCallback(OnEntityDestroyed);
        LivingEntities.Add(entity);
        // Envoi de la commande aux clients
        ModuleMatch.SendCommandToPlayers("CreateEntity", entity.ID);
    }

    void OnEntityDestroyed(BloodAndBileEngine.Entity entity)
    {
        BloodAndBileEngine.Debugger.Log("EntitiesStateModule : Entité " + entity.ID + " détruite !");
        LivingEntities.Remove(entity);
        ModuleMatch.SendCommandToPlayers("KillEntity", entity.ID);
        PlayerToEntityLink destroyedLink = null;
        foreach (PlayerToEntityLink link in PlayerControlledEntities)
        {
            if (link.Entity == entity)
            {
                destroyedLink = link;
            }
        }
        if (destroyedLink != null)
        {
            BloodAndBileEngine.Debugger.Log("Destruction de l'entité joueur " + entity.ID, UnityEngine.Color.yellow);
            PlayerControlledEntities.Remove(destroyedLink);
        }
    }
}
