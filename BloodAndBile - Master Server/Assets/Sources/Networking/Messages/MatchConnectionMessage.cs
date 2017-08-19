using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchConnectionMessage : NetworkMessage
{
    public string Username;

    public MatchConnectionMessage(string username) : base(2)
    {
        Username = username;
    }
}
