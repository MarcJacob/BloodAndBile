using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MatchListMessage : NetworkMessage
{
    public MatchInfo[] Infos;

    public MatchListMessage(MatchInfo[] info) : base(40003)
    {
        Infos = info;
    }
}