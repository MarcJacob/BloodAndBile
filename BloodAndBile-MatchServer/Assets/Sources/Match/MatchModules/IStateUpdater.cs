using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Un state updater est un module participant à l'envoi régulier de messages de mise à jour des WorldStates clients
/// selon le WorldState local au match.
/// </summary>
public interface IStateUpdater
{
    /// <summary>
    /// Renvoit l'ensemble des informations qu'un module souhaite ajouter au StateUpdate.
    /// </summary>
    /// <returns></returns>
    BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject[] GetStateUpdateInformation();

    /// <summary>
    /// Renvoit l'ensemble des informations qu'un module souhaite ajouter au premier StateUpdate après la création
    /// d'un match.
    /// </summary>
    /// <returns></returns>
    BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject[] GetConstructionStateInformation();
}
