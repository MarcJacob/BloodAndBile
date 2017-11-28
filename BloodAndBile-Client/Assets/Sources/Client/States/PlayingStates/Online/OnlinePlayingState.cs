using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Etat du jeu lors d'un match en ligne. Ne met pas à jour le WorldState local et est capable de gérer des
/// StateUpdateReceivers.
/// </summary>
public class OnlinePlayingState : PlayingState
{

    BloodAndBileEngine.Networking.HandlersManager NetworkHandlers;
    BloodAndBileEngine.PlayerControlCommandManager PlayerControlManager;
    int MatchServerConnectionID; // ID de connexion au MatchServer -> Permet de savoir quand le MatchServer n'est plus
    // connecté au client et de revenir au MainMenuState.

    public OnlinePlayingState(int matchServerConnectionID) : base()
    {
        MatchServerConnectionID = matchServerConnectionID;

        StateUpdateReceivers = new IStateUpdateReceiver[] // TODO : Déplacer l'initialisation des StateUpdateReceivers dans une classe Factory.
        {
            new MapStateUpdater(LocalWorldState),
            new EntitiesSynchronizer(LocalWorldState),
        };

        PlayerControlManager = new BloodAndBileEngine.PlayerControlCommandManager();
        PlayerControlManager.SetSendToNetworkHandler(matchServerConnectionID);
    }

    public override void OnEntry()
    {
        base.OnEntry();
        NetworkHandlers = new BloodAndBileEngine.Networking.HandlersManager();
        NetworkHandlers.Add<StateUpdateMessage>(20000, OnStateUpdate);
        NetworkHandlers.Add<StateConstructionMessage>(20002, OnStateConstruction);

        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnConnectionLost);

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
        NetworkHandlers.Clear();
        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnConnectionLost);
    }

    /// <summary>
    /// Executée quand la connexion à une autre machine est perdue.
    /// Vérifie si la machine avec laquelle on a perdu la connexion est celle sur laquelle était host le Match.
    /// Si c'est le cas, alors on revient au MainMenuState.
    /// </summary>
    /// <param name="coID"></param>
    void OnConnectionLost(int coID)
    {
        if (coID == MatchServerConnectionID)
        {
            BloodAndBileEngine.Debugger.Log("Connexion au MatchServer perdue ! Retour au MainMenuState.", UnityEngine.Color.red);
            Client.ChangeState(new MainMenuState());
        }
    }


    IStateUpdateReceiver[] StateUpdateReceivers;

    /// <summary>
    /// A la réception d'un message de type "StateUpdate".
    /// Pour chaque IStateUpdateReceiver dans StateUpdateReceivers,
    /// on lance la méthode "OnStateUpdate" qui permet de mettre à jour le WorldState avec les informations reçues.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="message"></param>
    void OnStateUpdate(BloodAndBileEngine.Networking.NetworkMessageInfo info, StateUpdateMessage message)
    {
        BloodAndBileEngine.Debugger.Log("Received StateUpdate");
        foreach(IStateUpdateReceiver receiver in StateUpdateReceivers)
        {
            receiver.OnStateUpdate(message);
        }
    }

    /// <summary>
    /// A la réception d'un message du type "StateConstruction"
    /// Pour chaque IStateUpdateReceiver dans StateUpdateReceivers,
    /// on lance la méthode "OnStateConstruction" qui permet de construire le WorldState avec les informations reçues.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="message"></param>
    void OnStateConstruction(BloodAndBileEngine.Networking.NetworkMessageInfo info, StateConstructionMessage message)
    {
        foreach (IStateUpdateReceiver receiver in StateUpdateReceivers)
        {
            receiver.OnStateConstruction(message);
        }
    }
}
