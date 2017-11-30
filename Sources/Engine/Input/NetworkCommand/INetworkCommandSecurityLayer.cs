using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// "Couche" de sécurité pour les commandes.
/// Toute classe implémentant cette interface doit pouvoir dire s'il gère la sécurité pour une commande donnée,
/// et dire si, d'après lui, une commande devrait pouvoir être exécutée ('laisser passer'), ou doit être
/// bloquée là ('arrêter').
/// </summary>
namespace BloodAndBileEngine
{
    public interface INetworkCommandSecurityLayer
    {
        bool CanCheckCommand(string command);
        bool DoSecurityCheck(string command, object[] args);
    }
}
