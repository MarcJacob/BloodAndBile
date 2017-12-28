using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine.Networking
{
    [Serializable]
    /// <summary>
    /// Message destiné à envoyer des messages sur le réseau,
    /// Command = commande à executer
    /// 
    /// Args = liste des arguments de la commande, peut etre vide
    /// </summary>
    class NetworkCommandMessage : NetworkMessage
    {
        public string Command { get; private set; }
        public object[] Args { get; private set; }

        public NetworkCommandMessage(string command, object[] args = null) : base(60002)
        {
            Command = command;
            Args = args;
        }


    }
}
