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


        public EntitySynchroniserComponent(Entity linked) : base(linked)
        {
            SynchData = new EntitySynchronizationDataObject(linked.ID);
        }

        public EntitySynchronizationDataObject GetSynchronizationData()
        {
            return SynchData;
        }

        public override void Initialise()
        {
            
        }

        // Met à jour l'objet "EntitySynchroniserObject" lié à cette instance.
        public override void Update(float deltaTime)
        {
            // A chaque synchronisation : récolte des données de synchronisation auprès de chaque
            // component de l'entité implémentant IEntitySynchroniser.
            // Ajoute également les informations de base de l'entité : Position, Rotation, Cellule actuelle,
            // taille, hauteur.
            foreach(EntityComponent component in LinkedEntity.GetComponents())
            {
                if (component is IEntitySynchroniser)
                {
                    ((IEntitySynchroniser)component).GetSynchInfo(SynchData);
                }
            }
            SynchData.SetSynchInfo("Position", LinkedEntity.Position);
            SynchData.SetSynchInfo("Rotation", LinkedEntity.Rotation);
            SynchData.SetSynchInfo("CurrentCell", LinkedEntity.CurrentCellID);
            SynchData.SetSynchInfo("Size", LinkedEntity.Size);
            SynchData.SetSynchInfo("Height", LinkedEntity.Height);
        }

        /// <summary>
        /// Appelle "OnSynch" de tous les Components implémentants IEntitySynchroniser de cette entité.
        /// Synchronise également la Position, la Rotation, la Cellule actuelle,
        /// la taille, et la hauteur.
        /// </summary>
        public void OnSynch()
        {
            // Synchronisation des propriétés basiques :
            if (SynchData == null)
            {
                Debugger.Log("ERREUR : le SynchData n'a pas été initialisé !", UnityEngine.Color.red);
            }
            Debugger.Log("Synch position", UnityEngine.Color.yellow);
            LinkedEntity.Position = (BloodAndBileEngine.SerializableVector3)SynchData.GetSynchInfo("Position");
            Debugger.Log("Synch rotation", UnityEngine.Color.yellow);
            LinkedEntity.Rotation = (BloodAndBileEngine.SerializableQuaternion)SynchData.GetSynchInfo("Rotation");
            Debugger.Log("Synch CellID", UnityEngine.Color.yellow);
            LinkedEntity.SetCellID((int)SynchData.GetSynchInfo("CurrentCell"));
            Debugger.Log("Synch Size", UnityEngine.Color.yellow);
            LinkedEntity.Size = (float)SynchData.GetSynchInfo("Size");
            Debugger.Log("Synch Height", UnityEngine.Color.yellow);
            LinkedEntity.Height = (float)SynchData.GetSynchInfo("Height");
        }

        public override uint GetComponentID()
        {
            return 0; // Component ID 0 : Le Synchronizer lui même.
        }
    }
}
