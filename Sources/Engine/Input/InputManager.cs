using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * <summary> Gestion des commandes. Une Commande est un objet composé d'un nom (string) et d'un tableau de paramètres (object[]).
 * Une commande peut être créee depuis n'importe où (il est préférable (c'est à dire obligatoire) de le faire dans des fichiers Input).
 * Quand une commande est crée, elle active un ensemble de fonctions "inscrites" à cette commande. </summary>
 */
namespace BloodAndBileEngine
{
    public static class InputManager
    {
        /**
         * <summary> Handlers. Chaque Action est une liste de méthodes à exécuter si la commande liée est envoyée. </summary>
         */
        static Dictionary<string, Action<object[]>> CommandHandlers = new Dictionary<string, Action<object[]>>();

        static public void AddHandler(string commandName, Action<object[]> handler)
        {
            if (CommandHandlers.ContainsKey(commandName))
            {
                CommandHandlers[commandName] += handler;
            }
            else
            {
                CommandHandlers.Add(commandName, handler);
                Debugger.Log("Nouvelle commande : '" + commandName + "'");
            }
        }

        static public void RemoveHandler(string commandName, Action<object[]> handler)
        {
            if (CommandHandlers.ContainsKey(commandName))
            {
                CommandHandlers[commandName] -= handler;
                if (CommandHandlers[commandName] == null)
                {
                    Debugger.Log("Commande retirée : " + commandName);
                    CommandHandlers.Remove(commandName);
                }
            }
            else
            {
                Debugger.Log("ERREUR - Cette commande n'existe pas.");
            }
        }

        /**
         * <summary> Envoie une commande. Si command est lié à un objet Action dans le dictionnaire des handler, cet objet Action sera
         * exécuté. Des paramètres peuvent / doivent être passés en fonction de la commande sous la forme d'un tableau d'objets. </summary>
         */
        static public void SendCommand(string command, object[] parameters = null)
        {
            Debugger.Log("Execution de la commande '" + command + "'");
            if (CommandHandlers.ContainsKey(command) && CommandHandlers[command] != null)
            {
                CommandHandlers[command](parameters);
            }
            else
            {
                Debugger.Log("La commande '" + command + "' est inconnue !", Color.red);
            }
        }

        static public string[] GetExistingCommands()
        {
            List<string> commands = new List<string>();
            foreach(string command in CommandHandlers.Keys)
            {
                commands.Add(command);
            }

            return commands.ToArray();
        }
    }
}