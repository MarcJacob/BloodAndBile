using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Un Client sur le Master Server est un ensemble de données concernant un client (nom, mot de passe, s'il est en ligne...)
 * associé à son ID de connexion actuelle s'il est connecté. </summary>
 */ 
public class Client
{
    static List<Client> ClientList; // L'ensemble des clients (En ligne ou Hors ligne).

    string AccountName;
    string Password;
    int ConnectionID;

}