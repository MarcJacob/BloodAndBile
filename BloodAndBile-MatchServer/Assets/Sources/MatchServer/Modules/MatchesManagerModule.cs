﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MatchesManagerModule : IMatchServerModule
{
    PlayersManagerModule PlayersManager;
    MatchUpdater[] Updaters;
    int PlayersPerMatch = 1;

    public MatchesManagerModule(PlayersManagerModule players, int maxUpdaters = 20)
    {
        PlayersManager = players;
        Updaters = new MatchUpdater[maxUpdaters];
    }

    public void Update()
    {
        // Vérifier le nombre de joueurs en queue. S'il est égal ou supérieur à "PlayersPerMatch", créer un match et l'ajouter à un
        // Updater.

        int[] matchMakingQueue = PlayersManager.GetMatchmakingQueue();

        if (matchMakingQueue.Length >= PlayersPerMatch)
        {
            List<int> players = new List<int>();
            for(int i = 0; i < PlayersPerMatch; i++)
            {
                players.Add(matchMakingQueue[i]);
                PlayersManager.RemovePlayerFromMatchmakingQueue(matchMakingQueue[i]);
            }
            Match newMatch = MatchCreator.CreateMatch(players.ToArray());
            int matchesPerUpdater = 0;
            int currentUpdaterID = 0;
            while (matchesPerUpdater >= 0) // Cherche le meilleur "Updater" de façon à ce que la charge des matchs soit bien
            {                               // Répartie entre les Threads.
                if (currentUpdaterID < Updaters.Length)
                {
                    if (Updaters[currentUpdaterID].GetNumberOfMatches() <= matchesPerUpdater)
                    {
                        Updaters[currentUpdaterID].AddMatch(newMatch);
                        matchesPerUpdater = -1;
                    }
                    else
                    {
                        currentUpdaterID++;
                    }
                }
                else
                {
                    currentUpdaterID = 0;
                    matchesPerUpdater++;
                }
            }
        }
    }

    public void Activate()
    {

    }

    public void Deactivate()
    {

    }

    public void Initialise()
    {

    }
}