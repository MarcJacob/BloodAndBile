using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodAndBileEngine
{
    namespace Networking
    {
        public struct NetworkMessageInfo
        {
            public int ConnectionID;

            public NetworkMessageInfo(int coID)
            {
                ConnectionID = coID;
            }
        } 
    }
}