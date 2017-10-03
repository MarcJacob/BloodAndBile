using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Ce module assure la gestion du WorldState lié au Match. Il contient donc indirectement toutes les données
/// de la partie en cours.
/// 
/// Il sert également à construire le message StateUpdate.
/// </summary>
public class WorldStateModule : MatchModule
{
    public WorldStateModule(Match m) : base(m)
    {

    }


}
