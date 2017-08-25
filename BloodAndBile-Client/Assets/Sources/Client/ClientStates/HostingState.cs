using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * <summary> Etat dans lequel le client se trouve lorsqu'il est hébergeur du match en cours. 
 * Envoi les messages aux clients connectés et reçoit leurs inputs. </summary>
 */ 
public class HostingState : ClientState
{
    MatchData HostedMatch;
    Dictionary<int, string> ConnectedPlayers = new Dictionary<int, string>();

    public HostingState(MatchData match)
    {
        HostedMatch = match;
    }

    public void SetHostedMatch(MatchData data)
    {
        HostedMatch = data;
    }

    public override void Init()
    {
        HandlersSetup();
    }

    public override void Inputs()
    {
        
    }

    public override void Update()
    {
        HostedMatch.Update();

        if (HostedMatch.GetState() == MATCH_STATE.ENDED)
        {
            End();
        }
    }

    public override void OnExit()
    {
        HostedMatch.OnMatchEnd(); // Mettre fin au match.
        MessageReader.RemoveHandler(2, OnClientIdentified);
        NetworkSocket.UnregisterOnConnectionEstablishedCallback(OnClientConnected);
    }

    public void End()
    {
        Client.ChangeState(new MainMenuState());
    }

    // HANDLERS D'HOTE

    void HandlersSetup()
    {
        NetworkSocket.RegisterOnConnectionEstablishedCallback(OnClientConnected);
        MessageReader.AddHandler(2, OnClientIdentified);
    }

    /**
     * <summary> Quand une machine susceptible d'être un client est connectée. </summary>
     */ 
    void OnClientConnected(int coID)
    {
        SendIdentificationRequest(coID);
    }

    /**
     * <summary> Quand un client s'identifie au match. </summary>
     */
    void OnClientIdentified(NetworkMessageInfo info, NetworkMessage msg)
    {
        MatchConnectionMessage message = (MatchConnectionMessage)msg;
        string PlayerName = message.Username;
        // Si le match est fermé, on vérifie que les mots de passe correspondent.
        if (HostedMatch.GetMatchInfo().Locked && message.Password != HostedMatch.Password)
        {
            // Les mots de passe ne correspondent pas ! Envoi d'un message de refus d'identification.
            SendIdentificationResponse(info.ConnectionID, false, "Wrong password !");
        }
        else
        {
            SendIdentificationResponse(info.ConnectionID, true);
            ConnectedPlayers.Add(info.ConnectionID, PlayerName);
            Debugger.Log("Joueur connecté : " + PlayerName, Color.cyan);
        }
    }

    //________________________________________________________________________________________________________________________________________

    // FONCTIONS D'ENVOI DE MESSAGES VERS LES JOUEURS

    void SendMessageToAllPlayers(NetworkMessage message, int channelID = -1)
    {
        foreach(int coID in ConnectedPlayers.Keys)
        {
            MessageSender.Send(message, coID, channelID);
        }
    }

    void SendIdentificationRequest(int coID)
    {
        MessageSender.Send(new NetworkMessage(20000), coID, 0); // Envoi d'un message de type 20000
    }

    void SendIdentificationResponse(int coID, bool accepted, string reason = "")
    {
        MessageSender.Send(new MatchConnectionResponseMessage(accepted, reason), coID, 0);
    }

    void SendMatchInfoUpdate()
    {
        List<string> lobby = new List<string>();
        foreach(string name in ConnectedPlayers.Values)
        {
            lobby.Add(name);
        }
        lobby.Add(MasterServerConnectionManager.GetAccountCredentials().Username);
        SendMessageToAllPlayers(new MatchInfoMessage(HostedMatch.GetMatchInfo(), lobby.ToArray()));
    }
}
