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
    Client Host;
    string IP;
    List<Client> Lobby;
    string Name;
    MATCH_STATE MatchState;

    public Match(Client host, string ip, string name)
    {
        Host = host;
        IP = ip;
        Name = name;
        MatchState = MATCH_STATE.OPEN_PUBLIC;
    }
}

enum MATCH_STATE
{
    OPEN_PRIVATE,
    OPEN_PUBLIC,
    CLOSED,
    ONGOING
}