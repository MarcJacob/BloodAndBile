using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Classe contenant toutes les informations relatives au match.
 * Elle est mise à jour à chaque image de MatchManager (qui lui même est mit à jour par le Client).
 * A chaque mise à jour, MatchData met à jour ses données, et envoie les messages correspondants aux joueurs connectés.
 * MatchData connait l'ID de connexion de chaque joueurs, et leur nom.
 * 
 * TODO : Créer le MatchData sur la machine host, et le faire gérer les demandes de connexion des autres clients. Quand un client est connecté
 * sur le NetworkSocket de cette machine, envoyer un message de demande d'identification qui servira de signal au client connecté qu'il est
 * bien connecté à un Match. Le client devra renvoyer un message avec son nom. Quand cela sera fait, il sera ajouté au dictionnaire des
 * joueurs connectés au Match (dans cette classe, dictionnaire de la forme int (pour l'ID de co du client) et string pour son nom), 
 * ce qui lui permettra d'être prit en compte lorsque le match enverra un message à tous les joueurs.</summary>
 */ 
public class MatchData
{

}