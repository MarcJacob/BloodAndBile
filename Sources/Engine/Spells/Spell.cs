using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Un Spell ("sort") est un objet persistant en mémoire capable d'appliquer un ou plusieurs effets.
/// Spell est une classe "Sealed" : il est impossible de créer des classes filles de Spell.
/// En effet, un Spell est définit par :
///     - Une source (L'entité ayant lancé le sort)
///     - Une cible (De type object : peut être une position, une entité, rien dutout...)
///     - Une fonction de lancement (définie dans le constructeur du sort)
///     - Optionnellement, une fonction de lancement continue (pour les sorts canalisés)
///     - Optionnellement, une fonction de fin de lancement (pour les sorts canalisés ayant un effet en fin de canalisation)
///     - Une durée de lancement, au début de laquelle la fonction de lancement sera effectuée, à la fin de laquelle la fonction de fin de lancement sera exécutée
///     et durant laquelle la fonction de lancement continue sera exécutée à chaque image.
///     - 
/// </summary>
namespace BloodAndBileEngine
{
    public sealed class Spell
    {
        public uint ID { get; private set; }
        public bool IDSet { get; private set; }
        public string Name;
        public string Desc;

        public float[] Costs; // [0] = Blood, [1] = Phlegm, [2] = Yellow Bile, [3] = Black Bile

        public float Cooldown;

        public Spell(string name, string desc, SpellFlags flags, float duration, float castTime, float cooldown, float costBlood, float costPhlegm, float costYB, float costBB, Action<SpellInstance> onCast, Action<SpellInstance> onCanalisation, Action<SpellInstance> onEnd, Action<SpellInstance> onCancel)
        {
            ID = 0;
            IDSet = false;
            Flags = flags;
            Duration = duration;
            CastTime = castTime;
            if (!Flags.HasFlag(SpellFlags.CANALISED)) // Si ce sort n'est pas canalisé, alors les actions OnCanalisation, OnEnd et OnCancel ne seront jamais exécutées.
            {
                onCanalisation = null;
                onEnd = null;
                onCancel = null;
            }

            OnCast = onCast;
            OnCanalisation = onCanalisation;
            OnEnd = onEnd;
            OnCancel = onCancel;

            Name = name;
            Desc = desc;

            Costs = new float[] { costBlood, costPhlegm, costYB, costBB };
            Cooldown = cooldown;
        }

        public float Duration { get; private set; }
        public float CastTime { get; private set; }

        SpellFlags Flags;
        Action<SpellInstance> OnCast;
        Action<SpellInstance> OnCanalisation;
        Action<SpellInstance> OnEnd;
        Action<SpellInstance> OnCancel;

        public void SetID(uint id)
        {
            ID = id;
            IDSet = true;
        }

        /// <summary>
        /// Crée un objet de type 'SpellInstance' et le renvoi après avoir exécuté la fonction "OnCast".
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        public SpellInstance Cast(Entity caster, object target)
        {
            SpellInstance newSpellInstance = new SpellInstance(ID, caster, target, Flags, OnCast, OnCanalisation, OnEnd, OnCancel);
            return newSpellInstance;
        }
    }

    public static class SpellFlagsExtension
    {
        /// <summary>
        /// Renvoi true si ce sort possède le flag passé en paramètre.
        /// </summary>
        public static bool HasFlag(this SpellFlags flag, SpellFlags f)
        {
            try
            {
                return (((int)(object)flag & (int)(object)f) == (int)(object)f);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// "Flags" de sort : définit certaines grandes lignes du comportement du sort (Canalisé ? Annulable ? Paralyse ? Auto-ciblage ? Peut changer de cible ? Peut cible une position ? Une Entité ?)
    /// </summary>
    [Flags]
    public enum SpellFlags
    {
        CANALISED, // Ce sort est-il canalisé ?
        CANCELABLE, // Ce sort est-il annulable ?
        ROOTS, // Ce sort paralyse-t-il le mage ?
        TARGETS_SELF, // Ce sort cible-t-il le mage ?
        TARGETS_ENTITY, // Ce sort peut-il cibler une entité ?
        TARGETS_POSITION, // Ce sort peut-il cibler une position ?
        CAN_CHANGE_TARGET, // Ce sort peut-il changer de cible durant son exécution ?
    }


}
