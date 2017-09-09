using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/**
 * <summary> Message de réponse général à une requête : contient un booléen (oui ou non) et une chaine de caractère en raison. 
 * Il peut être utilisé pour n'importe quel situation de "Gate keeping" (accepter ou pas) </summary>
 */ 
namespace BloodAndBileEngine.Networking.NetworkMessages
{
    [Serializable]
    public class ConditionCheckResponseMessage : NetworkMessage
    {
        public bool Accepted;
        public string Reason;

        public ConditionCheckResponseMessage(bool accepted, string reason) : base(60001)
        {
            Accepted = accepted;
            Reason = reason;
        }
    }
}