using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/**
 * <summary> Classe outil pour envoyer des messages (NetworkMessage) sur le réseau ou pas. 
 * Contient nottament la liste des messages à envoyer.</summary>
 */
public class MessageSender : MonoBehaviour
{
    // SINGLETON
    static MessageSender Instance;
    //

    // STATIC

    static public void Send(NetworkMessage message, int connectionID, int channelID = -1)
    {
        if (!NetworkTransport.IsStarted)
        {
            Debugger.Log("Impossible d'envoyer un message - NetworkTransport n'a pas été activé !");
            return;
        }

        bool isFragmented = channelID >= 5;

        // Conversion de l'objet en un tableau de bytes (Serialization).
        byte[] buffer;
        if (!isFragmented)
            buffer = new byte[NetworkReceiver.STANDARD_BUFFER_SIZE];
        else
            buffer = new byte[NetworkReceiver.FRAGMENTED_BUFFER_SIZE];
        MemoryStream stream = new MemoryStream(buffer);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(stream, message);
        byte error;
        if (channelID == -1)
            NetworkTransport.Send(NetworkSocket.HostID, connectionID, NetworkSocket.ChannelIDs[1], buffer, buffer.Length, out error);
        else
            NetworkTransport.Send(NetworkSocket.HostID, connectionID, channelID, buffer, buffer.Length, out error);

        if ((NetworkError)error != NetworkError.Ok)
        {
            Debugger.Log("Erreur lors de l'envoie d'un message ! Type : " + (NetworkError)error);
        }
    }

    //

    /**
    * <summary> Appelée quand cet objet devient l'Instance de classe. </summary>
    */ 
    void Init()
    {

    }

    // Monobehaviour

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
    }

    private void Update()
    {

    }
}