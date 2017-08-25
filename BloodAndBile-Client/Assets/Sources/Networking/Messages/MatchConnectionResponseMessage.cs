using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * <summary> Réponse du match à l'identification d'un client. Similaire au AuthentificationResponseMessage, séparé pour des raisons de clarté. </summary>
 */ 
[Serializable]
public class MatchConnectionResponseMessage : NetworkMessage
{
    public bool Accepted;
    public string Reason;

    public MatchConnectionResponseMessage(bool accepted, string reason) : base(20001)
    {
        Accepted = accepted;
        Reason = reason;
    }
}
