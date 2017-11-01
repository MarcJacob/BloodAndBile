using System;
using System.Collections.Generic;
using System.Threading;

/**
 * <summary> Un MatchUpdater met constamment à jour 0 à plusieurs matchs sur un Thread parallèle.
 * Les Match eux mêmes sont crées par le MatchModule, puis transférés à un MatchUpdater. </summary>
 */ 
public class MatchUpdater
{
    Thread UpdateThread;
    List<Match> Matches = new List<Match>(); // Ensemble de matchs à mettre à jour.

    public MatchUpdater()
    {
        // Création du Thread de mise à jour du match.
        UpdateThread = new Thread(() => { while (true) { UpdateMatches(); } });
        BloodAndBileEngine.Debugger.Log("Création MatchUpdater...");
        UpdateThread.Start();
    }

    public void AddMatch(Match m)
    {
        if(!Matches.Contains(m))
        {
            BloodAndBileEngine.Debugger.Log("Ajout d'un match au MatchUpdater !");
            Matches.Add(m);
            m.Start();
        }
    }

    void UpdateMatches()
    {
        List<Match> endedMatches = new List<Match>();
        foreach(Match match in Matches)
        {
            match.Update();
            if (match.Ongoing == false)
            {
                endedMatches.Add(match);
            }
        }

        foreach(Match match in endedMatches)
        {
            Matches.Remove(match);
        }
    }

    public void Stop()
    {
        UpdateThread.Abort();
        Matches.Clear();
    }

    ~MatchUpdater()
    {
        Stop();
    }

    public int GetNumberOfMatches()
    {
        return Matches.Count;
    }
}
