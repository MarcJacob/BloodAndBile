using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Etat  </summary>
 */ 
public class HostingState : ClientState
{
    public override void Init()
    {
        MatchManager.HostMatch("MyMatch", "test");
    }

    public override void Inputs()
    {
        //
    }

    public override void Update()
    {
        MatchManager.UpdateMatch();
    }

    public override void Exit()
    {
    
    }
}
