using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Structure 'wrapper' des messages lors de la transmission des messages au MessageReader, et décomposé en deux lors de la transmission aux
 * handlers.
 */ 
public struct ReceivedMessage
{
    public NetworkMessageInfo RecInfo;

    public NetworkMessage Message; // Message contenu dans le NetworkMessageReceiver.

    public ReceivedMessage(NetworkMessage msg)
    {
        RecInfo = new NetworkMessageInfo();
        Message = msg;
    }
}

/**
 * <summary> Structure contenant diverses informations sur la réception d'un message comme la provenance </summary>
 */ 
public struct NetworkMessageInfo
{
    public int ConnectionID;
}
