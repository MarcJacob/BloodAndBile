using System;
using System.Collections.Generic;
using UnityEngine;
/**
 * <summary> Lorsque le client se trouve dans le menu principal. </summary>
 */ 
class MainMenuState : ClientState
{
    MatchInfo[] OpenMatches; // Liste des matchs ouverts, fournie par le Master Server.

    override public void Init()
    {
        // Switch vers le menu principal.
        UIManagement.SwitchToUI("MainMenu");

        InputManager.AddHandler("ConnectToMatch", ConnectToMatch);
        MessageReader.AddHandler(40003, OnMatchesListReceived);
        MessageReader.AddHandler(20001, OnMatchIdentificationResponseReceived);
        MessageReader.AddHandler(20000, OnMatchIdentificationRequestReceived);
    }


    float MatchListRequestCooldown = 5f;
    override public void Update()
    {
        // Si on est connecté au Master Server, lui demander la liste des matchs ouverts toutes les 5 secondes.
        if (MasterServerConnectionManager.IsConnected())
        {
            MatchListRequestCooldown -= Time.deltaTime;
            if (MatchListRequestCooldown < 0f)
            {
                MatchListRequestCooldown = 5f;
                MessageSender.Send(new NetworkMessage(3), MasterServerConnectionManager.GetMasterServerConnectionID());
            }
        }
    }

    void OnMatchesListReceived(NetworkMessageInfo info, NetworkMessage msg)
    {
        OpenMatches = ((MatchListMessage)msg).Infos;
        Debugger.Log("Liste des matchs reçue :", Color.yellow);
        if (OpenMatches.Length == 0)
        {
            Debugger.Log("Pas de matchs !", Color.yellow);
        }
        foreach(MatchInfo match in OpenMatches)
        {
            Debugger.Log("--- " + match.MatchName + " hosted by " + match.HostName + ", IP : " + match.HostIP + "Port : " + match.HostPort + "; Locked = " + match.Locked, Color.yellow);
        }
    }

    /**
     * <summary> A la réception d'une réponse d'identification. </summary>
     */ 
    void OnMatchIdentificationResponseReceived(NetworkMessageInfo info, NetworkMessage msg)
    {
        MatchConnectionResponseMessage message = (MatchConnectionResponseMessage)msg;

        if (message.Accepted)
        {
            Debugger.Log("Connecté au match !");
            Client.ChangeState(new PlayingState());
        }
        else
        {
            Debugger.Log("Connexion au match refusé - " + message.Reason, Color.red);
        }
    }

    void OnMatchIdentificationRequestReceived(NetworkMessageInfo info, NetworkMessage msg)
    {
        MessageSender.Send(new MatchConnectionMessage(MasterServerConnectionManager.GetAccountCredentials().Username), info.ConnectionID, 0);
    }


    override public void Inputs()
    {
    }

    public override void OnExit()
    {
        MessageReader.RemoveHandler(3, OnMatchesListReceived);
    }

    void ConnectToMatch(string ip, int port, string pass = "")
    {
        MatchManager.ConnectToMatch(ip, port, pass);
    }

    void ConnectToMatch(object[] parameters)
    {
        if (parameters.Length > 1)
        {
            int port;
            int.TryParse((string)parameters[1], out port);
            string password = "";
            if (parameters.Length > 2)
            {
                password = (string)parameters[2];
            }
            Debugger.Log("Connexion au match à l'IP " + (string)parameters[0] + " Port : " + port);
            ConnectToMatch((string)parameters[0], port, password);
        }
        else
        {
            Debugger.Log("ERREUR - Pas assez d'arguments pour ConnectToMatch ! Il faut un IP et un Port.", Color.red);
        }
    }
}
