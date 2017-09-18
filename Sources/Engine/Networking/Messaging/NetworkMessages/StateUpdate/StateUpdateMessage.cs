using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Contient une liste de "StateUpdateObject" servants à mettre à jour des World clients par rapport à
 * un World source. L'ajout d'informations supplémentaires au StateUpdateMessage se fait à travers une méthode
 * d'ajout d'information : pas besoin de modifier cette classe. Les informations pourront alors être lus par les
 * clients.
 * 
 * ATTENTION : Ce message est normalement envoyé dans un channel StateUpdate, ce qui veut dire que les informations
 * envoyés par ce message ne sont pas garanties d'être reçus ou lus (le client ne lira que le dernier StateUpdate
 * reçu, et non pas les précédents même s'il ne les a pas lus !) </summary>
 */ 
namespace BloodAndBileEngine.Networking.Messaging.NetworkMessages
{
    public class StateUpdateMessage : NetworkMessage
    {
        StateUpdateObject[] StateUpdate;

        public StateUpdateMessage(StateUpdateObject[] stateUpdates) : base(20000)
        {
            StateUpdate = stateUpdates;
        }

        public StateUpdateObject[] GetStateUpdateInfo(int typeNumber)
        {
            return GetStateUpdateInfo((StateUpdateObjectType)typeNumber);
        }

        public StateUpdateObject[] GetStateUpdateInfo(StateUpdateObjectType type)
        {
            List<StateUpdateObject> updates = new List<StateUpdateObject>();
            foreach(StateUpdateObject update in StateUpdate)
            {
                if (update.Type == type)
                {
                    updates.Add(update);
                }
            }

            return updates.ToArray();
        }
    }
}