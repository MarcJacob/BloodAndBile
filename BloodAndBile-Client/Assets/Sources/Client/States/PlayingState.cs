using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Etat durant lequel le Client est en jeu. S'occupe d'afficher l'interface de jeu, et des mouvements de caméra.
/// </summary>
public class PlayingState : IClientState
{
   
    bool UpdateLocalWorld = false;
    public PlayingState(bool updateLocalWorld = true)
    {
        UpdateLocalWorld = updateLocalWorld;
    }

    public void OnEntry()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
