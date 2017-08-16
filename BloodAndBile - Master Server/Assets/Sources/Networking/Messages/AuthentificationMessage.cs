using System;
using System.Collections.Generic;

/**
 * <summary> Message envoyé au Master Server par le client pour tenter de s'authentifier. </summary>
 */ 
 [Serializable]
public class AuthentificationMessage : NetworkMessage
{
    public string Username;
    public string Password;

    public AuthentificationMessage(string Username, string Password) : base(0) // Message de type 0.
    {
        this.Username = Username;
        this.Password = Password;
    }
}
