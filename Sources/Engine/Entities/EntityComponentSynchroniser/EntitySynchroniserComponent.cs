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
            foreach(EntityComponent component in LinkedEntity.GetComponents())
            {
                if (component is IEntitySynchroniser)
                {
                    ((IEntitySynchroniser)component).GetSynchInfo(SynchData);
                }
            }
        }

        public override uint GetComponentID()
        {
            return 0; // Component ID 0 : Le Synchronizer lui même.
        }
    }
}
