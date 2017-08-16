using System;
using System.Collections.Generic;

/**
 * <summary> Permet la gestion des clients. Contient un Treemap des clients connectés (triés en fonction de leur ID de connexion), un
 * Treemap des clients connus (triés en fonction de leur Username, connectés au moins une fois)... </summary>
 * 
 * SINGLETON
 */ 
public class ClientsManager
{

    //STATIC
    static ClientsManager Instance;

    static Client[] GetOnlineClients()
    {
        List<Client> clients = new List<Client>();
        foreach(Client c in Instance.OnlineClients.Values)
        {
            clients.Add(c);
        }
        return clients.ToArray();
    }

    static Client GetClientFromConnectionID(int coID)
    {
        if (Instance.OnlineClients.ContainsKey(coID))
        {
            return Instance.OnlineClients[coID];
        }
        else
            return null;
    }

    //___________________

    Dictionary<string, Client> KnownClients = new Dictionary<string, Client>(); // L'intégralité des clients connus, triés selon leur nom d'utilisateur.
    Dictionary<int, Client> OnlineClients = new Dictionary<int, Client>(); // Clients en ligne, triés selon leur ID de connexion.
    List<int> UnknownClients = new List<int>(); // Machines connectés qui n'ont pas encore répondu à la demande d'authentification.

    public ClientsManager()
    {
        if (Instance !=null)
        {
            Instance = this;
        }
    }

    /**
     * <summary> Construit la liste des clients connus (KnownClients) à partir d'une liste de fichiers. </summary>
     */ 
    public ClientsManager(string knownClientsFolderName)
    {
        LoadClientsFromFiles(knownClientsFolderName);
    }

    
    public void LoadClientsFromFiles(string folder)
    {
        // TODO : Créer un nouvel objet "ClientFileReader" capable de lire des propriétés avec un certain nom à partir des lignes d'un fichier
        // et de construire un Client à partir de ces lignes. Pour éviter de perdre tous les clients à chaque ajout / suppression de
        // propriétés, il faut que l'on puisse construire un client à partir d'un minimum de 2 informations (nom de compte, mot de passe).
        // Le reste doit être FACULTATIF.
    }

    public void RegisterUnknownClient(int coID)
    {
        UnknownClients.Add(coID);
    }

    /**
     * <summary> A partir d'une ID de connexion (qui doit se trouver dans la liste UnknownClients), et d'informations passées par
     * la machine connectée, construire un nouveau Client dans KnownClients ET OnlineClients (Inscription) ou copier un client existant 
     * de KnownClients à OnlineClients (Connexion) </summary>
     */
    public void Authentification(NetworkMessageInfo info, NetworkMessage message)
    {
        if (UnknownClients.Contains(info.ConnectionID))
        {
            // L'ID de connexion est valide. Déterminer si le client existe déjà ou s'il faut en créer un nouveau.
            AuthentificationMessage authMessage = (AuthentificationMessage)message;
            Client client = GetClientFromUsername(authMessage.Username);
            if (client != null)
            {
                // Le client existe. Le déplacer dans la liste des clients connectés.
                Debugger.Log("Test");
                if (OnlineClients.ContainsValue(client))
                {

                    if (client.GetConnectionID() == info.ConnectionID) // Cas 1 : Le client est déjà connecté avec la même ID de connexion.
                    {
                        Debugger.Log("Le client " + client.GetAccountName() + " est consistant avec l'ID de connexion " + client.GetConnectionID() + 
                            " mais était déjà considéré comme connecté !");
                        // Rien à faire.
                    }
                    else // Cas 2 : le client est déjà connecté avec une ID de connexion différente.
                    {
                        Debugger.Log("Le client " + client.GetAccountName() + " est déjà connecté sur une autre machine !");
                        OnClientAlreadyConnected(client, info.ConnectionID);
                    }
                }
                else
                {
                        if (KnownClients[client.GetAccountName()].GetPassword() == authMessage.Password) // Cas 3.1 : Le mot de passe est bon.
                        {
                            LogIn(client, info.ConnectionID);
                            SendAuthentificationResponse(info.ConnectionID, true);
                        }
                        else // Cas 3.2 : le mot de passe est mauvais.
                            SendAuthentificationResponse(info.ConnectionID, false, "Invalid password.");
                }

            }
            else // Cas 4 : le client n'existe pas
            {
                CreateNewClient(authMessage.Username, authMessage.Password, info.ConnectionID);
                SendAuthentificationResponse(info.ConnectionID, true);
            }
        }
        else
        {
            Debugger.Log("ERREUR - CLIENTMANAGER - Authentification() - l'ID de connexion ne se trouve pas dans la liste des connexions inconnues !");
        }
    }

