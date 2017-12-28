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
    List<int> PlayersToDisconnect = new List<int>();
    bool Activated = true;
    float DeltaTime = 0f;



    public MatchUpdater()
    {
        // Création du Thread de mise à jour du match.
        UpdateThread = new Thread(() => { while (Activated) {
                System.Diagnostics.Stopwatch Watch = System.Diagnostics.Stopwatch.StartNew();
                Watch.Start();
                lock (Matches)
                {
                    if (Matches.Count > 0)
                    {

                        UpdateMatches(DeltaTime);

                    }
                }
                Thread.Sleep(10); // Temps minimal entre chaque mise à jour : 10ms.
                Watch.Stop();
                DeltaTime = (float)Watch.ElapsedTicks / (float)System.Diagnostics.Stopwatch.Frequency;


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

    void UpdateMatches(float deltaTime)
    {
            List<Match> endedMatches = new List<Match>();
            foreach (Match match in Matches)
            {
                if (!match.NeedsStop)
                {
                    match.Update(deltaTime);
                    if (match.NeedsStop == true)
                    {
                        endedMatches.Add(match);
                    }
                }
            }

            foreach (Match match in endedMatches)
            {
                Matches.Remove(match);
                // Déconnexion des joueurs concernés.
                foreach (int playerID in match.GetPlayerConnectionIDs())
                {
                    PlayersToDisconnect.Add(playerID);
                }
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

    public void UpdateDisconnects()
    {
        foreach(int playerID in PlayersToDisconnect)
            BloodAndBileEngine.Networking.NetworkSocket.Disconnect(playerID);

        PlayersToDisconnect.Clear();
    }
}
