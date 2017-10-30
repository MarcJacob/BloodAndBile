using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BloodAndBileEngine.Networking.Messaging.NetworkMessages
{
    /// <summary>
    /// Un StateUpdateObject est un objet portant un nom et contenant de l'information sous forme d'un attribut
    /// "object". Fait pour être contenu à l'intérieur d'un StateUpdateMessage. L'information contenue ne doit pas
    /// être trop importante.
    /// ATTENTION : l'information envoyée n'a aucune garanties d'être reçue ! Le message pourrait ne pas être reçu par
    /// le client, mais il pourrait aussi arriver trop tard et se faire "remplacer" par un autre StateUpdate plus récent
    /// (le client ne lira que le dernier message reçu).
    /// </summary>
    [Serializable]
    public class StateUpdateObject
    {
        public string Type;
        public object Information;

        public StateUpdateObject(string type, object info)
        {
            Information = info;
            Type = type;
        }
    }
}
