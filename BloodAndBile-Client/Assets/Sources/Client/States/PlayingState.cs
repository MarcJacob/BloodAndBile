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


    BloodAndBileEngine.WorldState.WorldState LocalWorldState;
    bool UpdateLocalWorld = false;
    public PlayingState(bool updateLocalWorld = true)
    {
        UpdateLocalWorld = updateLocalWorld;
    }

    public void OnEntry()
    {
        LocalWorldState = new BloodAndBileEngine.WorldState.WorldState();
        // Initialisation du WorldState local.


    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }

    // Handlers

        // TODO : Déplacer les handlers dans une classe spécifique à la réception de messages depuis un matchserver,
        // afin que les handlers ne soient pas déclaré en cas de jeu en solo.

    /// <summary>
    /// A la réception d'un message de type "StateUpdate".
    /// Décripte le StateUpdate.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="message"></param>
    void OnStateUpdate(BloodAndBileEngine.Networking.NetworkMessageInfo info, StateUpdateMessage message)
    {
        StateUpdateObject[] entityPositionsUpdate = message.GetStateUpdateInfo(0);

    }

    void OnCellInfoReceived(BloodAndBileEngine.Networking.NetworkMessageInfo info, )
}
