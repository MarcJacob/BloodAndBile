using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayingState : IClientState
{
    // World LocalWorld;

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
