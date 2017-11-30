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
    bool Activated = true;
    public MatchUpdater()
    {
        // Création du Thread de mise à jour du match.
        UpdateThread = new Thread(() => { while (Activated) {
                lock (Matches)
                {
                    if (Matches.Count > 0)
                        UpdateMatches();
                }

            } });
        if (UpdateThread == null) { BloodAndBileEngine.Debugger.Log("Création du Thread MatchUpdater infructueuse !");
            return; }
        BloodAndBileEngine.Debugger.Log("Création MatchUpdater...");
        UpdateThread.Start();
    }

    public void AddMatch(Match m)
    {
        lock (Matches)
        {
            if (!Matches.Contains(m))
            {
                BloodAndBileEngine.Debugger.Log("Ajout d'un match au MatchUpdater !");
                Matches.Add(m);
                m.Start();
            }
        }
    }

    void UpdateMatches()
    {
            List<Match> endedMatches = new List<Match>();
            foreach (Match match in Matches)
            {
                if (match.Ongoing)
                {
                    match.Update();
                    if (match.Ongoing == false)
                    {
                        endedMatches.Add(match);
                    }
                }
            }

            foreach (Match match in endedMatches)
            {
                Matches.Remove(match);
            }
    }

    public void Stop()
    {
        Activated = false;
        UnityEngine.Debug.Log("Ending thread");
        UpdateThread.Abort();
        foreach(Match m in Matches)
        {
            m.EndMatch();
        }
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
