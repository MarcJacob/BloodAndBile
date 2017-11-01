using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Toute classe implémentant IWorldStateData peut être utilisé comme collection de données par un objet WorldState
/// et est susceptible d'être mit à jour.
/// </summary>
namespace BloodAndBileEngine.WorldState
{
    public interface IWorldStateData
    {
        void Simulate(float deltaTime); // Fait 'avancer le temps' pour les données concernée pendant le temps passé
        // en paramètre.
    }
}
