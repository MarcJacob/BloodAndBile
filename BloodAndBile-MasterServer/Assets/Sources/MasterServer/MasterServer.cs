using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BloodAndBileEngine.Networking;

/**
 * <summary> Classe centrale faisant tourner tous les modules d'un Master Server BloodAndBile. </summary>
 */ 
public class MasterServer : MonoBehaviour {

    bool Online = false; // Ce Master Server est-il activé ?

    IMasterServerModule[] Modules; // Modules tournants sur ce Master Server.

    BloodAndBileEngine.InputHandlersManager InputHandlers;
    BloodAndBileEngine.PlayerControlCommandManager PlayerControlManager;

    void Activate()
    {
        Online = true;
        foreach(IMasterServerModule module in Modules)
        {
            module.Activate();
        }
    }

    void Deactivate()
    {
        Online = false;
        foreach (IMasterServerModule module in Modules)
        {
            module.Deactivate();
        }
    }

    /**
     * <summary> Initialisation des modules. </summary>
     */ 
    void InitialiseModules()
    {
        Modules = new IMasterServerModule[]
        {
            new ClientsManagerModule(),
            new MatchServersManagerModule()
        };

        foreach (IMasterServerModule module in Modules)
        {
            module.Init();
        }
    }


	void Start ()
    {
        NetworkSocket.Initialise(25001, 500); // Initialise un socket au port 25000 avec 500 connexions au max.
        InitialiseModules();
        Activate();
        BloodAndBileEngine.NetworkCommandManager.Initialize();
        InputHandlers = new BloodAndBileEngine.InputHandlersManager();
        InputHandlers.Add("Activate", (object[] parameters) => Activate());
        InputHandlers.Add("Deactivate", (object[] parameters) => Deactivate());

        PlayerControlManager = new BloodAndBileEngine.PlayerControlCommandManager();
        PlayerControlManager.SetExecuteLocally();
    }
	
	void Update ()
    {
        foreach (IMasterServerModule module in Modules)
        {
            module.Update();
        }
    }
}
