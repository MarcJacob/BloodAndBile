using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/**
 * <summary> Classe wrapper utilisée lors de l'envoie d'un message sur le réseau. Permet de conserver des informations sur l'envoie
 * d'un message avant que l'envoie soit réellement fait. </summary>
 */
namespace BloodAndBileEngine
{
    namespace Networking
    {
        public class SentMessage
        {
            public int ConnectionID;
            public int ChannelID;
            public byte[] Buffer;
            NetworkMessage Message;

            public SentMessage(NetworkMessage msg, int ConnectionID, int ChannelID)
            {
                this.ConnectionID = ConnectionID;
                this.ChannelID = ChannelID;
                Message = msg;
            }

            public void Serialize()
            {
                bool isFragmented = ChannelID >= 5;

                // Conversion de l'objet en un tableau de bytes (Serialization).
                if (!isFragmented)
                    Buffer = new byte[NetworkReceiver.STANDARD_BUFFER_SIZE];
                else
                    Buffer = new byte[NetworkReceiver.FRAGMENTED_BUFFER_SIZE];
                MemoryStream stream = new MemoryStream(Buffer);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, Message);
            }
        } 
    }

}