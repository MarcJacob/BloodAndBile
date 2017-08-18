using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Un match est en cours.</summary>
 */ 
public class PlayingState : ClientState
{
    // Propriétés.

    string MatchIP; // IP de l'hôte du match.

    //

        // CONSTRUCTEUR

        /**
         * <summary> Initialise le PlayingState. Nécessite une adresse IP ("127.0.0.1" si localhost) pour savoir à quel match se connecter.
         *  </summary>
         */ 
    public PlayingState(string IP)
    {
        MatchIP = IP;
    }
    
    /**
        * <summary> Tentative de connexion au Match à l'IP spécifiée dans le constructeur et setup des handlers. </summary>
        */ 
    public override void Init()
    {
        
    }

    public override void Inputs()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}
