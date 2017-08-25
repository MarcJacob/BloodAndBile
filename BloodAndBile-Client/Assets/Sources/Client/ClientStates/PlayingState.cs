using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Un match est en cours.</summary>
 */ 
public class PlayingState : ClientState
{
    // Propriétés.

    MatchData Match; // Match en cours
    string[] Lobby;
    int HostConnectionID;

    /**
        * <summary> Tentative de connexion au Match à l'IP spécifiée dans le constructeur et setup des handlers. </summary>
        */ 
    public override void Init()
    {
        MessageReader.AddHandler(20002, OnMatchInfoReceived);
    }

    public override void Inputs()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void OnExit()
    {
        
    }

    void OnMatchInfoReceived(NetworkMessageInfo info, NetworkMessage msg)
    {
        HostConnectionID = info.ConnectionID;
        MatchInfoMessage message = (MatchInfoMessage)msg;
        Match.UpdateMatchInfo(message.Info);
        Lobby = message.Lobby;
    }
}
