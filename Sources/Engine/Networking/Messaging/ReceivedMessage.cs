using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodAndBileEngine
{
    namespace Networking
    {
        public class ReceivedMessage
        {
            public NetworkMessage Message;

            public NetworkMessageInfo RecInfo;

            public ReceivedMessage(NetworkMessage message)
            {
                Message = message;
            }

        }
    }
}
