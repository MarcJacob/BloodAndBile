using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary> Gère l'état actuel du client (Menu principal, en recherche, en connexion, en cours de match, post-match). 
 * Contient les informations de Match comme les joueurs présents. </summary>
 */
public class Client : MonoBehaviour
{
    // Gestion de l'état du client && Singleton.
    static ClientState State; // Etat actuel du client.

    static public void ChangeState(ClientState newState)
    {
        State.OnExit();
        State = newState;
        State.Init();
    }

    static Client Instance;
    //_______________________________________________    

    void Start()
    {
        // Si l'Instance n'a pas encore été "revendiquée", la prendre. Sinon, détruire cet objet.
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /**
     * <summary> Exécutée quand cette instance de Client devient le Singleton. </summary>
     */
    private void Init()
    {
        GameObject.DontDestroyOnLoad(gameObject); // Fait que cet objet ne sera pas détruit lors d'un changement de scène.
        NetworkSocket.Initialise();
        State = new MainMenuState(); // Premier état : MainMenuState
        State.Init();

        // Setup des handlers liés au MatchManager.
        MatchManager.HandlersSetup();

        // Setup des handlers liés au Client (messages de type 2xxxx ou 4xxxxx)
        MessageReader.AddHandler(20000, OnConnectedToMatch);

        //Setup des Inputs.
        InputManager.AddHandler("Exit", Quit);
    }


    /**
     * <summary> Réponse à une demande d'identification du match. </summary>
     */ 
    public static void OnConnectedToMatch(NetworkMessageInfo info, NetworkMessage message)
    {
        MessageSender.Send(new MatchConnectionMessage(MasterServerConnectionManager.GetAccountCredentials().Username), info.ConnectionID, 0);
    }


    private void Update()
    {
        // Exécution de l'état actuel.
        if (State != null)
        {
            State.Inputs();
            State.Update();
        }
    }

    void Quit(object[] parameters)
    {
        Debugger.Log("Fermeture du jeu...");
        Application.Quit();
    }
}