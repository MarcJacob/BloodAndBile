using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Durant son déroulement, un match devra se comporter différement en fonction de plusieurs paramètres comme le temps écoulé,
 * si les joueurs ont indiqués qu'ils étaient prêts... certaines fonctionnalités peuvent ne plus être "supportés" quand il n'y en aura plus besoin
 * avant que le match soit terminé et détruit. Pour cela le Match a la possibilité d'être dans un certain état, et de faire la transition
 * vers un autre état. </summary>
 */ 
public class MatchState
{
    Match CurrentMatch; // Référence au match

    public virtual void OnStateEntered() { BloodAndBileEngine.Debugger.Log("Veuillez utiliser un Etat de Match autre que celui de base !", UnityEngine.Color.red);  } // A la première itération, quand l'état vient de "commencer".
    public virtual void OnStateExit() { BloodAndBileEngine.Debugger.Log("Veuillez utiliser un Etat de Match autre que celui de base !", UnityEngine.Color.red); } // Quand le Match fait une transition vers un autre état.
    public virtual void OnStateUpdate() { } // Exécuté à chaque itération tant que le match est en cours.

    public MatchState(Match match)
    {
        CurrentMatch = match;
    }

}
