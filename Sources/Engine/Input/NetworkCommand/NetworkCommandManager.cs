using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking;

namespace BloodAndBileEngine
{
    /// <summary>
    /// Gestionnaire des commandes à envoyer à une autre machine
    /// </summary>
   public static class NetworkCommandManager
    {
        static void NetworkCommandHandler(object[] args)
        {
            if (args.Length > 2)
            {
                int idConnection;
                if (args[0] is string)
                {
                    if (!Int32.TryParse((string)args[0], out idConnection))
                    {
                        Debugger.Log("Erreur conversion ID de connexion lors du TryParse", UnityEngine.Color.red);
                        return;
                    }
                }
                else if(args[0] is int)
                {
                    idConnection = (int)args[0];
                }
                else
                {
                    Debugger.Log("L'ID entré en argument n'est pas un int ni un string", UnityEngine.Color.red);
                    return;
                }
                string command = (string)args[1];
                string[] parameters = new string[args.Length - 2];
                for (int i = 2; i < args.Length; i++)
                {
                    parameters[i - 2] = args[i].ToString();
                }
                MessageSender.Send(new NetworkCommandMessage(command, parameters), idConnection);
            }
            else Debugger.Log("Trop peu d'arguments dans la commande 'NetCommand'", UnityEngine.Color.red);
        }

        public static void Initialize()
        {
            InputManager.AddHandler("NetCommand", NetworkCommandHandler);
            MessageReader.AddHandler(60002, NetworkCommandReceiver);
        }

        static void NetworkCommandReceiver(NetworkMessageInfo info, NetworkMessage message)
        {
            NetworkCommandMessage commandMessage = (NetworkCommandMessage)message;
            if(SecurityCheck(info.ConnectionID, commandMessage)) // Sécurité
            {
                // On a passé la sécurité ! Exécuter la commande.
                if (commandMessage.Args == null || commandMessage.Args.Length == 0)
                {
                    InputManager.SendCommand(commandMessage.Command);
                }
                else
                {
                    InputManager.SendCommand(commandMessage.Command, commandMessage.Args);
                }
            }
        }

        /// <summary>
        /// Couches de sécurité.
        /// Note : les couches de sécurité seront, la plupart du temps, définis dans le code des sous-projets Unity.
        /// </summary>
        static List<INetworkCommandSecurityLayer> SecurityLayers = new List<INetworkCommandSecurityLayer>()
        {

        };

        static public void AddSecurityLayer(INetworkCommandSecurityLayer layer)
        {
            bool contains = false;
            int i = 0;
            while (!contains && ++i < SecurityLayers.Count)
            {
                if (SecurityLayers[i].GetType() == layer.GetType()) // On évite d'avoir deux couches de sécurité du même type.
                {
                    contains = true;
                }
            }

            if (!contains)
            {
                SecurityLayers.Add(layer);
            }
        }

        static public void RemoveSecurityLayer<T>() where T : INetworkCommandSecurityLayer
        {
            int i = 0;
            bool removed = false;
            while (!removed && ++i < SecurityLayers.Count)
            {
                if (SecurityLayers[i].GetType() == typeof(T))
                {
                    removed = true;
                    SecurityLayers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Itère à travers toutes les couches de sécurité savant gérer la commande passée en paramètres.
        /// </summary>
        /// <returns></returns>
        static bool SecurityCheck(int connectionID, NetworkCommandMessage commandMessage)
        {
            bool passed = true; // Par défaut à true au cas où il n'y aurait aucune couche ne supportant cette commande.
            int i = 0;
            while (++i < SecurityLayers.Count && passed)
            {
                if (SecurityLayers[i].CanCheckCommand(commandMessage.Command))
                {
                    passed = SecurityLayers[i].DoSecurityCheck(commandMessage.Command, commandMessage.Args);
                }
            }

            return passed;
        }
    }
}
