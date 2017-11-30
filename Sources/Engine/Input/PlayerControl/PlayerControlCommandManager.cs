using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Gère les commandes de type "PlayerControl".
/// N'est pas une classe statique car son comportement dépend
/// de la machine : si c'est un MatchServer, ou si c'est un client
/// entrain de jouer en solo, les commandes PlayerControl sont exécutées
/// localement. Si c'est un client jouant en ligne, la commande est envoyé
/// sur le réseau (ajout du préfixe NetCommand avec l'ID de connexion du MatchServer).
/// 
/// La classe elle même contient quelques fonctions "prédéfinies" mais
/// il est toujours possible de définir un comportement personnalisé.
/// 
/// L'intérêt des commandes PlayerControl est de pouvoir faire passer ces commandes à travers un filtre
/// sans avoir à modifier le comportement du code appelant ces commandes : filtre envoi au serveur / exécution locale, filtre sécurité...
/// </summary>
namespace BloodAndBileEngine
{
    public class PlayerControlCommandManager
    {
        Action<object[]> PlayerControlHandler; // Handler input appelé lorsqu'une commande PlayerControl est reçue.

        /// <summary>
        /// Remplace le handler actuel (s'il existe) par le nouveau.
        /// </summary>
        /// <param name="handler"></param>
        public void SetHandler(Action<object[]> handler)
        {
            ClearHandler();
            PlayerControlHandler = handler;
            InputManager.AddHandler("PlayerControl", handler);
        }

        /// <summary>
        /// Supprime le handler actuel.
        /// </summary>
        public void ClearHandler()
        {
            if (PlayerControlHandler != null)
            {
                InputManager.RemoveHandler("PlayerControl", PlayerControlHandler);
                PlayerControlHandler = null;
            }
        }


        #region Handlers Prédéfinis

        /// <summary>
        /// Handler redirigeant les messages PlayerControl sur le réseau
        /// vers l'ID de connexion spécifiée en ajoutant les préfixes
        /// "NetCommand connectionID".
        /// On remet "PlayerControl" car on part du principe que
        /// le serveur sait gérer les commandes PlayerControl.
        /// </summary>
        public void SetSendToNetworkHandler(int connectionID)
        {
            SetHandler((parameters) =>
            {
                // On va ajouter 2 objet aux paramètres, dans l'ordre : l'ID de connexion et "PlayerControl".
                object[] newParameters = new object[parameters.Length + 2];
                newParameters[0] = connectionID;
                newParameters[1] = "PlayerControl";
                for (int i = 2; i < newParameters.Length; i++)
                {
                    newParameters[i] = parameters[i - 2];
                }

                // Puis on renvoit la commande au système d'Input avec le préfixe "NetCommand".
                InputManager.SendCommand("NetCommand", newParameters);
            });
        }

        /// <summary>
        /// Handler ne faisant qu'enlever le préfixe "PlayerControl" à la commande
        /// et la renvoyer dans le système d'inputs.
        /// </summary>
        public void SetExecuteLocally()
        {
            SetHandler((parameters) =>
            {
                string commandName = (string)parameters[0];
                object[] newParameters = new object[parameters.Length - 1];
                for (int i = 1; i < newParameters.Length + 1; i++)
                {
                    newParameters[i-1] = parameters[i];
                }
                InputManager.SendCommand(commandName, newParameters);
            });
        }

        #endregion

        ~PlayerControlCommandManager()
        {
            ClearHandler();
        }
    }
}
