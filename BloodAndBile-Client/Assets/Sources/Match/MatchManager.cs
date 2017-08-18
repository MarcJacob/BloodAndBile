using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Contient des informations relatives au Match en cours sur ce client, comme le nom, le nombre et le nom
 * des autres joueurs, et si le match est hébergé par ce client. </summary>
 */ 
public static class MatchManager
{
    static public bool Hosting = false;
    /**
     * <summary> Crée un nouveau Match. </summary>
     */ 
    static public void HostMatch(string matchName, string password)
    {
        Hosting = true;

        // Envoie d'un signal de création de match au Master Server.
        MessageSender.Send(new MatchCreationMessage(matchName, password), MasterServerConnectionManager.GetMasterServerConnectionID(), 0);

    }

    static public void UpdateMatch()
    {
        if (!Hosting)
        {
            Debugger.Log("Le match n'est pas host sur cette machine !");
            return;
        }
    }


}
