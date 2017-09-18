using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IStateUpdater
{
    BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject[] GetStateUpdateInformations();
}
