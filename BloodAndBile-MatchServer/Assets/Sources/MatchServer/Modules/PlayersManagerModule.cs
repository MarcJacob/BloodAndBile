using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BloodAndBileEngine.Networking;
using BloodAndBileEngine.Networking.NetworkMessages;

/**
 * <summary> Le PlayersManagerModule s'occupe de réceptionner les connexions Clients et maintient une liste des joueurs connectés. </summary>
 */
public class PlayersManagerModule : IMatchServerModule
{

    BloodAndBileEngine.PlayerControlCommandManager PlayerControlManager;

    Dictionary<int, string> ConnectedPlayers; // Joueurs connectés. Relie un ID de connexion à un nom de joueur.
    List<int> MatchmakingQueue; // Ensemble des joueurs attendant un match.
    public string GetPlayerNameFromID(int coID)
    {
        if (ConnectedPlayers.ContainsKey(coID))
            return ConnectedPlayers[coID];
        else
            return "";
    }

    public int[] GetConnectedPlayerIDs()
    {
        List<int> ids = new List<int>() ;
        foreach(int id in ConnectedPlayers.Keys)
        {
            ids.Add(id);
        }

        return ids.ToArray();
    }

    public int[] GetMatchmakingQueue()
    {
        return MatchmakingQueue.ToArray();
    }

    public void RemovePlayerFromMatchmakingQueue(int coID)
    {
        MatchmakingQueue.Remove(coID);
    }

    HandlersManager NetworkHandlers;

    public void Initialise()
    {
        ConnectedPlayers = new Dictionary<int, string>();

        NetworkHandlers = new HandlersManager();

        MatchmakingQueue = new List<int>();

        PlayerControlManager = new BloodAndBileEngine.PlayerControlCommandManager();
        PlayerControlManager.SetExecuteLocally();
    }

    public void Activate()
    {
        // Handlers

        NetworkHandlers.Add<AuthentificationMessage>(60000, OnAuthentificationRequestReceived);

        NetworkSocket.RegisterOnDisconnectionCallback(OnConnectionLost);
    }

    public void Deactivate()
    {
        NetworkHandlers.Clear();

        NetworkSocket.UnregisterOnConnectionEstablishedCallback(OnConnectionLost);
    }

    public void Update()
    {

    }

    /**
     * <summary> Lorsque l'on recoit une demande d'authentification. Si l'ID de connexion n'est pas déjà prise et que le type d'authentification est "Client",
     * alors on accepte. </summary>
     */ 
    void OnAuthentificationRequestReceived(BloodAndBileEngine.Networking.NetworkMessageInfo info, AuthentificationMessage message)
    {
        BloodAndBileEngine.Debugger.Log("Demande d'authentification reçue !");
        if (message.AuthType == AuthentificationType.CLIENT)
        {
            if (ConnectedPlayers.ContainsKey(info.ConnectionID))
            {
                BloodAndBileEngine.Debugger.Log("Demande d'authentification par un ID de connexion déjà authentifié ! Refus.", Color.red);
                SendAuthentificationResponse(info.ConnectionID, false, "ID de connexion déjà occupée !");
            }
            else
            {
                BloodAndBileEngine.Debugger.Log("Demande d'authentification acceptée !");
                BloodAndBileEngine.Debugger.Log("Joueur connecté : " + message.Name + " à l'ID " + info.ConnectionID);
                SendAuthentificationResponse(info.ConnectionID, true);
                ConnectedPlayers.Add(info.ConnectionID, message.Name); // Ajout du joueur au dictionnaire des joueurs connectés.
                MatchmakingQueue.Add(info.ConnectionID);
            }
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("Demande d'authentification autre que Client reçue ! Refus.", Color.red);
            SendAuthentificationResponse(info.ConnectionID, false, "Type d'authentification erroné ! Seul les clients peuvent se connecter au Match Server.");
        }
    }

    void SendAuthentificationResponse(int coID, bool accepted, string reason = "")
    {
        MessageSender.Send(new ConditionCheckResponseMessage(accepted, reason), coID, 0);
    }

    /**
     * <summary> Connexion perdue. On retire l'ID du dictionnaire des joueurs connectés. Il y a une petite chance pour que cette déconnexion concerne
     * celle avec le Master Server, qui ne se trouve pas dans le dictionnaire ! </summary>
     */ 
    void OnConnectionLost(int coID)
    {
        if (ConnectedPlayers.ContainsKey(coID))
        {
            BloodAndBileEngine.Debugger.Log("Joueur " + ConnectedPlayers[coID] + " déconnecté. ID : " + coID, Color.yellow);
            ConnectedPlayers.Remove(coID);
        }
        if (MatchmakingQueue.Contains(coID))
        {
            MatchmakingQueue.Remove(coID);
        }
    }

    public void Stop()
    {

    }
}
