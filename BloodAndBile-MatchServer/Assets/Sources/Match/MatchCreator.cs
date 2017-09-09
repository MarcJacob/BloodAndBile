using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Classe "Factory" qui se charge de la création d'un match. </summary>
 */ 
public static class MatchCreator
{
    public static Match CreateMatch(int[] players)
    {
        return new Match();
    }
}
