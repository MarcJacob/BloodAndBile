using System;
using System.Collections.Generic;
using BloodAndBileEngine.Networking;
using BloodAndBileEngine.Networking.NetworkMessages;
using UnityEngine;

/**
 * <summary> Module de gestion des clients.
 * 
 * Le Master Server prend en compte toutes les connexions entrantes et envoi un message de demande d'authentification.
 * La machine connecté répond en envoyant  un message d'authentification qui contient un nom, un mot de passe, et un type
 * d'authentification. Si c'est un Client, alors le nom et le mot de passe doivent correspondre à un compte. Si c'est un Match Server,
 * alors le nom correspond au nom du MatchServer et le mot de passe correspond au mot de passe MatchServer lié à ce MasterServer.
 * 
 * Ce module ne réagit qu'aux messages d'authentification de type "Client". </summary>
 */ 
public class ClientsManagerModule : IMasterServerModule
{
    HandlersManager Handlers;

    Dictionary<string, ClientAccountInfo> KnownClients; // Tous les clients connus, triés par leur ID. Inclu les hors ligne.
    Dictionary<int, ClientAccountInfo> OnlineClients; // Tous les clients connectés.

    List<int> DisconnectedClients; // Liste des clients déconnectés qui doivent être retirés de OnlineClients.

    bool Activated = false;

    public void Init()
    {
        Handlers = new HandlersManager();
        KnownClients = new Dictionary<string, ClientAccountInfo>(); // TODO : Ecrire une fonction pour charger cette liste à partir d'un fichier / base de données.
        OnlineClients = new Dictionary<int, ClientAccountInfo>();
        DisconnectedClients = new List<int>();

        NetworkSocket.RegisterOnDisconnectionCallback(OnDisconnection);
    }

    public void Activate()
    {
        if (Activated) { BloodAndBileEngine.Debugger.Log("Le module Clients est déjà activé !", Color.yellow); return; }
        Activated = true;
        // Création des handlers. Ils sont supprimés lors de la désactivation donc il faut les re-créer à chaque activation.
        Handlers.Add<AuthentificationMessage>(60000, OnAuthentificationRequest);

        //_________________________________________________________________________________________________________________
    }

    public void Deactivate()
    {
        if (!Activated) { BloodAndBileEngine.Debugger.Log("Le module Clients est déjà désactivé !", Color.yellow); return; }
        Activated = false;
        Handlers.Clear(); // On ne veut pas que le module réagisse aux messages reçus, donc on supprime tous les handlers.
        foreach(int coID in OnlineClients.Keys) // Déconnexion de tous les clients.
        {
            NetworkSocket.Disconnect(coID);
        }
        OnlineClients.Clear();
    }

    public void Update()
    {
        UpdateDisconnectedClients();
    }

    /**
     * <summary> Met à jour les clients déconnectés en supprimant toutes les IDs de connexion se trouvant dans DisconnectedClients de
     * OnlineClients. </summary>
     */ 
    void UpdateDisconnectedClients()
    {
        foreach(int coID in DisconnectedClients)
        {
            if (OnlineClients.ContainsKey(coID))
            {
                LogOff(OnlineClients[coID]);
            }
        }

        DisconnectedClients.Clear();
    }

    // HANDLERS

    /**
    * <summary> Déclenché lorsqu'une requête d'authentification est reçue. </summary>
    */ 
    void OnAuthentificationRequest(BloodAndBileEngine.Networking.NetworkMessageInfo info, AuthentificationMessage message)
    {
        if (message.AuthType != AuthentificationType.CLIENT) return; // Si le type d'authentification n'est pas "Client", on ne fait rien.

        BloodAndBileEngine.Debugger.Log("Requête d'authentification client reçue !", Color.yellow);
        BloodAndBileEngine.Debugger.Log("Vérification du compte...", Color.yellow);
        if (KnownClients.ContainsKey(message.Name))
        {
            BloodAndBileEngine.Debugger.Log("Compte existant : " + message.Name);
            BloodAndBileEngine.Debugger.Log("Vérification du mot de passe...");
            if (KnownClients[message.Name].GetPassword() != message.Password)
            {
                SendAuthentificationResponse(false, "Mot de passe invalide !", info.ConnectionID);
            }
            else
            {
                SendAuthentificationResponse(true, "", info.ConnectionID);
                LogIn(KnownClients[message.Name], info.ConnectionID);
            }
        }
        else // Le compte n'existe pas ! On en crée un nouveau puis on le connecte.
        {
            BloodAndBileEngine.Debugger.Log("Compte inconnu ! Création...");
            KnownClients.Add(message.Name, new ClientAccountInfo(message.Name, message.Password, info.ConnectionID));
            LogIn(KnownClients[message.Name], info.ConnectionID);
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

    /**
     * <summary> Passe un client en mode "en ligne". </summary>
     */ 
    void LogIn(ClientAccountInfo client, int coID)
    {
        if (!OnlineClients.ContainsKey(coID))
        {
            client.Login(coID);
            OnlineClients.Add(coID, client);
            BloodAndBileEngine.Debugger.Log("Client " + client.GetUsername() + " connecté !");
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - Cette ID de connexion est déjà associée à un client !", Color.red);
        }
    }

    /**
     * <summary> Passe un client en mode "hors ligne". </summary>
     */ 
    void LogOff(ClientAccountInfo client)
    {
        if (client == null || !KnownClients.ContainsKey(client.GetUsername()) || client.GetConnectionID() == -1)
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - Ce client est déjà hors ligne ou n'existe pas !");
            return;
        }
        int coID = client.GetConnectionID();
        OnlineClients.Remove(coID);
        BloodAndBileEngine.Debugger.Log("Client " + client.GetUsername() + " déconnecté !", Color.yellow);
    }

    void OnDisconnection(int coID)
    {
        if (OnlineClients.ContainsKey(coID))
        {
            DisconnectedClients.Add(coID);
        }
    }


}