    /**
     * <summary> Trouve un client selon un nom d'utilisateur. Renvoi null si le client n'existe pas. </summary>
     */ 
    public Client GetClientFromUsername(string Username)
    {
        if (KnownClients.ContainsKey(Username)) return KnownClients[Username]; else return null; 
        // TOUS les clients existants (même connectés) se trouvent dans ce dictionnaire.
    }

    /**
     * <summary> Exécutée quand le client est déjà connectée sur une autre machine. newCoID correspond à l'ID de connexion de la nouvelle machine.
     * Déconnecter le client déjà connecté ou refuser la connexion du nouveau client ? Pour l'instant refuser la connexion. </summary>
     */ 
    public void OnClientAlreadyConnected(Client c, int newCoID)
    {
        // Déconnecter le NetworkSocket de la nouvelle ID de connexion (newCoID).
        SendAuthentificationResponse(newCoID, false, "This client is already online on another machine !");
        NetworkSocket.Disconnect(newCoID);
    }


    /**
     * <summary> Crée un nouveau client à partir d'un nom, d'un mot de passe et d'un ID de connexion. Le place immédiatement dans
     * les clients connus et les clients connectés. </summary>
     */ 
    public void CreateNewClient(string username, string password, int coID)
    {
        Debugger.Log("Création d'un nouveau client : " + username + " " + password);
        Client c = new Client(username, password, coID);
        KnownClients.Add(username, c);
        LogIn(c, coID);
    }

    /**
     * <summary> Copie le client c de la liste des clients connus à la liste des clients connectés. </summary>
     */ 
    public void LogIn(Client c, int coID)
    {
        if (KnownClients.ContainsKey(c.GetAccountName()))
        {
            if (!OnlineClients.ContainsKey(c.GetConnectionID()))
            {
                c.SetConnectionID(coID); // Remplacement de l'ID de connexion actuelle du client par celle passée en paramètre.
                OnlineClients.Add(c.GetConnectionID(), c);
                Debugger.Log("Client " + c.GetAccountName() + " est connecté.");
            }
            else
            {
                Debugger.Log("Ce client est déjà connecté !");
            }
        } 
    }

    /**
     * <summary> Retire le client c de la liste des clients connectés. </summary>
     */ 
    public void LogOff(int coID)
    {
        if (OnlineClients.ContainsKey(coID))
        {
            Client c = OnlineClients[coID];
            if (!KnownClients.ContainsKey(c.GetAccountName())) // On vérifie pour des raisons de sureté que le client existe parmis les clients connus.
            {
                Debugger.Log("Le client déconnecté n'existe pas parmis les clients connus !");
            }

            // On retire le client de la liste des clients connectés
            OnlineClients.Remove(c.GetConnectionID());
            c.SetConnectionID(-1); // Mise de l'ID de connexion du client à -1 pour indiquer qu'il est déconnecté.
            Debugger.Log("Client " + c.GetAccountName() + " est déconnecté.");
        }
        else
            Debugger.Log("ERREUR - CLIENTSMANAGER - LogOff() - Ce client n'est pas connecté ou n'existe pas !");
    }


    /**
     * <summary> Envoi un message de type "AuthentificationResponseMessage" à l'ID de connexion indiquée, avec le booléen d'acceptance,
     * et la raison si ce dernier est faux. </summary>
     */ 
    void SendAuthentificationResponse(int coID, bool accepted, string reason = "")
    {
        MessageSender.Send(new AuthentificationResponseMessage(accepted, reason), coID, 0);
    }
}
