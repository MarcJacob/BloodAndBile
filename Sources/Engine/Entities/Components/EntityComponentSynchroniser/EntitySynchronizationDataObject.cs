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
            BasicSyncInfo = new List<StateUpdateObject>();
        }

        public void SetBasicSynchInfo(string syncInfoName, object info)
        {
            StateUpdateObject syncInfoObject = null;
            int i = 0;
            while(syncInfoObject == null && i < BasicSyncInfo.Count)
            {
                StateUpdateObject updateObject = BasicSyncInfo[i];
                if (updateObject.Type == syncInfoName)
                {
                    syncInfoObject = updateObject;
                }
                i++;
            }

            if (syncInfoObject == null)
            {
                syncInfoObject = new StateUpdateObject(syncInfoName, info);
                BasicSyncInfo.Add(syncInfoObject);
            }
            else
            {
                syncInfoObject.Information = info;
            }
            
        }

        public void SetComponentSynchInfo(Type type, StateUpdateObject[] data)
        {
            ComponentSynchronizationDataObject componentSynchObject = null;
            int index = 0;
            while (index < ComponentSyncInfo.Count && componentSynchObject == null)
            {
                if (ComponentSyncInfo[index].ComponentType == type)
                {
                    componentSynchObject = ComponentSyncInfo[index];
                }
                index++;
            }

            if (componentSynchObject != null) // Un ComponentSyncInfoDataObject a été trouvé !
            {
                // Ajout de tous les éléments du tableau de StateUpdateObject data à la
                // liste des StateUpdateObject de ce ComponentSyncInfoDataObject.
                foreach(StateUpdateObject dataObject in data)
                {
                    componentSynchObject.SetSynchInfo(dataObject.Type, dataObject.Information);
                }
            }
            else // Pas de ComponentSyncInfoDataObject trouvés ! On en crée un nouveau.
            {
                List<StateUpdateObject> dataList = new List<StateUpdateObject>();
                foreach(StateUpdateObject obj in data)
                { dataList.Add(obj); }
                ComponentSynchronizationDataObject newSynchObject = new ComponentSynchronizationDataObject(type, dataList);
                ComponentSyncInfo.Add(newSynchObject);
            }
        }

        public object GetSynchInfo(string syncInfoName)
        {
            StateUpdateObject syncInfoObject = null;
            int i = 0;
            while (syncInfoObject == null && i < BasicSyncInfo.Count)
            {
                StateUpdateObject updateObject = BasicSyncInfo[i];
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

        public ComponentSynchronizationDataObject[] GetComponentSynchData()
        {
            return ComponentSyncInfo.ToArray();
        }

        public void SetSynchInfoFromSynchObject(EntitySynchronizationDataObject other)
        {
            foreach(StateUpdateObject obj in other.BasicSyncInfo)
            {
                SetBasicSynchInfo(obj.Type, obj.Information);
            }
            foreach (ComponentSynchronizationDataObject obj in other.ComponentSyncInfo)
            {
                SetComponentSynchInfo(obj.ComponentType, obj.Data.ToArray());
            }
        }

        uint EntityID; // Identifiant de l'entité
        public uint GetEntityID()
        {
            return EntityID;
        }
        List<StateUpdateObject> BasicSyncInfo; // Ensemble des informations de synchronisation de base.
        List<ComponentSynchronizationDataObject> ComponentSyncInfo; // Ensemble des informations de synchronisation des Components.
    }
}
