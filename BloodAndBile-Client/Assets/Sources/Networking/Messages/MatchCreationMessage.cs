using System;
using System.Collections.Generic;

[Serializable]
public class MatchCreationMessage : NetworkMessage
{
    public MatchCreationMessage(string matchName, string password) : base (1)
    {
        MatchName = matchName;
        Password = password;
    }

    public string MatchName;
    public string Password;
}