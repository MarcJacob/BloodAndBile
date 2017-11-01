using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Un Controller est un "Cerveau" donnant des instructions à une entité en fonction des informations perçues par
/// celle ci et obtenues par le Controller.
/// Un Controller peut contrôler n'importe quel type d'entité à travers le système d'EntityComponent.
/// Controller elle même est une classe abstraite : il faut la dériver pour obtenir un comportement complet.
/// </summary>
namespace BloodAndBileEngine
{
    public abstract class Controller
    {
        Entity ControlledEntity; // Entité controllée.

        public Entity GetControlledEntity()
        {
            return ControlledEntity;
        }

        public abstract void Update(float deltaTime);

    }
}
