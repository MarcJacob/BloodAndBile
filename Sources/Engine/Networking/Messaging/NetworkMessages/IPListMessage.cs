using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine.Networking.NetworkMessages
{
    [Serializable]
    public class IPListMessage : NetworkMessage
    {
        public string[] IPList;

        public IPListMessage(string[] IPs) : base (40000)
        {
            IPList = IPs;
        }
    }
}
