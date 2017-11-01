using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Un module de match est exécuté à chaque itération d'un match. </summary>
 */ 
public class MatchModule
{
    protected Match ModuleMatch// Référence au Match actuel.
    {
        get
        {
            if (match.IsAlive)
                return (Match)match.Target;
            else
                return null;
        }
        set
        {
            match.Target = value;
        }
    }

    private WeakReference match = new WeakReference(null);

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