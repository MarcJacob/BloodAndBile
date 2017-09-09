using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/**
 * <summary> Gestion des connexions du client. Permet de répertorier les ID de connexion par nom, pour facilement les retrouver. </summary>
 */ 
public static class ClientConnectionsManager
{
    static Dictionary<string, int> Connections = new Dictionary<string, int>();

    static public void Init()
    {
        BloodAndBileEngine.Networking.NetworkSocket.RegisterOnDisconnectionCallback(OnConnectionLost);
    }

    static void OnConnectionLost(int coID)
    {
        if (Connections.ContainsValue(coID))
        {
            string key = "";
            foreach (string k in Connections.Keys)
            {
                if (Connections[k] == coID) key = k;
            }
            if (key != "")
            {
                Connections.Remove(key);
            }
        }
    }

    static public void AddConnection(string name, int coID)
    {
        if (Connections.ContainsKey(name) || Connections.ContainsValue(coID))
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - CETTE CONNEXION EST DEJA REPERTORIEE !", UnityEngine.Color.red);
            return;
        }

        Connections.Add(name, coID);
    }

    static public int GetConnectionIDFromName(string name)
    {
        if (Connections.ContainsKey(name))
        {
            return Connections[name];
        }
        else
            return -1;
    }
}
