using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    // Interface implémenté par un Component souhaitant être capable de faire de la synchronisation
    // par réseau. Note : TOUS les components sont partagés automatiquement. Les components
    // implémentants cette interface seront simplement capables de mettre à jour leurs informations
    // régulièrement.
    public interface IEntitySynchroniser
    {
        void GetSynchInfo(EntitySynchronizationDataObject synchObject);
        void OnSynch(); // Lorsqu'un message StateUpdate est reçu
    }
}
