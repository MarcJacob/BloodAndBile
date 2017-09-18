using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Un module de match est exécuté à chaque itération d'un match. </summary>
 */ 
public class MatchModule
{
    protected Match ModuleMatch; // Référence au Match actuel.

    public MatchModule(Match m)
    {
        ModuleMatch = m;
    }

    public virtual void Initialise()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Stop()
    {

    }
}