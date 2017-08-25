using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Structure d'envoi des messages. Lorsque le programme demande l'envoi d'un message, le message passé est "wrappé"
 * dans une structure SentMessage, qui contient l'information à envoyer, l'ID de connexion et de canal. </summary>
 */ 
public struct SentMessage
{
    public byte[] Buffer;
    public int ConnectionID;
    public int ChannelID;

    public SentMessage(byte[] buffer, int coID, int chanID)
    {
        Buffer = buffer;
        ConnectionID = coID;
        ChannelID = chanID;
    }
}
