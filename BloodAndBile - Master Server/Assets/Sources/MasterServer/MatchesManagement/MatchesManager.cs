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
        string HostIP = NetworkSocket.GetIPFromConnectionID(info.ConnectionID);
        if (c != null && c.Connected())
        {
            Debugger.Log("Création d'un nouveau match : " + '"' + msg.MatchName + '"' + ". Hôte : " + c.GetAccountName() + ' ' + HostIP);
            Matches.Add(msg.MatchName, new Match(c,HostIP, msg.MatchName, msg.Password)); // Création du nouveau match.
            MessageSender.Send(new MatchCreationResponseMessage(c.GetAccountName(), msg.Password, msg.MatchName, HostIP), info.ConnectionID);
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
                cleanup.Add(m.GetName());
            }
        }

        foreach(string name in cleanup)
        {
            Matches.Remove(name); // Supression du match.
            Debugger.Log("Supression du match " + name + ".");
        }
    }

    public void Init()
    {
        // Setup des handlers

        MessageReader.AddHandler(1, NewMatch);
    }
    
}