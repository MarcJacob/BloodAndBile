using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Etat du client durant lequel il se connecte à un MatchServer et attend que celui ci l'assigne à un match. </summary>
 */ 
public class MatchmakingState : IClientState
{
    /**
     * <summary> Le Matchmaking State doit être construit avec l'IP d'un Match Server. En principe cette IP est trouvée dans le MatchServerSearchState. </summary>
     */ 
    public MatchmakingState(string matchServerIP)
    {
        if (matchServerIP.Length < 8)
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - MatchmakingState - IP de Match Server non valide !", UnityEngine.Color.red);
            MatchServerIP = "";
        }
        else
        MatchServerIP = matchServerIP;
    }

    BloodAndBileEngine.Networking.HandlersManager NetworkHandlers;

    BloodAndBileEngine.InputHandlersManager InputHandlers;

    string MatchServerIP;
    int MatchServerConnectionID;

    bool MatchServerConnected = false;


    public void OnEntry()
    {
        if (MatchServerIP == "")
        {
            // Aucune IP n'a été transmise. On le signal sur la console puis on revient au menu principal.

            BloodAndBileEngine.Debugger.Log("IP du MatchServer invalide... retour au menu principal.", UnityEngine.Color.red);
            Client.ChangeState(new MainMenuState());
            return;
        }
        else
        {
            NetworkHandlers = new BloodAndBileEngine.Networking.HandlersManager();
            InputHandlers = new BloodAndBileEngine.InputHandlersManager();

            // Handlers

            BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnConnectionLost);
            BloodAndBileEngine.Networking.NetworkSocket.RegisterOnConnectionEstablishedCallback(OnConnectionEstablished);
            NetworkHandlers.Add<BloodAndBileEngine.Networking.NetworkMessage>(20001, OnMatchStarted);
            NetworkHandlers.Add<BloodAndBileEngine.Networking.NetworkMessages.ConditionCheckResponseMessage>(60001, OnAuthentificationResponseReceived);

            ConnectToMatchServer(); // Lancement de la connexion au Match Server.
        }
    }

    public void OnUpdate()
    {   

    }
    
    public void OnExit()
    {
        NetworkHandlers.Clear();
        InputHandlers.Clear();

        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnConnectionLost);
        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnConnectionEstablishedCallback(OnConnectionEstablished);
    }

    // Lance une tentative de connexion vers le Match Server à l'IP donnée puis envoi une demande d'authentification.
    void ConnectToMatchServer()
    {
        BloodAndBileEngine.Debugger.Log("Connexion au Match Server...");
        // Connexion & obtention de l'ID de connexion.
        MatchServerConnectionID =  BloodAndBileEngine.Networking.NetworkSocket.ConnectTo(MatchServerIP, 25000);

    }

    /**
     * <summary> Connexion perdue. Si cela concerne la connexion avec le Match Server, alors on revient au MatchServerSearchState. </summary>
     */ 
    void OnConnectionLost(int coID)
    {
        if (coID == MatchServerConnectionID)
        {
            BloodAndBileEngine.Debugger.Log("Connexion au Match Server choisi perdue... retour au MatchServerSearchState.", UnityEngine.Color.red);
            Client.ChangeState(new MatchServerSearchState());
        }
    }

    void OnConnectionEstablished(int coID)
    {
        if (coID == MatchServerConnectionID)
        {
            // Envoi de la demande d'authentification.
            BloodAndBileEngine.Debugger.Log("Envoie de la demande d'authentification au Match Server.");
            BloodAndBileEngine.Networking.NetworkMessages.AuthentificationMessage message;
            message = new BloodAndBileEngine.Networking.NetworkMessages.AuthentificationMessage(BloodAndBileEngine.Networking.NetworkMessages.AuthentificationType.CLIENT, Client.Username, "");
            BloodAndBileEngine.Networking.MessageSender.Send(message, MatchServerConnectionID, 0);
        }
    }

    void OnAuthentificationResponseReceived(BloodAndBileEngine.Networking.NetworkMessageInfo info, BloodAndBileEngine.Networking.NetworkMessages.ConditionCheckResponseMessage message)
    {
        if (message.Accepted)
        {
            BloodAndBileEngine.Debugger.Log("Authentification au Match Server acceptée !");
            BloodAndBileEngine.Debugger.Log("Attente d'un Match.");
            MatchServerConnected = true;
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("Authentification au Match Server refusée : " + message.Reason, UnityEngine.Color.red);
            BloodAndBileEngine.Debugger.Log("Retour au MatchServerSearchState...", UnityEngine.Color.red);
            Client.ChangeState(new MatchServerSearchState());
        }
    }

    void OnMatchStarted(BloodAndBileEngine.Networking.NetworkMessageInfo info, BloodAndBileEngine.Networking.NetworkMessage message)
    {
        BloodAndBileEngine.Debugger.Log("Match started !");
        // Switch vers le PlayingState.

    }
}