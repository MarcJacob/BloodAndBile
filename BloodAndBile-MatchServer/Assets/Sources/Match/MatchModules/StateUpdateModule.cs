using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Le StateUpdateModule construit les messages StateUpdate à partir des autres modules puis les envois aux joueurs
/// connectés au Match.
/// </summary>
public class StateUpdateModule : MatchModule
{
    IStateUpdater[] StateUpdaterModules; // Ensemble des modules participants à la construction de messages StateUpdate.

    BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateMessage StateUpdateMessage;
    BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateConstructionMessage StateConstructionMessage;

    byte StateUpdateFrequency = 10; // Nombre de StateUpdate par seconde.
    float StateUpdateClock = 0f;

    public StateUpdateModule(Match match) : base(match)
    {

    }

    public override void Initialise()
    {
        base.Initialise();
        // Trouver tous les modules StateUpdater dans le Match.
        List<IStateUpdater> StateUpdaters = new List<IStateUpdater>() ;
        MatchModule[] modules = ModuleMatch.GetModules();
        foreach(MatchModule module in modules)
        {
            if (module is IStateUpdater)
            {
                StateUpdaters.Add((IStateUpdater)module);
            }
        }

        StateUpdaterModules = StateUpdaters.ToArray();

        BloodAndBileEngine.Debugger.Log("StateUpdateModule Initialisé ! " + StateUpdaterModules.Length + " modules StateUpdater.");
        CookConstructionState();
        SendStateConstruction();
    }

    public override void Update(float deltaTime)
    {
        BloodAndBileEngine.Debugger.Log("StateUpdateModule.Update()");
        StateUpdateClock += deltaTime;
        if (StateUpdateClock >= 1f / StateUpdateFrequency)
        {
            CookStateUpdate();
            SendStateUpdate();
            StateUpdateClock = 0f;
        }
        BloodAndBileEngine.Debugger.Log("/ StateUpdateModule.Update()");
    }

    public override void Stop()
    {
        base.Stop();
        StateUpdaterModules = null;
        StateUpdateMessage = null;
   }

    /// <summary>
    /// Visite les modules qui implémentent "StateUpdater" un par un et collecte leurs informations pour les rassembler
    /// dans le StateUpdateMessage.
    /// </summary>
    void CookStateUpdate()
    {
        List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject> stateUpdates = new List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject>();

        foreach(IStateUpdater module in StateUpdaterModules)
        {
            foreach(BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject stateUpdate in module.GetStateUpdateInformation())
            {
                stateUpdates.Add(stateUpdate);
            }
        }

        StateUpdateMessage = new BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateMessage(stateUpdates.ToArray());
    }

    void CookConstructionState()
    {
        List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject> stateUpdates = new List<BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject>();

        foreach (IStateUpdater module in StateUpdaterModules)
        {
            foreach (BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateUpdateObject stateUpdate in module.GetConstructionStateInformation())
            {
                stateUpdates.Add(stateUpdate);
            }
        }

        StateConstructionMessage = new BloodAndBileEngine.Networking.Messaging.NetworkMessages.StateConstructionMessage(stateUpdates.ToArray());
    }

    /// <summary>
    /// Envoi le message StateUpdate dans son état actuel vers les joueurs connectés à ce match sur le canal 7.
    /// </summary>
    void SendStateUpdate()
    {
        BloodAndBileEngine.Debugger.Log("Sending state update...");
        ModuleMatch.SendMessageToPlayers(StateUpdateMessage, 6);
        BloodAndBileEngine.Debugger.Log("State update sent !");
    }

    void SendStateConstruction()
    {
        BloodAndBileEngine.Debugger.Log("Sending state construction...");
        ModuleMatch.SendMessageToPlayers(StateConstructionMessage, 5);
    }
}
