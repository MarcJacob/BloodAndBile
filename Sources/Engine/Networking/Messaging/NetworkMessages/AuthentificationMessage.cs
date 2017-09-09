using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Message d'authentification d'une machine à une autre. Dans tous les cas, un nom et un mot de passe sont inclus.
 * "AuthentificationType" relève du type de machine se connectant à une autre : Client à MasterServer / MatchServer ou MatchServer à MasterServer. </summary>
 */ 
namespace BloodAndBileEngine.Networking.NetworkMessages
{
    [Serializable]
    public class AuthentificationMessage : NetworkMessage
    {
        public string Name;
        public string Password;
        public string IP; // Optionel.
        public AuthentificationType AuthType;

        public AuthentificationMessage(AuthentificationType type, string name, string pass, string ip = "") : base(60000)
        {
            Name = name;
            Password = pass;
            AuthType = type;
            IP = ip;
        }
    }

    public enum AuthentificationType
    {
        CLIENT,
        MATCH_SERVER
    }
}
