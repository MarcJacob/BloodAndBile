using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/**
 * <summary> Un HandlersManager est un ensemble de Handlers pour les messages réseau. Passer par cette classe permet de facilement
 * en ajouter et de les supprimer. </summary>
 */
namespace BloodAndBileEngine.Networking
{
    public class HandlersManager
    {
        Dictionary<ushort, Action<NetworkMessageInfo, NetworkMessage>> Handlers = new Dictionary<ushort, Action<NetworkMessageInfo, NetworkMessage>>();

        public void Add<T>(ushort messageType, Action<NetworkMessageInfo, T> handler) where T : NetworkMessage
        {
            // Pour ne pas avoir à constamment gérer les casts vers les types de message enfants, on fait la conversion ici en
            // encapsulant le handler donné (qui peut avoir n'importe quel type de message comme paramètre) avec une méthode anonyme
            // qui elle prend un NetworkMessage en deuxième paramètre et se contente de le convertir en le type voulu puis appel le "vrai" handler.

            Action<NetworkMessageInfo, NetworkMessage> wrapper = (NetworkMessageInfo info, NetworkMessage m) => {

                if (m is T)
                {
                    handler(info, (T)m);
                }
                else
                {
                    Debugger.Log("ERREUR - MAUVAIS TYPE DE MESSAGE ! TYPE : " + m.Type, Color.red);
                }

            };

            if (Handlers.ContainsKey(messageType))
                Handlers[messageType] += wrapper;
            else
                Handlers.Add(messageType, wrapper);

            MessageReader.AddHandler(messageType, wrapper);
        }

        /**
         * <summary> Supprime tous les handlers de ce HandlerManager. </summary>
         */ 
        public void Clear()
        {
            foreach(ushort type in Handlers.Keys)
            {
                MessageReader.RemoveHandler(type, Handlers[type]);
            }
            Handlers.Clear();
        }

        /**
         * <summary> Appelé lorsque l'objet passe au garbage collector.
         * Avant que l'objet ne soit détruit, on veut enlever tous les handlers qu'il a déclaré dans le MessageReader pour éviter
         * d'appeler des handlers qui n'existent plus. </summary>
         */ 
        ~HandlersManager()
        {
            Clear();
        }
    }
}
