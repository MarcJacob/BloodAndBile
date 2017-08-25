using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * <summary> Réponse du client à une demande d'identification du match. </summary>
 */ 
[Serializable]
public class MatchConnectionMessage : NetworkMessage
{
    public string Username;
    public string Password;

    public MatchConnectionMessage(string username, string password = "") : base(2)
    {
        Username = username;
        Password = password;
    }
}
