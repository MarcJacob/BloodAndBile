using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary> Cette classe est très similaire à la classe NetworkListener présente sur le prototype : elle reçoit des messages,
 * détermine leur type et le restransmet aux fonctions "Handlers" correspondantes à ce type de message. 
 * 
 * La différence principale (et majeure !) est que ce n'est pas cette classe qui s'occupe de la réception des messages sur le réseau.
 * Cette classe s'occupe UNIQUEMENT de retransmettre les messages vers les fonctions handlers correspondantes. </summary>
 * 
 * CLASSE SINGLETON
 */ 
public class MessageReader : MonoBehaviour
{
    // STATIC
    static MessageReader Instance;
    static NetworkReceiver Receiver;
    /**
     * <summary> Ajoute un handler pour le type de message donné. 
     * ATTENTION : le Handler ne doit être crée qu'une seule fois ! Ne pas exécuter cette fonction avec le même handler plusieurs fois.</summary>
     */
    static public void AddHandler(ushort messageType, Action<NetworkMessageInfo, NetworkMessage> handlerFunction)
    {
        if (!Instance.Handlers.ContainsKey(messageType))
        {
            Instance.Handlers.Add(messageType, new List<Action<NetworkMessageInfo, NetworkMessage>>());
        }

        if (Instance.Handlers[messageType].Contains(handlerFunction))
        {
            Debug.Log("ATTENTION - MESSAGE READER - ADDHANDLER() - Le handler " + handlerFunction.Method.Name + " a déjà été crée pour le type de message " + messageType + " !");
            return;
        }


        Instance.Handlers[messageType].Add(handlerFunction);
    }

    /**
     * <summary> Ajoute un message à la queue des messages à traiter. Devrait être exécuté uniquement par les MessageReceiver sauf exceptions.</summary>
     */
    static public void AddMessageToQueue(ReceivedMessage msg)
    {
        Instance.MessageQueue.Enqueue(msg);
    }

    // _________________

    Queue<ReceivedMessage> MessageQueue = new Queue<ReceivedMessage>(); // Queue des messages reçus. "First in first out".

    Dictionary<ushort, List<Action<NetworkMessageInfo, NetworkMessage>>> Handlers; // Dictionnaire liant un type de message à un ensemble de fonctions "Handler" à exécuter
                                                      // En leur faisant passer le Message.
    /**
     * <summary> Exécutée lorsque cette instance devient le singleton de cette classe. </summary>
     **/ 
    void Init()
    {
        Debug.Log("Initialisation du MessageReader...");
        Handlers = new Dictionary<ushort, List<Action<NetworkMessageInfo, NetworkMessage>>>();
        Receiver = new NetworkReceiver();
    }

    void Start()
    {
        if (Instance != null)
        {
            Debug.Log("Seconde instance de MessageReader détectée ! Destruction...");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
    }

    /**
     * <summary> Update() normal de la classe MonoBehaviour (component). Dans ce cas, s'occupe de traiter la queue des messages. </summary>
     */ 
    void Update()
    {
        while (MessageQueue.Count > 0)
        {
            ReceivedMessage msg = MessageQueue.Dequeue();
            if (Handlers.ContainsKey(msg.Message.Type))
            {
                List<Action<NetworkMessageInfo, NetworkMessage>> handlers = Handlers[msg.Message.Type];
                foreach (Action<NetworkMessageInfo, NetworkMessage> handler in Handlers[msg.Message.Type])
                {
                    handler(msg.RecInfo, msg.Message); // Exécution des fonctions Handler correspondantes à ce type de message.
                }
            }
        }

        Receiver.Reception();
    }
}