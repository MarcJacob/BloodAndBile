using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
/**
 * <summary> Un Match est un ensemble de module et une State Machine combinés pour contrôler le déroulement d'un match en ligne.</summary>
 */
public class Match
{

    /// <summary>
    /// Dictionnaire avec comme clé l'ID de connexion du client associé à l'ID de l'entité qu'il contrôle. Si cette dernière n'éxiste pas, alors cette valeur est à -1;
    /// </summary>
    List<int> PlayerConnectionIDs = new List<int>();

    public bool Ongoing = false; // Ce match est-il en cours ?
    public bool NeedsStop = false; // Ce match devrait-il être supprimé ? (Force la déconnexion des joueurs).

    List<MatchModule> Modules = new List<MatchModule>();

    public MatchModule[] GetModules()
    {
        return Modules.ToArray();
    }


    public T GetModule<T>() where T : MatchModule
    {
        foreach (MatchModule module in Modules)
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

    public int[] GetPlayerConnectionIDs()
    {
        return PlayerConnectionIDs.ToArray();
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
        foreach (int coID in ids)
        {
            PlayerConnectionIDs.Add(coID);
        }
    }

    public void SendMessageToPlayers(BloodAndBileEngine.Networking.NetworkMessage message, int channelID = -1)
    {
        foreach (int coID in PlayerConnectionIDs)
        {
            BloodAndBileEngine.Networking.MessageSender.Send(message, coID, channelID);
        }
    }

    public void SendCommandToPlayers(string commandHeader, params object[] args)
    {
        object[] newParams = new object[args.Length + 2];
        newParams[1] = commandHeader;
        for(int i = 2; i < newParams.Length; i++)
        {
            newParams[i] = args[i - 2];
        }
        foreach(int coID in PlayerConnectionIDs)
        {
            newParams[0] = coID;
            BloodAndBileEngine.InputManager.SendCommand("NetCommand", newParams);
        }
    }

    public void SendCommandToPlayer(int connectionID, string commandHeader, params object[] args)
    {
        if (PlayerConnectionIDs.Contains(connectionID))
        {
            object[] newParams = new object[args.Length + 2];
            newParams[1] = commandHeader;
            for (int i = 2; i < newParams.Length; i++)
            {
                newParams[i] = args[i - 2];
            }
            newParams[0] = connectionID;
            BloodAndBileEngine.InputManager.SendCommand("NetCommand", newParams);
        }
    }

    float endTimer = 5f;
    public void Update(float deltaTime)
    {
        if (Ongoing)
        {
            // Mettre à jour les modules
            foreach (MatchModule module in Modules)
            {
                module.Update(deltaTime);
            }
        }
        else
        {
            endTimer -= deltaTime;
            if (endTimer <= 0f)
            {
                NeedsStop = true;
            }
        }
    }

    public void EndMatch()
    {
        Ongoing = false;
        foreach (MatchModule module in Modules)
        {
            module.Stop();
        }
    }

    public void OnPlayerDisconnected(int coID)
    {
        BloodAndBileEngine.Debugger.Log("Joueur déconnecté : " + coID);
        if(PlayerConnectionIDs.Contains(coID))
        {
            if (Ongoing)
            {
                foreach (MatchModule mod in Modules)
                {
                    mod.OnPlayerDisconnected(coID);
                }
            }
            PlayerConnectionIDs.Remove(coID);
        }
    }

    ~Match()
    {
        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnPlayerDisconnected);
    }
}