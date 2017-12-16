using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;
using BloodAndBileEngine.WorldState;

/// <summary>
/// Le SpellComponent permet à une entité de lancer des sorts.
/// Il retient en mémoire l'instance du sort actuellement / dernièrement lancé sous forme d'un objet "SpellInstance".
/// </summary>
namespace BloodAndBileEngine
{
    public class SpellComponent : EntityComponent, IEntitySynchroniser
    {
        public uint SelectedSpellId { get; private set; } // ID du sort actuellement sélectionné
        SpellInstance CurrentSpell; // Sort actuellement lancé. null si pas de sort entrain d'être lancé.*
        Dictionary<uint, UnityEngine.KeyCode> SpellKeyCodes; // Touche du clavier associée à chaque sort


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
            {
                bool canCast = true;
                float[] costs = SpellsManager.GetSpellByID(CurrentSpell.GetSpell().ID).OnCastCosts;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetBlood() < costs[0])
                    canCast = false;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetPhlegm() < costs[1])
                    canCast = false;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetYellowBile() < costs[2])
                    canCast = false;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetBlackBile() < costs[3])
                    canCast = false;
                if (canCast)
                {
                    ((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).RemoveHumors(costs);
                    CurrentSpell.ExecuteOnCast();
                }
                else
                {
                    ExecuteEnd();
                    Debugger.Log("Pas assez d'humeur par rapoort au coût de ce sort !");
                }
            }
        }

        void ExecuteCanalise()
        {
            if (CurrentSpell != null)
            {
                bool canCast = true;
                float[] costs = SpellsManager.GetSpellByID(CurrentSpell.GetSpell().ID).OnCastCosts;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetBlood() < costs[0])
                    canCast = false;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetPhlegm() < costs[1])
                    canCast = false;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetYellowBile() < costs[2])
                    canCast = false;
                if (((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).GetBlackBile() < costs[3])
                    canCast = false;
                if (canCast)
                {
                    ((HumorsComponent)LinkedEntity.GetComponent(typeof(HumorsComponent))).RemoveHumors(costs);
                    CurrentSpell.ExecuteOnCanalise();
                }
                else
                {
                    ExecuteEnd();
                    Debugger.Log("Pas assez d'humeur par rapoort au coût de ce sort !");
                }
            }
        }

        void ExecuteEnd()
        {
            if (CurrentSpell != null)
                CurrentSpell.ExecuteOnEnd();
            CurrentSpell = null;
        }

        public override void Initialise(WorldState.WorldState worldState)
        {
            CurrentSpell = null;
            SpellKeyCodes = new Dictionary<uint, UnityEngine.KeyCode>();
            if(SpellsManager.SpellsCount < 1)
                SpellsManager.GetSpellByID(0); // Pour initialiser le tableau de Spells s'il ne l'était pas déjà
            for(uint i = 0; i < SpellsManager.SpellsCount && i < 9; i++)
            {
                AddSpellKeyCode(i, UnityEngine.KeyCode.Alpha1 + (int)i);
                Debugger.Log("Sort d'ID " + i + " associé à la touche " + (UnityEngine.KeyCode.Alpha1 + (int)i).ToString(), UnityEngine.Color.magenta);
            }
            SelectedSpellId = 0;
        }
        
        public void AddSpellKeyCode(uint spellId, UnityEngine.KeyCode key)
        {
            bool existingSpell = false;
            foreach (uint sID in SpellKeyCodes.Keys)
            {
                if (sID == spellId)
                {
                    existingSpell = true;
                }
            }
            if (!existingSpell)
                SpellKeyCodes.Add(spellId, key);
        }
        public void ChangeKey(uint spellId, UnityEngine.KeyCode newKey)
        {
            foreach (uint sID in SpellKeyCodes.Keys)
            {
                if (sID == spellId)
                {
                    SpellKeyCodes[sID] = newKey;
                }
            }
        }
        public Dictionary<uint, UnityEngine.KeyCode> GetSpellKeyCodes()
        {
            return SpellKeyCodes;
        }
        public void SetSelectedSpellId(uint sId)
        {
            SelectedSpellId = sId;
        }

        // Met le sort actuel à jour.
        public override void Update(float deltaTime)
        {
            if (CurrentSpell != null)
            {
                CurrentSpell.IncrementTime(deltaTime);
                Spell s = CurrentSpell.GetSpell();
                if (!CurrentSpell.Casted && CurrentSpell.ElapsedTime >= s.CastTime)
                {
                    ExecuteCast();
                    CurrentSpell.Casted = true;
                }
                else if (CurrentSpell.ElapsedTime > s.Duration)
                {
                    ExecuteEnd();
                }
                else if (CurrentSpell.ElapsedTime > s.CastTime)
                {
                    if (CurrentSpell.Casted)
                        ExecuteCanalise();
                }
            }
        }

        public bool AttemptCast(uint spellID, object target=null)
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

        public StateUpdateObject[] GetSynchInfo()
        {
            StateUpdateObject spellKeys = new StateUpdateObject("SpellKeyCodes", SpellKeyCodes);
            return new StateUpdateObject[] { spellKeys };
        }

        public void OnSynch(ComponentSynchronizationDataObject synchData)
        {
            SpellKeyCodes = (Dictionary<uint, UnityEngine.KeyCode>)synchData.GetSynchInfo("SpellKeyCodes");
        }
    }
}
