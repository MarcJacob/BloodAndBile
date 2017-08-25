using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Gestion de la connexion au match en cours, et de l'hébergement d'un nouveau match. </summary>
 */ 
public static class MatchManager
{
    static public bool Hosting = false;
    static MatchData Match;// Données du match. 

    /**
     * <summary> Crée un nouveau Match. Renvoi une référence vers l'objet MatchData. </summary>
     */
    static public MatchData HostMatch(string matchName, string password, string ip)
    {
        Hosting = true;
        Match = new MatchData();
        Match.UpdateMatchInfo(new MatchInfo(matchName, MasterServerConnectionManager.GetAccountCredentials().Username, "", 0, password != ""));
        Match.Password = password;
        // Envoie d'un signal de création de match au Master Server.
        MessageSender.Send(new MatchCreationMessage(matchName, password), MasterServerConnectionManager.GetMasterServerConnectionID(), 0);
        return Match;
    }

    /**
     * <summary> Wrapper de HostMatch "décripteur" de commande du InputManager.
     * Pour être sur que le match soit mit à jour, il faut forcer le client à passer en HostingState. </summary>
     */ 
    static void HostMatch(object[] parameters)
    {
        string matchName ="";
        string ip = "";
        string password = "";
        if (parameters.Length < 1)
        {
            Debugger.Log("ERREUR - Un match nécessite au moins un nom !");
            return;
        }

        matchName = (string)parameters[0];
        
        if (parameters.Length > 1)
        {
            password = (string)parameters[1];
        }
        else
        {
            password = "";
        }

        if (parameters.Length > 2)
        {
            ip = (string)parameters[2];
        }
        else
        {
            ip = "127.0.0.1";
        }

        Client.ChangeState(new HostingState(HostMatch(matchName, password, ip)));
    }

    /**
     * <summary> Connexion à un match. Il faut donner l'IP, le port, et le mot de passe du match (optionnel). </summary>
     */ 
    static public void ConnectToMatch(string ip, int port, string password = "")
    {
        Match = new MatchData();
        NetworkSocket.ConnectTo(ip, port); // Connexion au serveur de match.
    }

    public static void HandlersSetup()
    {
        InputManager.AddHandler("CreateMatch", HostMatch);
    }


}
