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

    string AccountName;
    string Password;
    int ConnectionID = -1; // -1 signifie que le client n'est pas connecté.

    public Client(string name, string password, int coID)
    {
        AccountName = name;
        Password = password;
        ConnectionID = coID;
    }

    /**
     * <summary> Détermine si un Client correspond au nom ou client passé. Cela suffit car deux Clients avec le même nom ne peuvent exister. </summary>
     */ 
    public override bool Equals(object obj)
    {
        if (obj == null || (GetType() != obj.GetType() && obj.GetType() != typeof(string)))
        {
            return false;
        }

        if (obj.GetType() == typeof(string))
        {
            string s = (string)obj;
            return AccountName == s;
        }
        else
        {

            Client c = (Client)obj;

            return AccountName == c.AccountName;
        }
    }

    public string GetAccountName()
    {
        return AccountName;
    }

    public string GetPassword()
    {
        return Password;
    }

    public int GetConnectionID()
    {
        return ConnectionID;
    }

    public void SetConnectionID(int id)
    {
        ConnectionID = id;
    }

}