using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Une instance de sort permet de gérer des informations sur un sort entrain d'être lancé propres à ce lancement de sort : par exemple,
/// si on veut qu'un sort soit canalisé, une instance de sort permettra de traquer le temps 
/// Les SpellInstance sont crées par les Spells lorsqu'on leur demande d'être lancé par une entité.
/// Le Spell va créer le SpellInstance, et lui attribuer les comportements voulus.
/// Le SpellInstance sera retourné par la fonction de lancement de Spell, ce qui permettra, si besoin, de garder le SpellInstance en mémoire
/// (cela est fait dans le SpellComponent : le SpellInstance du sort lancé actuellement est conservé).
/// 
/// Les fonctions de comportement d'un spell demandent un SpellInstance en paramètre, donc un SpellInstance peut contenir toute information
/// propre à elle même : celles ci seront accessibles dans la fonction écrite dans la classe SpellsManager.
/// </summary>
namespace BloodAndBileEngine
{
    public class SpellInstance
    {
        // Propriétés propres à une instance de sort
        public float ElapsedTime { get; private set; } // Temps écoulé depuis le lancement de ce sort.
        float DeltaTime; // Variable DeltaTime pour le prochain appel de "OnCanalise".

        //..

        SpellFlags Flags; // Flags liés à ce sort. 
        Entity Caster; // Entité ayant lancé ce sort.
        object Target; // Cible du sort. Si le Sort est marqué CAN_CHANGE_TARGET, la cible est constamment mise à jour.
        uint SpellID; // ID du sort d'origine
        public bool Cancelled = false; // Ce sort a-t-il été annulé ?
        public bool Casted = false; // Ce sort a-t-il été Cast ?

        Action<SpellInstance> OnCast; // Au lancement du sort : après que ElapsedTime soit supérieur ou égal à CastTime dans Spell.
        Action<SpellInstance> OnCanalise; // Après OnCast, à chaque image. Utilise le deltaTime conservé dans SpellInstance (pour éviter d'avoir à le passer en paramètre).
        Action<SpellInstance> OnEnd; // Lorsque ElapsedTime > Duration (variable de Spell) ou si le sort n'est pas canalisé.
        Action<SpellInstance> OnCancel; // Lorsque le sort est canalisé et que son lancement est annulé quand ElapsedTime < Duration.

        public SpellInstance(uint spellID, Entity caster, object target, SpellFlags flags, Action<SpellInstance> onCast, Action<SpellInstance> onCanalisation, Action<SpellInstance> onEnd, Action<SpellInstance> onCancel)
        {
            Caster = caster;
            Target = target;
            OnCast = onCast;
            OnCanalise = onCanalisation;
            OnEnd = onEnd;
            OnCancel = onCancel;
            Flags = flags;
            SpellID = spellID;
        }
        
        public void ExecuteOnCast()
        {
            OnCast(this);
        }

        public void ExecuteOnCanalise()
        {
            if (Flags.HasFlag(SpellFlags.CANALISED))
                OnCanalise(this);
        }

        public void ExecuteOnEnd()
        {
            if (Flags.HasFlag(SpellFlags.CANALISED))
                OnEnd(this);
        }

        public void ExecuteOnCancel()
        {
            if (Flags.HasFlag(SpellFlags.CANCELABLE))
                OnCancel(this);
        }

        public void ChangeTarget(object newTarget)
        {
            if (Flags.HasFlag(SpellFlags.CAN_CHANGE_TARGET))
            {
                if (newTarget is Entity && Flags.HasFlag(SpellFlags.TARGETS_ENTITY))
                {
                    Target = newTarget;
                }
                else if ((newTarget is SerializableVector3 || newTarget is UnityEngine.Vector3) && Flags.HasFlag(SpellFlags.TARGETS_POSITION))
                {
                    Target = newTarget;
                }
            }
        }

        public object GetTarget()
        {
            return Target;
        }

        public Entity GetCaster()
        {
            return Caster;
        }

        public void IncrementTime(float amount)
        {
            ElapsedTime += amount;
        }

        public Spell GetSpell()
        {
            return SpellsManager.GetSpellByID(SpellID);
        }
    }
}
