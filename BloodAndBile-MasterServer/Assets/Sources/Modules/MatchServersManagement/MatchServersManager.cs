using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BloodAndBileEngine.Networking;
using BloodAndBileEngine.Networking.NetworkMessages;

/**
 * <summary> Module de gestion des Match Servers.
 * 
 * D'une manière similaire aux Clients, les Match Servers se connectent au Master Server. 
 * Lorsqu'un Match Server est connecté à un Master Server, son IP sera susceptible d'être envoyée à un joueur qui lui même sera susceptibe de
 * s'y connecter si le ping est satisfaisant. </summary>
 */
public class MatchServersManagerModule : IMasterServerModule
{
    HandlersManager Handlers;

    Dictionary<int, string> MatchServerIPs;
    List<int> DisconnectedMatchServers;

    bool Activated = false;

    string MatchServerPassword = "DevMatchServer";

    public void Init()
    {
        Handlers = new HandlersManager();
        MatchServerIPs = new Dictionary<int, string>();
        DisconnectedMatchServers = new List<int>();
        NetworkSocket.RegisterOnDisconnectionCallback(OnDisconnection);
    }

    public void Activate()
    {
        if (Activated) { BloodAndBileEngine.Debugger.Log("Le module Match Servers est déjà activé !", Color.yellow); return; }
        Activated = true;
        // Création des handlers. Ils sont supprimés lors de la désactivation donc il faut les re-créer à chaque activation.
        Handlers.Add<AuthentificationMessage>(60000, OnAuthentificationRequest);
        Handlers.Add<NetworkMessage>(0, OnMatchServersListRequestReceived);
        //_________________________________________________________________________________________________________________
    }

    public void Deactivate()
    {
        if (!Activated) { BloodAndBileEngine.Debugger.Log("Le module Match Servers est déjà désactivé !", Color.yellow); return; }
        Activated = false;
        Handlers.Clear(); // On ne veut pas que le module réagisse aux messages reçus, donc on supprime tous les handlers.
        foreach (int coID in MatchServerIPs.Keys) // Déconnexion de tous les match servers.
        {
            NetworkSocket.Disconnect(coID);
        }
        MatchServerIPs.Clear();
    }

    public void Update()
    {
        UpdateDisconnectedMatchServers();
    }

    /**
     * <summary> Met à jour les match servers déconnectés en supprimant toutes les IDs de connexion se trouvant dans DisconnectedClients de
     * OnlineClients. </summary>
     */
    void UpdateDisconnectedMatchServers()
    {
        foreach (int coID in DisconnectedMatchServers)
        {
            if (MatchServerIPs.ContainsKey(coID))
            {
                NetworkSocket.Disconnect(coID);
                MatchServerIPs.Remove(coID);
                BloodAndBileEngine.Debugger.Log("Match Server retiré ! ID : " + coID, Color.yellow);
            }
        }

        DisconnectedMatchServers.Clear();
    }

    // HANDLERS

    /**
    * <summary> Déclenché lorsqu'une requête d'authentification est reçue. </summary>
    */
    void OnAuthentificationRequest(BloodAndBileEngine.Networking.NetworkMessageInfo info, AuthentificationMessage message)
    {
        if (message.AuthType != AuthentificationType.MATCH_SERVER) return; // Si le type d'authentification n'est pas "Match_Server", on ne fait rien.

        // On vérifie que cette IP ne soit pas déjà occupée.
        string IP;
        if (message.IP == "")
        {
            IP = NetworkSocket.GetConnectionInfoFromConnectionID(info.ConnectionID).IP.Split(':')[3];
        }
        else IP = message.IP;

        BloodAndBileEngine.Debugger.Log("Tentative de connexion d'un Match Server avec l'IP " + IP);

        if (MatchServerIPs.ContainsKey(info.ConnectionID))
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - Cette ID de connexion est déjà occupée !", Color.red);
            SendAuthentificationResponse(false, "ID de connexion déjà prit !", info.ConnectionID);
        }
        else if (MatchServerIPs.ContainsValue(IP))
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - Cette IP est déjà répertoriée !", Color.red);
            SendAuthentificationResponse(false, "IP déjà prise !", info.ConnectionID);
        }
        else if (message.Password == MatchServerPassword)
        {
            MatchServerIPs.Add(info.ConnectionID, IP);
            BloodAndBileEngine.Debugger.Log("Ajout d'un Match Server : " + IP);
            SendAuthentificationResponse(true, "", info.ConnectionID);
        }



    }

    /**
     * <summary> Envoi d'un message de réponse à une demande d'authentification. </summary>
     */
    void SendAuthentificationResponse(bool accepted, string reason, int coID)
    {
        MessageSender.Send(new ConditionCheckResponseMessage(accepted, reason), coID, 0);
    }

    void OnDisconnection(int coID)
    {
        if (MatchServerIPs.ContainsKey(coID))
        {
            DisconnectedMatchServers.Add(coID);
        }
    }

    void OnMatchServersListRequestReceived(BloodAndBileEngine.Networking.NetworkMessageInfo info, NetworkMessage msg)
    {
        BloodAndBileEngine.Debugger.Log("Demande de transfert de la liste des Match Servers reçue ! Envoie...");
        List<string> IPs = new List<string>();
        foreach( string IP in MatchServerIPs.Values)
        {
            IPs.Add(IP);
        }

        MessageSender.Send(new IPListMessage(IPs.ToArray()), info.ConnectionID, 0);
    }

}
