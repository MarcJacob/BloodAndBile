using System;
using System.Collections.Generic;
using UnityEngine;
/**
 * <summary> Lorsque le client se trouve dans le menu principal. </summary>
 */ 
class MainMenuState : ClientState
{
    override public void Init()
    {
        // Switch vers le menu principal.
        UIManagement.SwitchToUI("MainMenu");
    }

    override public void Update()
    {
        
    }

    override public void Inputs()
    {

    }

    public override void Exit()
    {
        
    }
}
