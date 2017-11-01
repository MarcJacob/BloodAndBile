using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contient l'ensemble des Controllers d'un WorldState et est capable de les mettre à jour.
/// </summary>
namespace BloodAndBileEngine.WorldState.WorldStateData.ControllersManager
{
    public class ControllersManager : IWorldStateData
    {
        List<Controller> Controllers = new List<Controller>();

        public void Simulate(float deltaTime)
        {
            List<Controller> deadControllers = new List<Controller>(); // Controllers dont l'entité associée a été détruite.
            foreach(Controller c in Controllers)
            {
                if (c.GetControlledEntity().Destroyed)
                {
                    deadControllers.Add(c);
                }
                else
                {
                    c.Update(deltaTime);
                }
            }

            foreach(Controller c in deadControllers)
            {
                Controllers.Remove(c);
            }
        }

        public void AddController(Controller newController) // Ajouter un Controller à la liste des Controller de ce WorldState. Le Controller doit être crée dans une classe Factory faite pour cela.
        {
            Controllers.Add(newController);
        }
    }
}
