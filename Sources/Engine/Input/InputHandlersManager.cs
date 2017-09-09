using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/**
 * <summary> Un HandlersManager est un ensemble de Handlers pour les messages réseau. Passer par cette classe permet de facilement
 * en ajouter et de les supprimer. </summary>
 */
namespace BloodAndBileEngine
{
    public class InputHandlersManager
    {
        Dictionary<string, Action<object[]>> Handlers = new Dictionary<string, Action<object[]>>();

        public void Add(string commandName, Action<object[]> handler)
        {
            if (Handlers.ContainsKey(commandName))
                Handlers[commandName] += handler;
            else
                Handlers.Add(commandName, handler);

            InputManager.AddHandler(commandName, handler);
        }

        /**
         * <summary> Supprime tous les handlers de ce InputHandlerManager. </summary>
         */ 
        public void Clear()
        {
            foreach(string type in Handlers.Keys)
            {
                Debugger.Log("InputHandlersManager - Supression de " + type);
                InputManager.RemoveHandler(type, Handlers[type]);
            }
            Handlers.Clear();
        }

        /**
         * <summary> Appelé lorsque l'objet passe au garbage collector.
         * Avant que l'objet ne soit détruit, on veut enlever tous les handlers qu'il a déclaré dans le InputManager pour éviter
         * d'appeler des handlers qui n'existent plus. </summary>
         */ 
        ~InputHandlersManager()
        {
            Clear();
        }
    }
}
