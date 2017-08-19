using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MatchCreationResponseMessage : NetworkMessage
{
    public string HostName;
    public string Password;
    public string MatchName;
    public string IP;

    public MatchCreationResponseMessage(string hostname, string passwd, string matchname, string ip) : base(40002)
    {
        HostName = hostname;
        Password = passwd;
        MatchName = matchname;
        IP = ip;
    }
}
