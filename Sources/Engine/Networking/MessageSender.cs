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
namespace BloodAndBileEngine
{
    namespace Networking
    {
        public class MessageSender : MonoBehaviour
        {
            // SINGLETON
            static MessageSender Instance;
            //

            // STATIC

            static public void Send(NetworkMessage message, int connectionID, int channelID = -1)
            {
                Instance.MessagesToSend.Enqueue(new SentMessage(message, connectionID, channelID));
            }

            //

            public Queue<SentMessage> MessagesToSend;

            /**
            * <summary> Appelée quand cet objet devient l'Instance de classe. </summary>
            */
            void Init()
            {
                Instance = this;
                MessagesToSend = new Queue<SentMessage>();
            }

            // Monobehaviour

            public float Frequency = 20f; // Fréquence en Hertz de l'envoi des messages ajoutés à la queue "MessagesToSend".
            float messageClock = 0f;
            private void Start()
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

            /**
             * <summary> Envoi des messages dans la Queue des messages à envoyer. </summary>
             */
            void SendMessages()
            {
                while (MessagesToSend.Count > 0)
                {
                    SentMessage msg = MessagesToSend.Dequeue();
                    msg.Serialize();
                    byte error;
                    if (msg.ChannelID == -1)
                        NetworkTransport.Send(NetworkSocket.HostID, msg.ConnectionID, NetworkSocket.ChannelIDs[1], msg.Buffer, msg.Buffer.Length, out error);
                    else
                        NetworkTransport.Send(NetworkSocket.HostID, msg.ConnectionID, msg.ChannelID, msg.Buffer, msg.Buffer.Length, out error);

                    if ((NetworkError)error != NetworkError.Ok)
                    {
                        Debugger.Log("Erreur lors de l'envoie d'un message ! Type : " + (NetworkError)error);
                    }
                }
            }

            private void Update()
            {
                messageClock += Time.deltaTime;
                if (messageClock > 1f / Frequency)
                {
                    SendMessages();
                    messageClock = 0f;
                }
            }
        }
    }
}