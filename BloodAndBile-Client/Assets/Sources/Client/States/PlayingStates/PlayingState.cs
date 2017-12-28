using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Etat durant lequel le Client est en jeu. S'occupe d'afficher l'interface de jeu, et des mouvements de caméra.
/// Se prépare également à recevoir des messages de type "StateUpdate".
/// </summary>
public class PlayingState : IClientState
{

    protected BloodAndBileEngine.WorldState.WorldState LocalWorldState;
    protected BloodAndBileEngine.Entity ControlledEntity;
    public PlayingState()
    {
        BloodAndBileEngine.Debugger.Log("Initialisation du WorldState local...");
        LocalWorldState = new BloodAndBileEngine.WorldState.WorldState();
    }

    public virtual void OnEntry()
    {
        BloodAndBileEngine.Debugger.Log("Match commencé !");
        // Initialisation de la commande "SetControlledEntity".
        BloodAndBileEngine.InputManager.AddHandler("SetControlledEntity", SetControlledEntity);


    }

    public virtual void OnUpdate()
    {
        if (ControlledEntity != null)
        {
            if (ControlledEntity.Destroyed)
            {
                OnControlledEntityDeath();
            }
        }
    }

    public virtual void OnExit()
    {
        // Nettoyage de toutes les entités
        BloodAndBileEngine.EntitiesManager.Clear();
    }

    /// <summary>
    /// Recherche l'Actor contrôlant l'entité d'ID args[0] et lui assigne un script EntityController.
    /// </summary>
    void SetControlledEntity(object[] args)
    {
        BloodAndBileEngine.Debugger.Log("Prise de contrôle de l'entité " + args[0], UnityEngine.Color.magenta);
        Actor[] actors = UnityEngine.GameObject.FindObjectsOfType<Actor>();
        Actor act = null;
        int i = 0;
        uint entityID;
        if (args[0] is string)
            uint.TryParse((string)args[0], out entityID);
        else
            entityID = (uint)args[0];
        while (i < actors.Length && act == null)
        {
            if (actors[i].GetControlledEntity().ID == entityID)
            {
                act = actors[i];
                ControlledEntity = actors[i].GetControlledEntity();
            }
            i++;
        }

        if (act != null && act.GetComponent<EntityController>() == null)
        {
            act.gameObject.AddComponent<EntityController>();
        }
    }

    /// <summary>
    /// Exécutée à la mort de l'entité contrôlée.
    /// Provoque un changement d'état vers le menu principal et un "cleanup" de l'affichage de WorldState local.
    /// </summary>
    protected virtual void OnControlledEntityDeath()
    {
        EntityRenderer.OnCleanup();
    }
}
