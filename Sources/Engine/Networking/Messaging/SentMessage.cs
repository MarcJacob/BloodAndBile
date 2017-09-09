using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            public SentMessage(byte[] buffer, int ConnectionID, int ChannelID)
            {
                this.ConnectionID = ConnectionID;
                this.ChannelID = ChannelID;
                this.Buffer = buffer;
            }
        } 
    }

}