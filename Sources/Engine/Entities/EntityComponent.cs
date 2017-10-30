using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Un EntityComponent est un "module" fait pour être lié à une entité. Il permet d'ajouter de la logique à cette
/// entité. Est mit à jour par l'entité elle même lors de son "Update".
/// </summary>
namespace BloodAndBileEngine
{
    public abstract class EntityComponent
    {
        protected Entity LinkedEntity; // Entité auquel ce Component est lié.

        public EntityComponent(Entity linked)
        {
            LinkedEntity = linked;
        }

        abstract public void Initialise(); // Appelé lorsque ce Component est ajouté à une Entité.
        abstract public void Update(float deltaTime); // Appelé à chaque mise à jour de l'entité.

        // Renvoi l'identifiant du Component.
        // Utilisé pour la synchronisation des Components sur un réseau : l'identifiant de chaque
        // components sera le même sur toutes les machines.
        abstract public uint GetComponentID();
    }
}
