using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary> Gère la connexion au Master Server. Permet d'obtenir un match que le Client se mettra à "écouter" à travers le
 * Network Interface. NOTE : Lorsqu'un match est en cours, la connection au Master Server est facultative mais à défaut de celle ci,
 * cet objet s'occupera d'afficher un bout d'interface pour prévenir le joueur, puis essayera de s'y reconnecter régulièrement. </summary>
 * 
 * SINGLETON
 */ 
public class MasterServerConnectionManager : MonoBehaviour
{
    // SINGLETON

    static MasterServerConnectionManager Instance;
    static public bool IsConnected()
    {
        return Instance != null && Instance.Connected;
    }

    static public void SetCredentials(string Username, string Password)
    {
        Instance.Account.Username = Username;
        Instance.Account.Password = Password;
    }

    static public void SetCredentials(object[] parameters)
    {
        if (parameters.Length < 1)
        {
            Debugger.Log("ERREUR - Nombre d'arguments insuffisant pour SetCredentials() Fichier : MasterServerConnectionManager.cs", Color.red);
        }
        else
        {
            SetCredentials((string)parameters[0], (string)parameters[1]);
        }
    }

    static public AccountCredentials GetAccountCredentials()
    {
        return Instance.Account;
    }

    static public int GetMasterServerConnectionID()
    {
        return Instance.MasterServerConnectionID;
    }

    //_____________________________________________

    public string MasterServerIP; // - IP du Master Server. 
    public int MasterServerPort; // - Port du Master Server. // TODO : Charger l'IP et le Port du Master Server depuis un fichier de config.

    AccountCredentials Account = new AccountCredentials("", "");

    bool CredentialsReady = false; // Les informations de compte ont-elles été tapés par l'utilisateur ?
    bool Connected = false; // Sommes-nous connectés au Master Server ?
    bool Authentified = false; // Sommes-nous authentifiés sur le Master Server ?
    int MasterServerConnectionID = -1; // -1 = déconnecté.
    float ConnectionAttemptFrequency = 0.1f; // Combien de fois par seconde essayons nous de nous connecter au Master Server ?
    float CurrentConnectionAttemptCooldown = 0f; // Cooldown

    /**
     * <summary> Tente une connexion au Master Server à l'IP MasterServerIP et au Port MasterServerPort. </summary>
     */ 
    void AttemptConnection(object[] parameters)
    {
        CurrentConnectionAttemptCooldown = 1f / ConnectionAttemptFrequency;
        if (parameters != null)
        {
            Account.Username = (string)parameters[0];
            Account.Password = (string)parameters[1];
        }
        if (Account.IsValid())
        {
            NetworkSocket.ConnectTo(MasterServerIP, MasterServerPort);
            Debugger.Log("Tentative de connexion au Master Server. IP : " + MasterServerIP + " Port : " + MasterServerPort);
            CredentialsReady = true;
        }
        else
            Debugger.Log("Informations de compte invalides !");
    }

    void AttemptConnection()
    {
        AttemptConnection(null);
    }

    /**
    * <summary> Executé lorsque la connexion au Master Server n'est pas établie mais que le client souhaite être connecté (infos de compte
    * tapés). </summary>
    */
    void StateDisconnected()
    {
        CurrentConnectionAttemptCooldown -= Time.deltaTime;
        if (CurrentConnectionAttemptCooldown <= 0)
        {
            Debugger.Log("Tentative de connexion au Master Server...");
            AttemptConnection();
        }
    }

    /**
     * <summary> Executé lorsque la connexion au Master Server est perdue.
     * Note : la connexion au Master Server n'est JAMAIS sensé être perdue volontairement. </summary>
     */
    void OnMasterServerDisconnected()
    {
        Debugger.Log("Déconnecté du Master Server !");
        Connected = false;
        Authentified = false;
    }

    /**
     * <summary> Exécutée à chaque image lorsque l'on est connecté au Master Server. </summary>
     */
    void StateConnected()
    {

    }

    /**
     * <summary> Exécutée à la réception d'une demande d'authentification du Master Server. </summary>
     */ 
    void OnMasterServerConnected(NetworkMessageInfo info, NetworkMessage message)
    {
        Connected = true;
        MasterServerConnectionID = info.ConnectionID;
        SendMasterServerAuthentificationRequest();
    }

    private void Update()
    {
        if (Connected)
        {
            StateConnected();
        }
        else if (CredentialsReady)
        {
            StateDisconnected();
        }
    }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            HandlersSetup();
        }
    }

    private void HandlersSetup()
    {
        MessageReader.AddHandler(40000, OnMasterServerConnected);
        MessageReader.AddHandler(40001, OnMasterServerAuthentificationResponse);

        InputManager.AddHandler("ConnectToMasterServer", AttemptConnection);
        InputManager.AddHandler("SetAccountCredentials", SetCredentials);
    }

    /**
     * <summary> Exécutée quand le Master Server envoi une demande d'authentification au Client. </summary>
     */ 
    void SendMasterServerAuthentificationRequest()
    {
        Debugger.Log("Envoi d'une demande d'authentification au Master Server.");
        // Vérifier que les informations sont valides.
        if (Connected)
        {
            // Envoyer les informations de compte en réponse au Master Server.
            MessageSender.Send(new AuthentificationMessage(Account.Username, Account.Password), MasterServerConnectionID, 0);
        }
        else
        {
            Debugger.Log("Impossible d'envoyer une demande d'authentification sans être connecté !");
        }

    }

    void OnMasterServerAuthentificationResponse(NetworkMessageInfo info, NetworkMessage message)
    {
        AuthentificationResponseMessage msg = (AuthentificationResponseMessage)message;

        if (msg.Accepted == false)
        {
            Debugger.Log("Authentification refusée : " + msg.Reason);
        }
        else
        {
            Authentified = true;
            Debugger.Log("Authentification réussie !");
        }
    }
}