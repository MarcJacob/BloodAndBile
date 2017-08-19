using System;
/**
 * <summary> Message de Type 40001. Envoyé en réponse à une tentative d'authentification du client. Contient un booléen 
 * d'acceptation ou de refus, et une chaine de caractère indiquant la raison du refus. </summary>
 */

[Serializable]
public class AuthentificationResponseMessage : NetworkMessage
{
    public bool Accepted;
    public string Reason;

    public AuthentificationResponseMessage(bool accepted, string reason) : base(40001)
    {
        Accepted = accepted;
        Reason = reason;
    }
}

