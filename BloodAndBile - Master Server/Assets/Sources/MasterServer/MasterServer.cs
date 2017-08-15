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

    private void Init()
    {
        DontDestroyOnLoad(gameObject);
        NetworkSocket.Initialise();
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
		
	}

    private void Start()
    {
        SetupHandlers();
    }

    /**
     * <summary> L'INTEGRALITE des handlers du Master Server doivent être déclarés ici. </summary>
     */ 
    void SetupHandlers()
    {

    }
}
