using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Equivalent du IStateUpdater, à l'échelle d'une Entité : liste tous les composants implémentant l'interface
/// IEntitySynchroniser et leur "demande" de lister l'ensemble des informations à envoyer à leur homologue des WorldStates
/// locaux.
/// </summary>
namespace BloodAndBileEngine
{
    public class EntitySynchroniserComponent : EntityComponent
    {
        EntitySynchronizationDataObject SynchData; // Données de synchronisation.
        // Modifiées soit par une Update de ce component ou la réception
        // d'un StateUpdate contenant le StateUpdateObject portant le nom
        // "EntitiesSynchronization".


        public EntitySynchroniserComponent()
        {
           
        }

        public EntitySynchronizationDataObject GetSynchronizationData()
        {
            return SynchData;
        }

        public override void Initialise(BloodAndBileEngine.WorldState.WorldState worldState)
        {
            SynchData = new EntitySynchronizationDataObject(LinkedEntity.ID);
        }

        // Met à jour l'objet "EntitySynchroniserObject" lié à cette instance.
        public override void Update(float deltaTime)
        {
            // A chaque synchronisation : récolte des données de synchronisation auprès de chaque
            // component de l'entité implémentant IEntitySynchroniser.
            // Ajoute également les informations de base de l'entité : Position, Rotation, Cellule actuelle,
            // taille, hauteur.

            // Synchronisation des Components.
            foreach(EntityComponent component in LinkedEntity.GetComponents())
            {
                if (component is IEntitySynchroniser)
                {
                    SynchData.SetComponentSynchInfo(component.GetType(),((IEntitySynchroniser)component).GetSynchInfo());
                }
            }

            // Synchronisation des infos de base
            SynchData.SetBasicSynchInfo("Position", LinkedEntity.Position);
            SynchData.SetBasicSynchInfo("Rotation", LinkedEntity.Rotation);
            SynchData.SetBasicSynchInfo("CurrentCell", LinkedEntity.CurrentCellID);
            SynchData.SetBasicSynchInfo("Size", LinkedEntity.Size);
            SynchData.SetBasicSynchInfo("Height", LinkedEntity.Height);
        }

        /// <summary>
        /// Appelle "OnSynch" de tous les Components implémentants IEntitySynchroniser de cette entité.
        /// Synchronise également la Position, la Rotation, la Cellule actuelle,
        /// la taille, et la hauteur.
        /// </summary>
        public void OnSynch()
        {
            Debugger.Log("Test", UnityEngine.Color.cyan);
            if (SynchData == null)
            {
                Debugger.Log("ERREUR : le SynchData n'a pas été initialisé !", UnityEngine.Color.red);
                return;
            }
            // Synchronisation des propriétés basiques :
            LinkedEntity.Position = (BloodAndBileEngine.SerializableVector3)SynchData.GetSynchInfo("Position");
            LinkedEntity.Rotation = (BloodAndBileEngine.SerializableQuaternion)SynchData.GetSynchInfo("Rotation");
            LinkedEntity.SetCellID((int)SynchData.GetSynchInfo("CurrentCell"));
            LinkedEntity.Size = (float)SynchData.GetSynchInfo("Size");
            LinkedEntity.Height = (float)SynchData.GetSynchInfo("Height");

            // Synchronisation des Components
            // Pour chaque ComponentSynchronizationDataObject, on vérifie que l'entité
            // possède le component correspondant, puis on exécute le GetSynch() sur ce dernier.
            // Si l'entité n'a pas le component, on lui ajoute.
            foreach(ComponentSynchronizationDataObject componentData in SynchData.GetComponentSynchData())
            {
                EntityComponent component = LinkedEntity.GetComponent(componentData.ComponentType);
                if (component != null)
                {
                    if (component is IEntitySynchroniser)
                    {
                        ((IEntitySynchroniser)(component)).OnSynch(componentData); // On lance la synchronisation.
                    }
                }
                else // On ajoute le component
                {
                    component = LinkedEntity.AddComponent(componentData.ComponentType);
                    if (component != null && component is IEntitySynchroniser)
                    {
                        ((IEntitySynchroniser)(component)).OnSynch(componentData);
                    }
                }
            }
        }

    }
}
