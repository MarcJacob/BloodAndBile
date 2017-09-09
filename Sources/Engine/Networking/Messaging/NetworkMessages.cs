using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/**
 * <summary> Ensemble des types de message existants. </summary>
 */ 
namespace BloodAndBileEngine.Networking
{
    [Serializable]
        public class NetworkMessage
        {
            public ushort Type;

            public NetworkMessage(ushort type)
            {
                Type = type;
            }
        }
}