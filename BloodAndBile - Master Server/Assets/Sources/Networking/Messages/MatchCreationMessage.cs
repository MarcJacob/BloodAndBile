using System;
using System.Collections.Generic;

[Serializable]
public class MatchCreationMessage : NetworkMessage
{
    public MatchCreationMessage(string matchName, string password, string ip) : base (1)
    {
        MatchName = matchName;
        Password = password;
    }

    public string MatchName;
    public string Password;
}