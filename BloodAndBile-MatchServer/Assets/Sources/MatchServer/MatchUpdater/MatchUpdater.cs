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
    List<Match> Matches; // Ensemble de matchs à mettre à jour.

    public MatchUpdater()
    {
        UpdateThread = new Thread(() => { while (true) { UpdateMatches(); } });
        UpdateThread.Start();
    }

    public void AddMatch(Match m)
    {
        if(!Matches.Contains(m))
        {
            Matches.Add(m);
            m.Start();
        }
    }

    void UpdateMatches()
    {
        foreach(Match match in Matches)
        {
            match.Update();
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
