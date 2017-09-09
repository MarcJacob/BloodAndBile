using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Contient des informations relatives à un client : son nom, son mot de passe, son ID de connexion (-1 si hors ligne). </summary>
 */ 
public class ClientAccountInfo
{

    string Username; // Nom du client.
    string Password; // Mot de passe du client.
    int ConnectionID; // ID de connexion. -1 si le client est hors-ligne.

    public ClientAccountInfo(string name, string pass, int coID)
    {
        Username = name;
        Password = pass;
        ConnectionID = coID;
    }

    public void Login(int coID)
    {
        if (coID >= 0)
        {
            ConnectionID = coID;
        }
    }

    public void Logoff()
    {
        ConnectionID = -1;
    }

    public bool IsOnline()
    {
        return ConnectionID >= 0;
    }

    public int GetConnectionID()
    {
        return ConnectionID;
    }

    public string GetUsername()
    {
        return Username;
    }

    public string GetPassword()
    {
        return Password;
    }


}