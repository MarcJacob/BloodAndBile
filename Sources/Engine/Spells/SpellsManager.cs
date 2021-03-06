﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Gère la liste des sorts disponibles aux mages.
/// La liste des sorts est chargée au premier appel de GetSpellByID.
/// </summary>
namespace BloodAndBileEngine
{
    public static class SpellsManager
    {
        static bool Initialised = false; // La liste des sorts a-t-elle été initialisée ?
        static Spell[] Spells; // Ensemble des sorts.
        static public int SpellsCount = 0;
        static public Spell GetSpellByID(uint ID)
        {
            if (!Initialised)
            {
                BuildSpellArray();
                AssignIDs();
                Initialised = true;
            }

            if (Spells.Length > ID)
            {
                return Spells[ID];
            }
            else
            {
                return null;
            }
        }

        static void BuildSpellArray()
        {
            Spells = new Spell[] // Rédiger ici la liste de tous les sorts et leur code de comportement. TODO : écrire les caractéristiques d'un sort dans un fichier config et leur code dans un fichier LUA.
            {
                new Spell(

                    "Warp", // Nom du sort
                    "Warps the caster to the target area", // Description
                    SpellFlags.TARGETS_POSITION, // Flags du sort
                    0f, // Durée (Duration)
                    0.5f, // Temps de lancement (CastTime)
                    0f, // Cooldown
                    0f, // Coût Sang (Costs[0]) -OnCast
                    0f, // Coût Phlegm (Costs[1]) -OnCast
                    20f, // Coût Yellow Bile (Costs[2]) -OnCast
                    0f, // Coût Black Bile (Costs[3]) -OnCast
                    0f, // Coût Sang (Costs[0]) -OnCanalise
                    0f, // Coût Phlegm (Costs[1]) -OnCanalise
                    0f, // Coût Yellow Bile (Costs[2]) -OnCanalise
                    0f, // Coût Black Bile (Costs[3]) -OnCanalise
                    (instance) => // Quand le sort est lancé. Lancé après le CastTime.
                    {
                        // Téléporter le lanceur
                        Debugger.Log("Warp se lance");
                        if (instance.GetTarget() is UnityEngine.Vector3 || instance.GetTarget() is SerializableVector3)
                        {
                            SerializableVector3 pos = (SerializableVector3)instance.GetTarget();
                            WorldState.Cell targetCell = ((WorldState.CellSystem) instance.GetCaster().GetWorldState().GetData<WorldState.CellSystem>()).GetCellFromPosition(pos.z, pos.x);
                            if (targetCell != null)
                            {
                                pos.y = targetCell.GetHeightFrom2DCoordinates(pos.z, pos.x);
                            }
                            if (instance.GetCaster().GetComponent<EntityMover>() != null)
                            {
                                instance.GetCaster().GetComponent<EntityMover>().Teleport(pos.z, pos.x);
                                instance.GetCaster().GetComponent<EntityMover>().SetRootTime(1f);
                            }
                            else
                            {
                                instance.GetCaster().Position = pos;
                            }
                        }
                    },

                    (instance) => // Quand le sort est en cours de canalisation (que le joueur n'a pas relâché la touche de lancement et que la durée de vie du sort n'a pas dépassé la variable Duration.
                    {

                    },
                    
                    (instance) => // Quand le sort est terminé (durée atteinte)
                    {

                    },

                    (instance) => // Quand le sort est annulé.
                    {

                    } // NOTE : en pratique, le sort Warp n'est pas canalisable donc seule la fonction "OnCast" sera exécutée.
                    // Les autres fonctions sont toujours définies à titre d'exemple.
                ),
                new Spell(
                  "BloodToPhlegm", // Nom du sort
                    "Convert 20 Blood into 15 Phlegm", // Description
                    SpellFlags.TARGETS_SELF, // Flags du sort
                    0f, // Durée (Duration)
                    0.5f, // Temps de lancement (CastTime)
                    5f, // Cooldown
                    20f, // Coût Sang (Costs[0]) -OnCast
                    0f, // Coût Phlegm (Costs[1]) -OnCast
                    0f, // Coût Yellow Bile (Costs[2]) -OnCast
                    0f, // Coût Black Bile (Costs[3]) -OnCast
                    0f, // Coût Sang (Costs[0]) -OnCanalise
                    0f, // Coût Phlegm (Costs[1]) -OnCanalise
                    0f, // Coût Yellow Bile (Costs[2]) -OnCanalise
                    0f, // Coût Black Bile (Costs[3]) -OnCanalise
                    (instance) => // Quand le sort est lancé. Lancé après le CastTime.
                    {
                        ((HumorsComponent)instance.GetCaster().GetComponent(typeof(HumorsComponent))).ChangePhlegm(15);
          
                    },

                    (instance) => // Quand le sort est en cours de canalisation (que le joueur n'a pas relâché la touche de lancement et que la durée de vie du sort n'a pas dépassé la variable Duration.
                    {

                    },

                    (instance) => // Quand le sort est terminé (durée atteinte)
                    {

                    },

                    (instance) => // Quand le sort est annulé.
                    {

                    }
                ),
            };
            SpellsCount = Spells.Count();
        } // Fonction de construction des sorts

        static void AssignIDs()
        {
            for(uint i = 0; i < Spells.Length; i++)
            {
                Spells[i].SetID(i);
            }
        }
    }
}
