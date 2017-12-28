using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class EntitiesSynchronizer : IStateUpdateReceiver
{
    BloodAndBileEngine.WorldState.WorldState LocalWorldState;

    BloodAndBileEngine.WorldState.WorldStateData.WorldEntityFactory EntityFactory;

    public EntitiesSynchronizer(BloodAndBileEngine.WorldState.WorldState worldState)
    {
        LocalWorldState = worldState;
        EntityFactory = new BloodAndBileEngine.WorldState.WorldStateData.WorldEntityFactory(LocalWorldState);
        LocalWorldState.AddData(EntityFactory);
        BloodAndBileEngine.InputManager.AddHandler("CreateEntity", OnCreateEntityCommand);
        BloodAndBileEngine.InputManager.AddHandler("DestroyEntity", OnDestroyEntityCommand);
    }
    ~EntitiesSynchronizer()
    {
        BloodAndBileEngine.InputManager.RemoveHandler("CreateEntity", OnCreateEntityCommand);
    }
    /// <summary>
    /// Appelée lorsqu'une commande "CreateEntity" a été reçue du serveur.
    /// </summary>
    public void OnCreateEntityCommand(params object[] args)
    {
        if (args.Length > 0)
        {
            uint id;
            if (!(args[0] is uint))
            {
                if (args[0] is string)
                {
                    if (!uint.TryParse((string)args[0], out id))
                    {
                        BloodAndBileEngine.Debugger.Log("ERREUR : l'ID de l'entité à créer n'est pas un nombre !", UnityEngine.Color.red);
                        return;
                    }
                }
                else
                {
                    BloodAndBileEngine.Debugger.Log("ERREUR : l'ID de l'entité à créer doit être de type uint ou string !", UnityEngine.Color.red);
                    return;
                }
            }
            else
            {
                id = (uint)args[0];
            }

            // Si on arrive ici, alors id contient une valeur valide.
            BloodAndBileEngine.Entity entity = EntityFactory.BuildEntity(id, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, 1.0f, 1.0f);
            if (entity != null)
                EntityRenderer.OnEntityCreation(id);
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("ERREUR : La commande 'CreateEntity' prend un paramètre !", UnityEngine.Color.red);
        }
    }

    public void OnDestroyEntityCommand(params object[] args)
    {
        if (args.Length > 0)
        {
            uint id;
            if (!(args[0] is uint))
            {
                if (args[0] is string)
                {
                    if (!uint.TryParse((string)args[0], out id))
                    {
                        BloodAndBileEngine.Debugger.Log("ERREUR : l'ID de l'entité à détruire n'est pas un nombre !", UnityEngine.Color.red);
                        return;
                    }
                }
                else
                {
                    BloodAndBileEngine.Debugger.Log("ERREUR : l'ID de l'entité à détruire doit être de type uint ou string !", UnityEngine.Color.red);
                    return;
                }
            }
            else
            {
                id = (uint)args[0];
            }

            // Si on arrive ici, alors id contient une valeur valide.

            BloodAndBileEngine.Entity entity = BloodAndBileEngine.EntitiesManager.GetEntityFromID(id);
            entity.Destroy();
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("ERREUR : La commande 'DestroyEntity' prend un paramètre !", UnityEngine.Color.red);
        }
    }

    /// <summary>
    /// Création des entités créées, destruction des entités détruites,
    /// et synchronisation des autres.
    /// </summary>
    /// <param name="stateUpdate"></param>
    public void OnStateUpdate(BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateMessage stateUpdate)
    {
        // Synchronisation
        // Chaque EntitySynchronizationDataObject a un identifiant d'entité à synchroniser, il suffit donc
        // d'appliquer la synchronisation à l'entité en question.
        BloodAndBileEngine.EntitySynchronizationDataObject[] synchObject = (BloodAndBileEngine.EntitySynchronizationDataObject[])(stateUpdate.GetStateUpdateInfo("EntitySynchronization")[0].Information);
        if (synchObject == null || synchObject.Length == 0)
        {
            BloodAndBileEngine.Debugger.Log("ERREUR : pas de EntitySynchronizationDataObject !", UnityEngine.Color.red);
        }
        foreach(BloodAndBileEngine.EntitySynchronizationDataObject SynchData in synchObject)
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

