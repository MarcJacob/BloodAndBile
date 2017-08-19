using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCreationResponseMessage : NetworkMessage
{
    string HostName;
    string Password;
    string MatchName;
    string IP;

    public MatchCreationResponseMessage(string hostname, string passwd, string matchname, string ip) : base(40002)
    {
        HostName = hostname;
        Password = passwd;
        MatchName = matchname;
        IP = ip;
    }
}
