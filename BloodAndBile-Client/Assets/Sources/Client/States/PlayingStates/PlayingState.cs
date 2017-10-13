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

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit()
    {

    }

    

}
