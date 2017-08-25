using System;
using System.Collections.Generic;


/**
 * <summary> Message contenant des informations sur un match. Envoyé du serveur du match aux clients connectés régulièrement. </summary>
 */ 
[Serializable]
public class MatchInfoMessage : NetworkMessage
{
    public MatchInfo Info;
    public string[] Lobby; // Tableau des noms des joueurs connectés.

    public MatchInfoMessage(MatchInfo info, string[] lobby) : base(20002)
    {
        Info = info;
        Lobby = lobby;
    }
}
