using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.WorldState;

/// <summary>
/// Le SpellComponent permet à une entité de lancer des sorts.
/// Il retient en mémoire l'instance du sort actuellement / dernièrement lancé sous forme d'un objet "SpellInstance".
/// </summary>
namespace BloodAndBileEngine
{
    public class SpellComponent : EntityComponent
    {
        SpellInstance CurrentSpell; // Sort actuellement lancé. null si pas de sort entrain d'être lancé.*

        public void CancelSpell()
        {
            if (CurrentSpell != null)
            {
                CurrentSpell.ExecuteOnCancel();
                if (CurrentSpell.Cancelled)
                {
                    CurrentSpell = null;
                }
            }
        }

        void ExecuteCast()
        {
            if (CurrentSpell != null)
            CurrentSpell.ExecuteOnCast();
        }

        void ExecuteCanalise()
        {
            if (CurrentSpell != null)
                CurrentSpell.ExecuteOnCanalise();
        }

        void ExecuteEnd()
        {
            if (CurrentSpell != null)
                CurrentSpell.ExecuteOnEnd();
        }

        public override void Initialise(WorldState.WorldState worldState)
        {
            CurrentSpell = null;
        }

        // Met le sort actuel à jour.
        public override void Update(float deltaTime)
        {
            if (CurrentSpell != null)
            {
                CurrentSpell.IncrementTime(deltaTime);
                Spell s = CurrentSpell.GetSpell();
                if (CurrentSpell.ElapsedTime > s.Duration)
                {
                    ExecuteEnd();
                }
                else if (CurrentSpell.ElapsedTime > s.CastTime)
                {
                    if (CurrentSpell.Casted)
                        ExecuteCanalise();
                    else
                        ExecuteCast();
                }
            }
        }

        public bool AttemptCast(uint spellID, object target)
        {
            if (CurrentSpell != null) // Si ce sort n'est pas nul
            {
                CancelSpell();
                if (CurrentSpell == null)
                {
                    Cast(spellID, target);
                    return true;
                }
            }
            else
            {
                Cast(spellID, target);
                return true;
            }

            return false;
        }

        void Cast(uint spellID, object target)
        {
            Spell s = SpellsManager.GetSpellByID(spellID);
            if (s != null)
            {
                CurrentSpell = s.Cast(LinkedEntity, target);
            }
        }
    }
}
