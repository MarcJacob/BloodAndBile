using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;
/// <summary>
/// Toute classe implémentant IStateUpdateReceiver est capable de réagir à la réception de messages "StateConstruction"
/// et "StateUpdate" et de modifier le WorldState local en conséquence.
/// 
/// Les informations reçues par message sont tranmises en paramètre sous forme d'un message "StateUpdate" ou "StateConstruction".
/// Les informations sont accessibles en utilisant les méthodes de recherche d'information de ces messages.
/// Attention ! StateConstruction ne sera généralement reçu qu'une seule fois, mais ce n'est pas garantie, ce qui
/// signifie que par sécurité, il faut toujours vérifier que les bonnes informations ont été reçues par message
/// avant de modifier le WorldState local.
/// </summary>
public interface IStateUpdateReceiver
{
    BloodAndBileEngine.WorldState.WorldState GetWorldState();

    void OnStateUpdate(StateUpdateMessage stateUpdate);
    void OnStateConstruction(StateConstructionMessage stateConstruction);
}

