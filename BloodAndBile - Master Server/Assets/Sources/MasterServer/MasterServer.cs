using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * <summary> Le MasterServer est un serveur central auquel se connectent les clients pour rendre leur match visible et pour rejoindre
 * le match d'un autre client. Il est donc un "Hub", une plate-forme centrale, où TOUS les joueurs en ligne sont en intéraction. </summary>
 */ 
public class MasterServer : MonoBehaviour
{
    // STATIC (Singleton)
    static MasterServer Instance;

    ClientsManager ClientsModule;
    MatchesManager MatchesModule;

    private void Init()
    {
        DontDestroyOnLoad(gameObject);
        NetworkSocket.Initialise();
        ClientsModule = new ClientsManager();
        MatchesModule = new MatchesManager();

    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Init();
        }
    }
	
	void Update ()
    {
        MatchesModule.UpdateMatches();
	}

    private void Start()
    {
        MatchesModule.Init();
        SetupHandlers();
    }

    /**
     * <summary> L'INTEGRALITE des handlers du Master Server doivent être déclarés ici. </summary>
     */ 
    void SetupHandlers()
    {
        NetworkSocket.RegisterOnConnectionEstablishedCallback(OnClientConnected);
        NetworkSocket.RegisterOnDisconnectionCallback(ClientsModule.LogOff);
        MessageReader.AddHandler(0, ClientsModule.Authentification);
    }

    void OnClientConnected(int coID)
    {
        ClientsModule.RegisterUnknownClient(coID);
        MessageSender.Send(new NetworkMessage(40000), coID, 0); // Envoi de la demande d'authentification.
        // Le type de message est le type de base "NetworkMessage" car l'unique contenu important du message est son type.
        // Ce genre de message est appelé "commande" car son utilité réside dans le fait d'appeler les bons handlers chez le récepteur
        // qui ne traitent pas de contenu, mais engendrent uniquement une réaction.
    }
}
