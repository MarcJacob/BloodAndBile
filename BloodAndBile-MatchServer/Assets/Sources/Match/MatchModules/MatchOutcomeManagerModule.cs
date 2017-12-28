using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Module servant à gérer les conditions de victoire d'un match.
/// Par défaut, vérifie à chaque mise à jour le nombre de joueurs restants dans la partie.
/// Si celui ci est égal ou inférieur à VictoriousPlayerAmount, alors tous les joueurs
/// restants sont considérés comme victorieux et le match se termine là.
/// </summary>
public class MatchOutcomeManagerModule : MatchModule
{
    const byte VICTORIOUS_PLAYER_AMOUNT = 0;

    public MatchOutcomeManagerModule(Match match) : base(match)
    {

    }

    public override void Initialise()
    {
        base.Initialise();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        CheckVictoryConditions();
    }

    public override void OnPlayerDisconnected(int coID)
    {
        base.OnPlayerDisconnected(coID);
    }

    public override void Stop()
    {
        base.Stop();
    }

    /// <summary>
    /// Renvoi true si toutes les conditions de victoire sont réunies.
    /// </summary>
    protected virtual void CheckVictoryConditions()
    {
        int livingPlayersAmount = -1;

        EntitiesManagerModule entitiesModule = ModuleMatch.GetModule<EntitiesManagerModule>();
        if (entitiesModule != null)
        {
            livingPlayersAmount = entitiesModule.GetPlayerControlledEntityLinks().Length;
        }

        if (livingPlayersAmount > -1 && livingPlayersAmount <= VICTORIOUS_PLAYER_AMOUNT)
        {
            BloodAndBileEngine.Debugger.Log("FIN DU MATCH");
            // Envoi d'un message de victoire à tous les joueurs en vie.
            List<int> victoriousPlayers = new List<int>();
            foreach(EntitiesManagerModule.PlayerToEntityLink link in entitiesModule.GetPlayerControlledEntityLinks())
            {
                OnVictoriousPlayer(link.ConnectionID);
                victoriousPlayers.Add(link.ConnectionID);
            }
            // Envoi d'un message de défaite à tous les joueurs morts
            foreach(int coID in ModuleMatch.GetPlayerConnectionIDs())
            {
                if (!victoriousPlayers.Contains(coID))
                {
                    OnDefeatedPlayer(coID);
                }
            }
            ModuleMatch.EndMatch();
        }
    }

    protected virtual void OnVictoriousPlayer(int connectionID)
    {
        ModuleMatch.SendCommandToPlayer(connectionID, "Log", "Vous êtes victorieux !");
    }

    protected virtual void OnDefeatedPlayer(int connectionID)
    {
        ModuleMatch.SendCommandToPlayer(connectionID, "Log", "Vous avez été défait...");
    }
}

