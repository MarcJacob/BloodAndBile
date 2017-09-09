using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/**
 * <summary> NetworkReceiver s'occupe de recevoir les messages sur le réseau. Cela inclut les messages "standards" qui seront retransmis
 * au MessageReader, et les messages "spéciaux" comme les demandes de connexion. </summary>
 * 
 * NOTE IMPORTANTE : Cette classe est techniquement un singleton car il n'existe qu'une référence à une instance de cette classe, dans le
 * NetworkSocket.
 */
namespace BloodAndBileEngine
{
    namespace Networking
    {
        public class NetworkReceiver
        {
            public const int STANDARD_BUFFER_SIZE = 1024;
            public const int FRAGMENTED_BUFFER_SIZE = 1024 * 32; // Le nombre de fragments maximal est de 32.

            // FORMATEUR BINAIRE
            static BinaryFormatter Formatter = new BinaryFormatter();

            public void Reception()
            {
                int recHostID;
                int recConnectionID;
                int recChannelID;
                int recSize;
                byte error;
                byte[] buffer = new byte[STANDARD_BUFFER_SIZE];

                NetworkEventType e;
                e = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, buffer, STANDARD_BUFFER_SIZE, out recSize, out error);
                if (error != 0)
                {
                    if ((NetworkError)error == NetworkError.MessageToLong)
                    {
                        // Re-recevoir le message avec un buffer plus grand.
                        buffer = new byte[FRAGMENTED_BUFFER_SIZE];
                        e = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, buffer, STANDARD_BUFFER_SIZE, out recSize, out error);
                    }
                    else
                    {
                        Debugger.Log("ERREUR - NETWORKRECEIVER - RECEPTION() - " + (NetworkError)error);
                    }
                }

                switch (e)
                {
                    case (NetworkEventType.ConnectEvent):
                        Debugger.Log("Connexion établie. ID = " + recConnectionID);
                        NetworkSocket.OnConnectionEstablished(recConnectionID);
                        break;
                    case (NetworkEventType.DisconnectEvent):
                        Debugger.Log("Connexion fermée. ID = " + recConnectionID);
                        NetworkSocket.OnDisconnection(recConnectionID);
                        break;
                    case (NetworkEventType.DataEvent):

                        // Des données ont été reçues. D'abord, construire l'objet ReceivedMessage en reconstituant le NetworkMessage :
                        MemoryStream stream = new MemoryStream(buffer);
                        ReceivedMessage msg = new ReceivedMessage((NetworkMessage)Formatter.Deserialize(stream));
                        // Ensuite, remplir l'objet NetworkMessageInfo de msg :
                        msg.RecInfo.ConnectionID = recConnectionID;

                        //_____________________________________________________
                        // Enfin, envoyer le ReceivedMessage au MessageReader.

                        MessageReader.AddMessageToQueue(msg);
                        break;

                }
            }
        }
    }
}
