using System;
using System.Collections.Generic;


/**
 * <summary> Gestion des matchs. Classe outil possèdant la liste de tous les matchs en attente et en cours. 
 * Régulièrement, tous les matchs sont mis à jour : on vérifie si l'hôte est toujours connecté, notamment.</summary>
 */ 
public class MatchesManager
{
    // SINGLETON
    static MatchesManager Instance;

    //______________________________________


    Dictionary<string, Match> Matches = new Dictionary<string, Match>();

    /**
     * <summary> En vérifiant si le client est bien considéré comme connecté, cette fonction crée un nouveau match selon les informations
     * qu'on lui donne. </summary>
     */ 
    public void NewMatch(NetworkMessageInfo info, NetworkMessage message)
    {
        MatchCreationMessage msg = (MatchCreationMessage)message;
        Client c = ClientsManager.GetClientFromConnectionID(info.ConnectionID);
        ConnectionInfo coInfo = NetworkSocket.GetConnectionInfoFromConnectionID(info.ConnectionID);
        if (c != null && c.Connected())
        {
            Debugger.Log("Création d'un nouveau match : " + '"' + msg.MatchName + '"' + ". Hôte : " + c.GetAccountName() + ' ' + coInfo.IP + ' ' + coInfo.Port);
            Matches.Add(msg.MatchName, new Match(c, coInfo.IP, coInfo.Port, msg.MatchName, msg.Password != "")); // Création du nouveau match.
        }
    }

    /**
     * <summary> A chaque Update, on met à jour les matchs. Si un match est dans l'état "Ended", 
     * on le supprime du dictionnaire des matchs.</summary>
     */ 
    public void UpdateMatches()
    {
        List<string> cleanup = new List<string>();
        foreach(Match m in Matches.Values)
        {
            m.Update();
            if (m.IsOver())
            {
                cleanup.Add(m.Info.MatchName);
            }
        }

        foreach(string name in cleanup)
        {
            Matches.Remove(name); // Supression du match.
            Debugger.Log("Supression du match " + name + ".");
        }
    }

    void SendMatchList(NetworkMessageInfo info, NetworkMessage msg)
    {
        List<MatchInfo> OpenMatches = new List<MatchInfo>();
        foreach(Match match in Matches.Values)
        {
            if (match.MatchState == MATCH_STATE.OPEN_PUBLIC)
            {
                OpenMatches.Add(match.Info);
            }
        }

        MessageSender.Send(new MatchListMessage(OpenMatches.ToArray()), info.ConnectionID, 0);
    }

    public void Init()
    {
        // Setup des handlers

        MessageReader.AddHandler(1, NewMatch);
        MessageReader.AddHandler(3, SendMatchList);
    }
    
}