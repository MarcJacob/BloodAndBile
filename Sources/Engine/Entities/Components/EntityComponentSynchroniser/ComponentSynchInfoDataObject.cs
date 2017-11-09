using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Objet contenant un Type de component et un ensemble de StateUpdateObjects associés à
/// ce component. Ce type d'objet est utilisé par les EntitySynchronizationDataObject donc
/// il faut passer par ce dernier pour connaitre l'ID de l'entité.
/// </summary>
namespace BloodAndBileEngine
{
    [Serializable]
    public class ComponentSynchronizationDataObject
    {
        public Type ComponentType;
        public List<StateUpdateObject> Data;

        public void SetSynchInfo(string syncInfoName, object info)
        {
            Debugger.Log("SetSynchInfo() - " + syncInfoName);
            StateUpdateObject syncInfoObject = null;
            int i = 0;
            while (syncInfoObject == null && i < Data.Count)
            {
                StateUpdateObject updateObject = Data[i];
                if (updateObject.Type == syncInfoName)
                {
                    syncInfoObject = updateObject;
                }
                i++;
            }

            if (syncInfoObject == null)
            {
                syncInfoObject = new StateUpdateObject(syncInfoName, info);
                Data.Add(syncInfoObject);
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
            while (syncInfoObject == null && i < Data.Count)
            {
                StateUpdateObject updateObject = Data[i];
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
                Debugger.Log("ERREUR : " + syncInfoName + " n'est pas présent dans ce ComponentSynchInfoDataObject !", UnityEngine.Color.red);
                return null;
            }
        }

        public ComponentSynchronizationDataObject(Type type, List<StateUpdateObject> data)
        {
            ComponentType = type;
            Data = data;
        }
    }
}
