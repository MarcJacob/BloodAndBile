using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/**
 * <summary> Classe contenant toutes les informations relatives au match en cours (connecté ou hébergé) </summary>
 */
public class MatchData
{

    MatchInfo Info;
    public string Password = "";
    MATCH_STATE State = MATCH_STATE.LOBBY;


    /**
     * Mise à jour du monde. Déclenchée seulement quand on est l'hôte.
     */
    public void Update()
    {
        if (MatchManager.Hosting)
        {

        }
    }

    /**
     * <summary> Mise à jour des informations de match. </summary>
     */ 
    public void UpdateMatchInfo(MatchInfo newInf)
    {
        Info = newInf;
    }

    public MatchInfo GetMatchInfo()
    {
        return Info;
    }

    /**
     * <summary> Quand le match se termine. Déclenche le callback "OnMatchEnded". </summary>
     */ 
    public void OnMatchEnd()
    {
        if (OnMatchEnded != null)
        {
            OnMatchEnded();
        }
        MatchManager.Hosting = false;
    }

    Action OnMatchEnded;
    public void RegisterOnMatchEndedCallback(Action cb)
    {
        OnMatchEnded += cb;
    }

    public void SetState(MATCH_STATE state)
    {
        State = state;
    }

    public MATCH_STATE GetState()
    {
        return State;
    }
}

public enum MATCH_STATE
{
    LOBBY,
    ONGOING,
    ENDED
}