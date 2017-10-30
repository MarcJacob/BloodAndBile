using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

namespace BloodAndBileEngine
{
    /// <summary>
    /// Contient un identifiant lié à une entité, et un ensemble de StateUpdateObjects contenants
    /// des informations. 
    /// </summary>
    [Serializable]
    public class EntitySynchronizationDataObject
    {
        public EntitySynchronizationDataObject(uint entityID)
        {
            EntityID = entityID;
            SyncInfo = new List<StateUpdateObject>();
        }

        public void SetSynchInfo(string syncInfoName, object info)
        {
            StateUpdateObject syncInfoObject = null;
            int i = 0;
            while(syncInfoObject == null && i < SyncInfo.Count)
            {
                StateUpdateObject updateObject = SyncInfo[i];
                if (updateObject.Type == syncInfoName)
                {
                    syncInfoObject = updateObject;
                }
                i++;
            }

            if (syncInfoObject == null)
            {
                syncInfoObject = new StateUpdateObject(syncInfoName, info);
            }
            else
            {
                syncInfoObject.Information = info;
            }
        }

        public object GetSynchInfo(string syncInfoName)
        {
            StateUpdateObject syncInfoObject = null;
            int i = 0;
            while (syncInfoObject == null && i < SyncInfo.Count)
            {
                StateUpdateObject updateObject = SyncInfo[i];
                if (updateObject.Type == syncInfoName)
                {
                    syncInfoObject = updateObject;
                }
                i++;
            }

            if (syncInfoObject != null)
            {
                return syncInfoObject.Information;
            }
            else
            {
                return null;
            }
        }

        uint EntityID; // Identifiant de l'entité
        List<StateUpdateObject> SyncInfo; // Ensemble des informations de synchronisation.
    }
}
