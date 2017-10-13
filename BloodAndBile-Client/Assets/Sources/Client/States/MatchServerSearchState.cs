using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Etat du client durant lequel il cherche le meilleur Match Server possible donné par le Master Server. 
 * Une fois le meilleur match server trouvé, on passe à l'état "Matchmaking" qui s'occupera de la connexion à ce match server.
 * Si aucun match server n'a pu être trouvé, alors on retourne au MainMenuState. </summary>
 */ 
public class MatchServerSearchState : IClientState
{
    string[] MatchServerIPs;

    bool IPsReceived = false;

    UnityEngine.Ping[] MatchServerPings;

    BloodAndBileEngine.Networking.HandlersManager NetworkHandlers;

    public void OnEntry()
    {
        NetworkHandlers = new BloodAndBileEngine.Networking.HandlersManager();

        NetworkHandlers.Add<BloodAndBileEngine.Networking.NetworkMessages.IPListMessage>(40000, OnMatchServersListReceived);

        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnConnectionLost);

        int masterServerCoID = ClientConnectionsManager.GetConnectionIDFromName("MasterServer");
        if (masterServerCoID > 0)
        {
            BloodAndBileEngine.Debugger.Log("Début de recherche d'un MatchServer...");
            SendMatchServersListRequest(masterServerCoID);
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("Pas de connexion au Master Server... retour à l'écran Login.", UnityEngine.Color.red);
            Client.ChangeState(new LoginState());
        }
    }

    public void OnUpdate()
    {
        if (IPsReceived) LookForBestMatchServer();
    }

    public void OnExit()
    {
        NetworkHandlers.Clear();
        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnConnectionLost);
    }

    /**
     * <summary> Exécutée à chaque image lorsque l'on cherche le meilleur Match Server. </summary>
     */
    float SearchTime = 1f;
    float CurrentSearchTime = 0f;
    void LookForBestMatchServer()
    {
        CurrentSearchTime += UnityEngine.Time.deltaTime;
        if (CurrentSearchTime >= SearchTime)
        {
            UnityEngine.Ping bestPing = null;
            foreach(UnityEngine.Ping ping in MatchServerPings)
            {
                if (ping.isDone && (bestPing == null || ping.time > bestPing.time))
                {
                    bestPing = ping;
                }
            }

            if (bestPing != null)
            {
                BloodAndBileEngine.Debugger.Log("Match server trouvé ! Connexion à l'IP : " + bestPing.ip + " ping : " + bestPing.time);
                Client.ChangeState(new MatchmakingState(bestPing.ip));
            }

            CurrentSearchTime = 0f;
        }
    }

    // HANDLERS

    /**
    * <summary> Quand on reçoit la liste des IPs des Match Server. </summary>
    */ 
    void OnMatchServersListReceived(BloodAndBileEngine.Networking.NetworkMessageInfo info, BloodAndBileEngine.Networking.NetworkMessages.IPListMessage message)
    {
        BloodAndBileEngine.Debugger.Log("Liste des Match Servers reçue !");
        MatchServerIPs = message.IPList;

        if (MatchServerIPs.Length > 0)
        {
            foreach (string IP in MatchServerIPs)
            {
                BloodAndBileEngine.Debugger.Log("--- " + IP);
            }

            IPsReceived = true;
            MatchServerPings = new UnityEngine.Ping[MatchServerIPs.Length];
            for(int i = 0; i < MatchServerPings.Length; i++)
            {
                MatchServerPings[i] = new UnityEngine.Ping(MatchServerIPs[i]);
            }
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("Pas de Match Servers !", UnityEngine.Color.red);
            // Retour au MainMenuState
            Client.ChangeState(new MainMenuState());
        }
    }

    //___________________________________

    /**
    * <summary> Envoi d'une demande de transmission de la liste des IPs des Match Servers connectés au Master Server. </summary>
    */ 
    void SendMatchServersListRequest(int masterServerCoID)
    {
        BloodAndBileEngine.Debugger.Log("Envoi d'une demande de liste des matchs servers au Master Server.");
        BloodAndBileEngine.Networking.MessageSender.Send(new BloodAndBileEngine.Networking.NetworkMessage(0), masterServerCoID, 0);
    }

    void OnConnectionLost(int coID)
    {
        // Si la connexion vers le Master Server a été perdue avant que l'on recoive les IPs, alors on revient au menu principal.
        if (coID == ClientConnectionsManager.GetConnectionIDFromName("MasterServer") || ClientConnectionsManager.GetConnectionIDFromName("MasterServer") == -1)
        {
            if (IPsReceived == false)
            {
                BloodAndBileEngine.Debugger.Log("ERREUR - Connexion au Master Server perdue avant de recevoir la liste des IPs... retour au menu principal.", UnityEngine.Color.red);
                Client.ChangeState(new MainMenuState());
            }
        }
    }

}
