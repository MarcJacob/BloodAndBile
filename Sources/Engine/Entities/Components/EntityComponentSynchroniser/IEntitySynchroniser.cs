using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;
namespace BloodAndBileEngine
{
    // Interface implémenté par un Component souhaitant être capable de faire de la synchronisation
    // par réseau. Note : TOUS les components sont partagés automatiquement. Les components
    // implémentants cette interface seront simplement capables de mettre à jour leurs informations
    // régulièrement.
    public interface IEntitySynchroniser
    {
        StateUpdateObject[] GetSynchInfo();
        void OnSynch(ComponentSynchronizationDataObject synchData); // Lorsqu'un message StateUpdate est reçu
    }
}
