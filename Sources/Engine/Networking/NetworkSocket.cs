﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

/**
 * <summary> NetworkSocket est une classe statique représentant "l'identité" de la machine sur le réseau : Host ID, connexions en
 * cours, </summary>
 * 
 */
namespace BloodAndBileEngine
{
    namespace Networking
    {
        public static class NetworkSocket
        {
            static public int HostID { get; private set; } // HostID de ce Socket.
            static public List<int> ConnectionIDs { get; private set; } // Liste des connexions ouvertes.
                                                                        /**
                                                                         * <summary> Liste des canaux. Par défaut :
                                                                         * 0 = Reliable,
                                                                         * 1 = Unreliable,
                                                                         * 2 = ReliableStateUpdate,
                                                                         * 3 = UnreliableStateUpdate,
                                                                         * 4 = ReliableSequenced,
                                                                         * 5 = UnreliableFragmented, 
                                                                         * 6 = ReliableFragmented 
                                                                         * 7 = StateUpdate </summary>
                                                                         */
            static public List<byte> ChannelIDs { get; set; } // Liste des canaux.



            static bool Initialised = false;
            /**
             * <summary> Initialise le NetworkSocket sur cette machine. Le port est défini comme une constante dans la classe NetworkSocket. </summary>
             */
            static public void Initialise(int Port, int maxConnections = 4)
            {
                if (!Initialised)
                {
                    NetworkTransport.Init();
                    ConnectionConfig config = new ConnectionConfig();
                    ChannelIDs = new List<byte>();
                    ConnectionIDs = new List<int>();
                    ChannelIDs.Add(config.AddChannel(QosType.Reliable)); // Canal Reliable : les messages seront forcément reçus, mais pas forcément dans l'ordre.
                                                                         // ATTENTION : Ce canal demande plus du double des ressources nécessaires à l'envoi de messages Unreliable. A utiliser uniquement en
                                                                         // cas de stricte nécessité.
                    ChannelIDs.Add(config.AddChannel(QosType.Unreliable)); // Canal Unreliable : les messages ne seront pas forcément reçus, et pas forcément dans l'ordre.
                                                                           // Ce canal est le moins coûteux en ressources et devrait donc être utilisé pour l'envoi de messages fréquents.
                    ChannelIDs.Add(config.AddChannel(QosType.ReliableStateUpdate)); // Canal ReliableStateUpdate : les messages seront forcéments reçus, et à la réception,
                                                                                    // seul le message arrivé le plus récemment sera prit en compte. Peut être utile, mais seulement quelques types de messages différents
                                                                                    // peuvent passer par ce canal, sinon ils pourraient s'annuler les uns les autres.
                    ChannelIDs.Add(config.AddChannel(QosType.StateUpdate)); // Comme ReliableStateUpdate, à la différence que les messages ne seront pas forcément reçus.

                    ChannelIDs.Add(config.AddChannel(QosType.ReliableSequenced)); // Canal ReliableSequences : les messages seront forcément reçus, et dans l'ordre.


                    ChannelIDs.Add(config.AddChannel(QosType.ReliableFragmented)); // Canal ReliableFragmented : les messages seront forcément reçus, et peuvent être plus
                                                                                   // gros car il est possible de les envoyer en fragments.
                    ChannelIDs.Add(config.AddChannel(QosType.UnreliableFragmented)); // Canal UnreliableFragmented : comme ReliableFragmented, mais les messages ne seront
                                                                                     // pas forcément reçus.

                    ChannelIDs.Add(config.AddChannel(QosType.StateUpdate)); // Canal StateUpdate : comme un unreliable, mais seul le message le plus
                    // récent sera lu (si un message arrive en retard, il ne sera pas lu).

                    config.PacketSize = 1470;

                    HostTopology ht = new HostTopology(config, maxConnections);


                    HostID = NetworkTransport.AddHost(ht, Port);
                    if (HostID < 0) // Y a - t - il déjà quelqu'un d'autre sur ce port ?
                    {
                        while (HostID < 0)
                        {
                            Port--;
                            HostID = NetworkTransport.AddHost(ht, Port);
                        }
                    }
                    Initialised = true;
                }
            }

            /**
             * <summary> Envoi une demande de connexion à l'adresse IP et port indiqués.
             * ATTENTION : la connexion n'est PAS immédiatement créée ! Cette fonction ne fait qu'envoyer une demande de connexion, qui
             * POURRAIT résulter en un établissement d'une connexion. </summary>
             */
            static public int ConnectTo(string ip, int port)
            {
                byte error;
                int coID = NetworkTransport.Connect(HostID, ip, port, 0, out error);
                if (error != 0)
                {
                    Debugger.Log("ERREUR LORS DE L'ENVOIE D'UNE DEMANDE DE CONNEXION : " + (NetworkError)error);
                    return -1;
                }
                return coID;
            }

            /**
             * <summary> Exécutée lorsqu'une connexion est établie. Déclenche OnConnectionEstablishedCallback. </summary>
             */
            static public void OnConnectionEstablished(int coID)
            {
                ConnectionIDs.Add(coID);
                if (OnConnectionEstablishedCallback != null)
                    OnConnectionEstablishedCallback(coID);
            }

            static Action<int> OnConnectionEstablishedCallback;
            static public void RegisterOnConnectionEstablishedCallback(Action<int> cb)
            {
                OnConnectionEstablishedCallback += cb;
            }
            static public void UnregisterOnConnectionEstablishedCallback(Action<int> cb)
            {
                OnConnectionEstablishedCallback -= cb;
            }

            static public void OnDisconnection(int coID)
            {
                if (OnDisconnectionCallback != null)
                {
                    OnDisconnectionCallback(coID);
                }
            }

            static Action<int> OnDisconnectionCallback;
            static public void RegisterOnDisconnectionCallback(Action<int> cb)
            {
                OnDisconnectionCallback += cb;
            }
            static public void UnregisterOnDisconnectionCallback(Action<int> cb)
            {
                OnDisconnectionCallback -= cb;
            }

            /**
             * <summary> Déconnecte ce NetworkSocket de la connexion indiquée. </summary>
             */
            static public void Disconnect(int coID)
            {
                byte error;
                NetworkTransport.Disconnect(HostID, coID, out error);
                if (error != 0)
                {
                    Debugger.Log("ERREUR LORS DE L'ENVOIE D'UN SIGNAL DE DECONNEXION : " + (NetworkError)error);
                }
                else
                {
                    ConnectionIDs.Remove(coID);
                }
            }

            /**
             * <summary> Déconnecte ce NetworkSocket de TOUTES ses connexions. </summary>
             */
            static public void Disconnect()
            {
                foreach (byte coID in ConnectionIDs)
                {
                    Disconnect(coID);
                }
            }

            static public ConnectionInfo GetConnectionInfoFromConnectionID(int coID)
            {
                ConnectionInfo info = new ConnectionInfo();
                UnityEngine.Networking.Types.NetworkID ID;
                UnityEngine.Networking.Types.NodeID Node;
                byte error;
                NetworkTransport.GetConnectionInfo(HostID, coID, out info.IP, out info.Port, out ID, out Node, out error);
                return info;
            }
        }
    }
}