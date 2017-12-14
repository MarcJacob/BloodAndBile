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
    public PlayingState()
    {
        BloodAndBileEngine.Debugger.Log("Initialisation du WorldState local...");
        LocalWorldState = new BloodAndBileEngine.WorldState.WorldState();
    }

    public virtual void OnEntry()
    {
        BloodAndBileEngine.Debugger.Log("Match started !");
        // Initialisation du WorldState local.
        LocalWorldState = new BloodAndBileEngine.WorldState.WorldState();
        // Initialisation de la commande "SetControlledEntity".
        BloodAndBileEngine.InputManager.AddHandler("SetControlledEntity", SetControlledEntity);


    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit()
    {

    }

    /// <summary>
    /// Recherche l'Actor contrôlant l'entité d'ID args[0] et lui assigne un script EntityController.
    /// </summary>
    void SetControlledEntity(object[] args)
    {
        BloodAndBileEngine.Debugger.Log("Setting controlled entity to ID " + args[0], UnityEngine.Color.magenta);
        Actor[] actors = UnityEngine.GameObject.FindObjectsOfType<Actor>();
        Actor act = null;
        int i = 0;
        uint entityID;
        uint.TryParse((string)args[0], out entityID);
        while (i < actors.Length && act == null)
        {
            if (actors[i].GetControlledEntity().ID == entityID)
            {
                act = actors[i];
            }
            i++;
        }

        if (act != null && act.GetComponent<EntityController>() == null)
        {
            act.gameObject.AddComponent<EntityController>();
        }
    }

    

}
