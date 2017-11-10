using System;
using System.Collections.Generic;
using System.Threading;
/**
 * <summary> Un Match est un ensemble de module et une State Machine combinés pour contrôler le déroulement d'un match en ligne.</summary>
 */ 
public class Match
{
    List<int> PlayerConnectionIDs = new List<int>();

    public bool Ongoing = false; // Ce match est-il en cours ?

    List<MatchModule> Modules = new List<MatchModule>();

    public MatchModule[] GetModules()
    {
        return Modules.ToArray();
    }
    
    
    public T GetModule<T>() where T : MatchModule
    {
        foreach(MatchModule module in Modules)
        {
            if (module is T)
            {
                return (T)module;
            }
        }

        return null;
    }

    public T AddModule<T>(T module) where T : MatchModule
    {
        Modules.Add(module);
        return module;
    }

    public void Start()
    {
        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnPlayerDisconnected);


        // Envoi d'un message de commencement du match.
        BloodAndBileEngine.Networking.NetworkMessage message = new BloodAndBileEngine.Networking.NetworkMessage(20001);
        SendMessageToPlayers(message, 0);

        // Initialisation des modules
        foreach (MatchModule modules in Modules)
        {
            modules.Initialise(); // Initialisation des modules.
        }
        Ongoing = true; // On lance le match.
    }

    public void SetPlayerConnectionIDs(int[] ids)
    {
        foreach(int coID in ids)
        {
            PlayerConnectionIDs.Add(coID);
        }
    }

    public void SendMessageToPlayers(BloodAndBileEngine.Networking.NetworkMessage message, int channelID = -1)
    {
        foreach(int coID in PlayerConnectionIDs)
        {
            BloodAndBileEngine.Debugger.Log("Envoi d'un message au joueur ID " + coID);
            BloodAndBileEngine.Networking.MessageSender.Send(message, coID, channelID);
        }
    }

    float DeltaTime = 0.0f;

    public void Update()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        // Mettre à jour les modules
        foreach(MatchModule module in Modules)
        {
            module.Update(DeltaTime);
        }

        watch.Stop();
        DeltaTime = (float)watch.Elapsed.Milliseconds / 1000;
    }

    public void EndMatch()
    {
        Ongoing = false;

        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnPlayerDisconnected);

        foreach (MatchModule module in Modules)
        {
            module.Stop();
        }
    }

    public void OnPlayerDisconnected(int coID)
    {
        if(PlayerConnectionIDs.Contains(coID))
        {
            PlayerConnectionIDs.Remove(coID);
        }

        if (PlayerConnectionIDs.Count == 0)
        {
            BloodAndBileEngine.Debugger.Log("Fin du match : tous les joueurs ont été déconnectés !", UnityEngine.Color.red);
            EndMatch();
        }
    }
}