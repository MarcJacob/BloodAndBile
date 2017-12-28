using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    public class SetEntityPositionSecurityLayer : INetworkCommandSecurityLayer
    {

        public bool CanCheckCommand(string command)
        {
            return (command == "SetEntityPosition");
        }

        public bool DoSecurityCheck(string command, object[] args)
        {
            if (args.Length < 4)
            {
                Debugger.Log("ERREUR : Pas assez de paramètres dans l'appel de la commande SetEntityPosition !", UnityEngine.Color.red);
                return false;
            }

            uint ID;
            float posX;
            float posY;
            float posZ;
            
            // On vérifie que chaque argument soit du bon type. S'ils ne le sont pas, on les converti.
            if (args[0] is int)
            {
                ID = (uint)args[0];
            }
            else
            {
                if (!(args[0] is string) || !uint.TryParse((string)args[0], out ID))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - L'ID de l'entité doit être un entier ou une chaine de caractères !");
                    return false;
                }
            }

            float maxDistance = EntitiesManager.GetEntityFromID(ID).GetComponent<EntityMover>().GetCurrentSpeed();

            if (args[1] is float)
            {
                posX = (int)args[1];
            }
            else
            {
                if (!(args[1] is string) || !float.TryParse((string)args[1], out posX))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - La position X de l'entité doit être un entier ou une chaine de caractères !");
                    return false;
                }
            }


            if (args[2] is float)
            {
                posY = (int)args[2];
            }
            else
            {
                if (!(args[2] is string) || !float.TryParse((string)args[2], out posY))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - La position Y de l'entité doit être un entier ou une chaine de caractères !");
                    return false;
                }
            }

            if (args[3] is float)
            {
                posZ = (int)args[3];
            }
            else
            {
                if (!(args[3] is string) || !float.TryParse((string)args[3], out posZ))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - La position Z de l'entité doit être un entier ou une chaine de caractères !");
                    return false;
                }
            }
            Entity e = EntitiesManager.GetEntityFromID(ID);
            if ((e.Position - new UnityEngine.Vector3(posX, posY, posZ)).sqrMagnitude > maxDistance*maxDistance)
                return false;
            else
                return true;
        }
    }
}
