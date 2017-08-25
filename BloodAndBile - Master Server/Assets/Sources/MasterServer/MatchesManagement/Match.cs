using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Un Match est une structure de donnée indiquant des informations importantes sur un Match : l'Hôte actuel, l'ensemble des
 * joueurs connectés, l'IP, et le nom du lobby. </summary>
 */ 
public class Match
{
    public Client Host;
    public MatchInfo Info;

    public MATCH_STATE MatchState;

    public Match(Client host, string ip, int Port, string name, bool Locked = false)
    {
        Host = host;
        Info.HostIP = ip;
        Info.MatchName = name;
        Info.Locked = Locked;
        Info.HostPort = Port;
        Info.HostName = Host.GetAccountName();
        MatchState = MATCH_STATE.OPEN_PUBLIC;
    }

    public void Update()
    {
        // Si l'hôte est déconnecté, alors le match se termine (TODO : gérer les fins de match).
        if (!Host.Connected())
        {
            MatchState = MATCH_STATE.ENDED;
        }
    }

    public bool IsOver()
    {
        return MatchState == MATCH_STATE.ENDED;
    }
}

public enum MATCH_STATE
{
    OPEN_PRIVATE,
    OPEN_PUBLIC,
    CLOSED,
    ONGOING,
    ENDED
}