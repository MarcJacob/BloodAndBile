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
        State.Exit();
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
        State = new MainMenuState();
        State.Init();
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
}