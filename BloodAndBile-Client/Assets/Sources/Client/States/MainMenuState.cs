using System;
using System.Collections.Generic;
using BloodAndBileEngine;


/**
 * <summary> Etat du client dans le menu principal. Cherche à maintenir une connexion vers un Master Server.
 * Quand la connexion est établie, il peut demander au Master Server de le pointer vers le meilleur Match Server. 
 * En attendant, le MainMenuState peut maintenir toute sorte d'informations sur le joueur connecté se trouvant sur le Master Server. </summary>
 */ 
public class MainMenuState : IClientState
{
    BloodAndBileEngine.Networking.HandlersManager NetworkHandlers;
    BloodAndBileEngine.InputHandlersManager InputHandlers;

    public void OnEntry()
    {
        NetworkHandlers = new BloodAndBileEngine.Networking.HandlersManager();
        InputHandlers = new InputHandlersManager();
        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnDisconnected);
        InputHandlers.Add("StartMatchmaking", StartMatchmaking);
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        BloodAndBileEngine.Networking.NetworkSocket.UnregisterOnDisconnectionCallback(OnDisconnected);
        NetworkHandlers.Clear();
    }

    void OnDisconnected(int coID)
    {
        // On vérifie si la déconnexion concerne le Master Server (probablement oui).
        int masterServerConnectionID = ClientConnectionsManager.GetConnectionIDFromName("MasterServer");
        if ( masterServerConnectionID == -1 || masterServerConnectionID == coID) // Il faut vérifier s'il est égal à -1 car la méthode dans le ClientConnectionsManager qui pourrait retirer l'ID "MasterServer" a peut être déjà été appelée.
        {
            Debugger.Log("Déconnecté du Master Server ! Retour au menu Login !", UnityEngine.Color.red);
            Client.ChangeState(new LoginState());
        }
    }

    void StartMatchmaking(object[] parameters)
    {
        Client.ChangeState(new MatchServerSearchState());
    }
}

