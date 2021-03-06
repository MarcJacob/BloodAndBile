﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>Gestion des modules de Match Server.
 * 
 * Un Match Server recoit des joueurs et les associes par pairs selon certains critères, puis lance un match, en le notifiant à ces joueurs.
 * Lorsqu'un match est lancé, il est mit à jour à chaque image, et par intervalles réguliers des messages réseaux "State update" sont envoyés. </summary>
 */ 
public class MatchServer : MonoBehaviour {

    BloodAndBileEngine.InputHandlersManager InputHandlers;
    BloodAndBileEngine.Networking.HandlersManager NetworkHandlers;
    BloodAndBileEngine.PlayerControlCommandManager PlayerControlManager;
    IMatchServerModule[] Modules;

	void Start ()
    {
        BloodAndBileEngine.Networking.NetworkSocket.Initialise(25000, 500);
        BloodAndBileEngine.NetworkCommandManager.Initialize();
        MasterServerConnectionModule masterServerConnection = new MasterServerConnectionModule();
        PlayersManagerModule playersManager = new PlayersManagerModule();
        MatchesManagerModule matchesModule = new MatchesManagerModule(playersManager);
        PlayerControlManager = new BloodAndBileEngine.PlayerControlCommandManager();
        PlayerControlManager.SetExecuteLocally();

        Modules = new IMatchServerModule[] // Initialisation des modules du Match Server.
        {
            masterServerConnection,
            playersManager,
            matchesModule
        };

        foreach(IMatchServerModule mod in Modules)
        {
            mod.Initialise();
        }
         //Initialisation de la mémoire des entités TODO : déplacer ça dans un fichier plus pertinent.
        BloodAndBileEngine.EntitiesManager.Initialise();
        Activate();
	}

    void Activate()
    {
        foreach (IMatchServerModule mod in Modules)
        {
            mod.Activate();
        }
    }

    void Deactivate()
    {
        foreach (IMatchServerModule mod in Modules)
        {
            mod.Deactivate();
        }
    }
	
	void Update ()
    {
        foreach (IMatchServerModule mod in Modules)
        {
            mod.Update();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Killing all threads !");
        foreach(IMatchServerModule mod in Modules)
        {
            mod.Stop();
        }
    }
}
