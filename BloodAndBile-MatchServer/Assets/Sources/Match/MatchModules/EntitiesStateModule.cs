using System;
using System.Collections.Generic;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// Module de match s'occupant de mettre à jour l'ensemble des entités du WorldState lié au match et de construire
/// le StateUpdate contenant les informations importantes sur les entités comme :
/// - Leur position
/// - Leur rotation
/// (Ajouter à la liste au fur et à mesure).
/// </summary>
public class EntitiesStateModule : MatchModule, IStateUpdater
{
    public EntitiesStateModule(Match match) : base(match)
    {

    }

    public override void Initialise()
    {
        base.Initialise();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Stop()
    {
        base.Stop();
    }

    public StateUpdateObject[] GetStateUpdateInformation()
    {

    }

    public StateUpdateObject[] GetConstructionStateInformation()
    {

    }
}
