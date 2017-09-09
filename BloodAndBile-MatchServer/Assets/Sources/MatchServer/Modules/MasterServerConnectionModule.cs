using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Ce module maintient une connexion au Master Server à l'IP qui lui est donnée.
 * L'ID de connexion est accessible au niveau statique. </summary>
 */ 
public class MasterServerConnectionModule : IMatchServerModule
{

    static int MasterServerConnectionID = -1;
    public static int GetMasterServerConnectionID()
    {
        return MasterServerConnectionID;
    }

    BloodAndBileEngine.InputHandlersManager InputHandlers;
    BloodAndBileEngine.Networking.HandlersManager NetworkHandlers;

    bool Activated = false;
    bool Connected = false;
    bool Connecting = false;

    string MasterServerIP = "127.0.0.1"; // IP par défaut : local.
    string MasterServerPassword = "DevMatchServer"; // Mot de passe par défaut.
    string MyIP = "109.24.189.54"; // IP de Match Server par défaut.

    void SetMasterServerInfos(object[] args)
    {
        if (args.Length > 2)
        {
            MasterServerIP = (string)args[0];
            MasterServerPassword = (string)args[1];
            MyIP = (string)args[2];
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - Pas assez d'arguments pour SetMasterServerInfos ! Il faut IP + Mot de passe + IP du match server !", UnityEngine.Color.red);
        }
    }

    public void Initialise()
    {
        InputHandlers = new BloodAndBileEngine.InputHandlersManager();
        
    }

    public void Activate()
    {
        if (Activated) { BloodAndBileEngine.Debugger.Log("Le module MasterServerConnectionModule est déjà activé !", UnityEngine.Color.red); return; }

        Activated = true;
        InputHandlers.Add("SetMasterServerInfos", SetMasterServerInfos);
        InputHandlers.Add("LoginToMasterServer", ConnectToMasterServer);

        NetworkHandlers = new BloodAndBileEngine.Networking.HandlersManager();
        NetworkHandlers.Add<BloodAndBileEngine.Networking.NetworkMessages.ConditionCheckResponseMessage>(60001, OnAuthentificationResponseReceived);

        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnConnectionEstablishedCallback(OnConnectionEstablished);
        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnConnectionLost);
    }

    public void Deactivate()
    {
        Activated = false;
        Connected = false;
        Connecting = false;

        if (MasterServerConnectionID > -1) // Déconnexion du Master Server.
        BloodAndBileEngine.Networking.NetworkSocket.Disconnect(MasterServerConnectionID);

        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnConnectionEstablishedCallback(OnConnectionEstablished);
        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnConnectionLost);

        NetworkHandlers.Clear();
        InputHandlers.Clear();
    }

    public void Update()
    {
        if (Activated && !Connecting) ConnectToMasterServer(null);
    }

    void ConnectToMasterServer(object[] args)
    {
        if (!Connecting)
        {
            BloodAndBileEngine.Debugger.Log("Tentative de connexion au Master Server !");
            MasterServerConnectionID = BloodAndBileEngine.Networking.NetworkSocket.ConnectTo(MasterServerIP, 25001);
            Connecting = true;
        }
        
    }

    void SendAuthentificationRequest(int masterServerCoID)
    {
        BloodAndBileEngine.Networking.MessageSender.Send(new BloodAndBileEngine.Networking.NetworkMessages.AuthentificationMessage(BloodAndBileEngine.Networking.NetworkMessages.AuthentificationType.MATCH_SERVER, "MatchServer", MasterServerPassword, MyIP), masterServerCoID, 0);
    }

    void OnAuthentificationResponseReceived(BloodAndBileEngine.Networking.NetworkMessageInfo info, BloodAndBileEngine.Networking.NetworkMessages.ConditionCheckResponseMessage msg)
    {
        if (msg.Accepted)
        {
            BloodAndBileEngine.Debugger.Log("Authentification au Master Server acceptée !");
            Connected = true;
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("Authentification au Master Server refusée : " + msg.Reason, UnityEngine.Color.red);
            BloodAndBileEngine.Networking.NetworkSocket.Disconnect(MasterServerConnectionID); // Déconnexion du Master Server.
        }
    }

    /**
     * <summary> Connexion établie. On vérifie si l'ID de connexion est celle de la connexion avec le Master Server.
     * Si c'est le cas, alors on envoi une demande d'authentification à cette ID là. </summary>
     */ 
    void OnConnectionEstablished(int coID)
    {
        if (coID == MasterServerConnectionID)
        {
            BloodAndBileEngine.Debugger.Log("Envoie de la demande d'authentification au Master Server");
            SendAuthentificationRequest(coID);
        }
    }

    /**
     * <summary> Connexion perdue ! Si c'est avec le Master Server, on repasse "Connected" et "Connecting" à false et on retente de s'y connecter. </summary>
     */ 
    void OnConnectionLost(int coID)
    {
        Connected = false;
        Connecting = false;

        MasterServerConnectionID = -1;
    }
}
