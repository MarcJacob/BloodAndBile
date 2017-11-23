using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine;
using BloodAndBileEngine.Networking;


/**
 * <summary> Etat dans lequel se trouve le client au lancement de l'application. Se contente d'attendre
 * que le joueur envoi une demande d'identification à un Master Server avec succès. 
 * 
 * Pour sortir de cet état :
 * Se connecter à un Master Server -> MainMenuState. </summary>
 */ 
public class LoginState : IClientState
{
    InputHandlersManager InputHandlers = new InputHandlersManager();
    Dictionary<string, string> MasterServers = new Dictionary<string, string>(); // TODO : Charger ce dictionnaire avec un fichier de config.

    HandlersManager NetworkHandlers = new HandlersManager();

    string Username; // Nom d'utilisateur avec lequel on souhaite se connecter.
    string Password; // Mot de passe qu'on tente d'utiliser.

    // FONCTIONS DE CLIENTSTATE
    public void OnEntry()
    {
        if (BloodAndBileEngine.WorldState.Map.Maps == null)
            BloodAndBileEngine.WorldState.Map.LoadMaps();
        InputHandlers.Add("Login", Login);
        InputHandlers.Add("LoginToMatchServer", LoginToMatchServer); // Permet de rapidement tester le Match Server (et surtout de ne pas avoir besoin de lancer un Master Server.)
        NetworkSocket.RegisterOnConnectionEstablishedCallback(OnConnection);

        NetworkHandlers.Add<BloodAndBileEngine.Networking.NetworkMessages.ConditionCheckResponseMessage>(60001, OnAuthentificationResponse);
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        NetworkSocket.UnregisterOnConnectionEstablishedCallback(OnConnection);

        NetworkHandlers.Clear();
        InputHandlers.Clear();
    }
    //________________________

    /**
        * <summary> Tente une connexion vers le master server. Si la connexion réussie, alors une tentative d'authentification sera
        * effectuée. </summary>
        */ 
    void Login(object[] parameters)
    {
        if (parameters.Length > 2)
        {
            Username = (string)parameters[1];
            Password = (string)parameters[2];
        }
        if (CheckCredentialsConditions())
        {
            NetworkSocket.ConnectTo((string)parameters[0], 25001);
        }
        else
        {
            Debugger.Log("Informations de compte invalides !", UnityEngine.Color.red);
        }
    }

    void LoginToMatchServer(object[] parameters)
    {
        if (parameters.Length < 2)
        {
            Debugger.Log("ERREUR - LoginToMasterServer demande 2 arguments : l'IP du match server et le nom du joueur.");
            return;
        }

        string name = (string)parameters[0];
        string IP = (string)parameters[1];

        Client.Username = name;
        Client.ChangeState(new MatchmakingState(IP));
    }

    void OnConnection(int coID)
    {
        // Connexion établie ! Très probablement avec le Master Server. Pour être sur, on envoi une demande d'authentification : dans l'état actuel,
        // seul le Master Server va y répondre favorablement.

        MessageSender.Send(new BloodAndBileEngine.Networking.NetworkMessages.AuthentificationMessage(BloodAndBileEngine.Networking.NetworkMessages.AuthentificationType.CLIENT, Username, Password), coID, 0);
    }

    /**
     * <summary> Réponse à l'authentification. </summary>
     */ 
    void OnAuthentificationResponse(NetworkMessageInfo info, BloodAndBileEngine.Networking.NetworkMessages.ConditionCheckResponseMessage message)
    {
        if (message.Accepted)
        {
            Debugger.Log("Authentification au Master Server acceptée !");
            ClientConnectionsManager.AddConnection("MasterServer", info.ConnectionID);

            // Mise à jour des informations client.

            Client.Username = Username;

            // Transition vers le "MainMenuState".
            Client.ChangeState(new MainMenuState());
        }
        else
        {
            Debugger.Log("Authentification au Master Server refusée ! Raison : " + message.Reason, UnityEngine.Color.red);
            NetworkSocket.Disconnect(info.ConnectionID); // On se déconnecte du master server.
        }
    }

    /**
     * <summary> Charge la liste des IPs et noms des Master Servers.
     * TODO : La charger depuis un fichier de configuration. </summary>
     */ 
    void LoadMasterServerIPs()
    {
        MasterServers.Add("Development", "127.0.0.1"); // Serveur de développement, local.
    }

    /**
     * <summary> Vérifie si les informations de compte actuelles sont valables. </summary>
     */ 
    bool CheckCredentialsConditions()
    {
        /*List<bool> conditions = new List<bool>();
        // Pour ajouter une condition : Conditions.Add(<code qui renvoi un booléen>);

        conditions.Add(Username.Length > 2); // Nom d'utilisateur fait au moins 3 caractères de long.
        conditions.Add(Password.Length > 6); // Mot de passe fait au moins 6 caractères de long.
        conditions.Add(Password.ToLower() != Password); // Mot de passe contient au moins une majuscule

        //

        foreach(bool condition in conditions)
        {
            if (!condition) return false;
        }*/

        return true;
    }
}
