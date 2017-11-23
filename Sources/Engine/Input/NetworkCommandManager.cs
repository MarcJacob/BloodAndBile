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
                    if (Int32.TryParse((string)args[0], out idConnection))
                    {
                        idConnection = (int)args[0];
                    }
                    else
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
                    parameters[i - 2] = (string)args[i];
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
            if(commandMessage.Args.Length > 1) InputManager.SendCommand(commandMessage.Command, commandMessage.Args);
            else InputManager.SendCommand(commandMessage.Command);
        }
    }
}
