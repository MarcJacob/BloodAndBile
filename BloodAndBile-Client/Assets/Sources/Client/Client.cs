using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BloodAndBileEngine.Networking;

/**
 * <summary> Gestion des états du client et de la transition entre eux. </summary>
 * 
 * SINGLETON
 */ 
public class Client : MonoBehaviour {

    // STATIC
    static Client Instance;
    static IClientState CurrentState;
    static public IClientState GetCurrentState()
    {
        return CurrentState;
    }
    static public void ChangeState(IClientState state)
    {
        if (CurrentState != null)
        CurrentState.OnExit();

        CurrentState = state;
        CurrentState.OnEntry();
    }
    //

	void Start () {
	    if (Instance == null)
        {
            Instance = this;
            NetworkSocket.Initialise(24999); // Initialise un Socket pour cette application sur le port 24999.
            BloodAndBileEngine.EntitiesManager.Initialise();
        }	
        else
        {
            Destroy(gameObject);
        }
	}

	void Update ()
    {
		if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
        else
        {
            ChangeState(new LoginState());
        }
	}

    // INFORMATIONS CLIENT (Statiques)

    static public string Username;
}
