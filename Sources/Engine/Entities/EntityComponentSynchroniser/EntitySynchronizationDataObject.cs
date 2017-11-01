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
            Debugger.Log("SetSynchInfo() - " + syncInfoName);
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
                SyncInfo.Add(syncInfoObject);
            }
            else
            {
                syncInfoObject.Information = info;
            }
            
        }

        public object GetSynchInfo(string syncInfoName)
        {
            Debugger.Log("GetSynchInfo()");
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
                Debugger.Log("ERREUR : " + syncInfoName + " n'est pas présent dans ce EntitySynchronizationDataObject !" ,UnityEngine.Color.red);
                return null;
            }
        }

        public void SetSynchInfoFromSynchObject(EntitySynchronizationDataObject other)
        {
            foreach(StateUpdateObject obj in other.SyncInfo)
            {
                SetSynchInfo(obj.Type, obj.Information);
            }
        }

        uint EntityID; // Identifiant de l'entité
        public uint GetEntityID()
        {
            return EntityID;
        }
        List<StateUpdateObject> SyncInfo; // Ensemble des informations de synchronisation.
    }
}
