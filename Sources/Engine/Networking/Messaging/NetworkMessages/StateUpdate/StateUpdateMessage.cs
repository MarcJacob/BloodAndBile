using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Contient une liste de "StateUpdateObject" servants à mettre à jour des World clients par rapport à
 * un World source. L'ajout d'informations supplémentaires au StateUpdateMessage se fait à travers une méthode
 * d'ajout d'information : pas besoin de modifier cette classe. Les informations pourront alors être lus par les
 * clients. </summary>
 */ 
namespace BloodAndBileEngine.Networking.Messaging.NetworkMessages
{
    [Serializable]
    public class StateUpdateMessage : NetworkMessage
    {
        StateUpdateObject[] StateUpdate;

        public StateUpdateMessage(StateUpdateObject[] stateUpdates) : base(20000)
        {
            StateUpdate = stateUpdates;
        }

        public StateUpdateObject[] GetStateUpdateInfo()
        {
            return StateUpdate;
        }

        public StateUpdateObject[] GetStateUpdateInfo(string type)
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

    [Serializable]
    public class StateConstructionMessage : NetworkMessage
    {
        StateUpdateObject[] StateConstruction;

        public StateConstructionMessage(StateUpdateObject[] stateUpdates) : base(20002)
        {
            StateConstruction = stateUpdates;
        }

        public StateUpdateObject[] GetStateConstructionInfo()
        {
            return StateConstruction;
        }

        public StateUpdateObject[] GetStateConstructionInfo(string type)
        {
            List<StateUpdateObject> updates = new List<StateUpdateObject>();
            foreach (StateUpdateObject update in StateConstruction)
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