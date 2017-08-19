using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Structure de données contenant un ensemble de données correspondantes à un compte Client. </summary>
 */ 
[Serializable]
public struct AccountCredentials
{
    public string Username;
    public string Password;

    public AccountCredentials(string Username, string Password)
    {
        this.Username = Username;
        this.Password = Password;
    }

    /**
     * <summary> Vérifie si ce compte est valide. </summary>
     */ 
    public bool IsValid()
    {
        List<bool> Conditions = new List<bool>();
        // Syntaxe pour ajouter une condition : Conditions.Add(<CodeQuiRenvoieUnBooléen>);
        Conditions.Add(Username.Length > 2); // Nom d'utilisateur au moins 3 caractères de long.
        Conditions.Add(Password.Length > 0);
        Conditions.Add(Password.ToLower() != Password); // Mot de passe contient au moins une majuscule.

        //_____________________________________________________________________________________

        bool isValid = true;
        int conditionID = 0;
        while (isValid && conditionID < Conditions.Count)
        {
            if (!Conditions[conditionID]) isValid = false;
            conditionID++;
        }

        return isValid;
    }
}