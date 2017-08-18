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
        MatchManager.HostMatch("MyMatch", "");
    }

    public override void Inputs()
    {
        //
    }

    public override void Update()
    {
        //
    }

    public override void Exit()
    {
        Client.ChangeState(new PlayingState("127.0.0.1"));
    }
}
