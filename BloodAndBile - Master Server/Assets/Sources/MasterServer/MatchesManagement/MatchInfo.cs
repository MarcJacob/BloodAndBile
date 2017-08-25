using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public struct MatchInfo
{
    public string HostIP;
    public int HostPort;
    public string HostName;
    public string MatchName;
    public bool Locked;

    public MatchInfo(string matchName, string hostName, string ip, int port, bool locked)
    {
        HostIP = ip;
        HostPort = port;
        MatchName = matchName;
        HostName = hostName;
        Locked = locked;
    }
}
