using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contient un ensemble de cellules qui seront copiés sur les WorldState locaux des clients recevant ce message.
/// </summary>
namespace BloodAndBileEngine.Networking.Messaging.NetworkMessages
{
    [Serializable]
    public class CellArrayMessage : NetworkMessage
    {
        public float[] Cells;

        public CellArrayMessage(float[] data) : base(20002)
        {
            Cells = data;
        }
    }
}
