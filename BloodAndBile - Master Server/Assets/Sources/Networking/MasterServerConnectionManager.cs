using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary> Gère la connexion au Master Server. Permet d'obtenir un match que le Client se mettra à "écouter" à travers le
 * Network Interface. NOTE : Lorsqu'un match est en cours, la connection au Master Server est facultative mais à défaut de celle ci,
 * cet objet s'occupera d'afficher un bout d'interface pour prévenir le joueur, puis essayera de s'y reconnecter régulièrement. </summary>
 */ 
public class MasterServerConnectionManager : MonoBehaviour
{
    public string MasterServerIP; // - IP du Master Server. 
    public int MasterServerPort; // - Port du Master Server. // TODO : Charger l'IP et le Port du Master Server depuis un fichier de config.

    bool Connected = false; // Sommes-nous connectés au Master Server ?
    int MasterServerConnectionID;
    int ConnectionAttemptFrequency = 1; // Combien de fois par seconde essayons nous de nous connecter au Master Server ?
    float CurrentConnectionAttemptCooldown = 0f; // Cooldown

    /**
     * <summary> Tente une connexion au Master Server à l'IP MasterServerIP et au Port MasterServerPort. </summary>
     */ 
    void AttemptConnection()
    {

    }

    /**
    * <summary> Executé lorsque la connexion au Master Server n'est pas établie. </summary>
    */
    void StateDisconnected()
    {
        CurrentConnectionAttemptCooldown -= Time.deltaTime;
        if (CurrentConnectionAttemptCooldown <= 0)
        {
            Debug.Log("Tentative de connexion au Master Server...");
            CurrentConnectionAttemptCooldown = 1.0f / (float)ConnectionAttemptFrequency;
            AttemptConnection();
        }
    }

    /**
     * <summary> Executé lorsque la connexion au Master Server est perdue.
     * Note : la connexion au Master Server n'est JAMAIS sensé être perdue volontairement. </summary>
     */
    void OnMasterServerDisconnected()
    {

    }

    /**
     * <summary> Exécuté à chaque image lorsque l'on est connecté au Master Server. </summary>
     */
    void StateConnected()
    {

    }

    /**
     * <summary> Exécuté au moment où la connection au Master Server est établie. </summary>
     */ 
    void OnMasterServerConnected()
    {
        Connected = true;
    }

    private void Update()
    {
        if (Connected)
        {
            StateConnected();
        }
        else
        {
            StateDisconnected();
        }
    }


}