using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



/// <summary>
/// Le EntitiesManager permet un accès rapide à n'importe quelle entité à partir de son
/// identifiant : l'identifiant d'une entité correspond à son index dans le tableau des
/// entités.
/// 
/// Cela permet également d'optimiser la création et la destruction des entités
/// en limitant le nombre d'appels système : toutes les entité "possibles" existeront
/// constamment en mémoire, une entité "détruite" pouvant se faire remplacer par une autre.
/// 
/// TODO : Pour la synchronisation des entités, il est pour l'instant nécessaire que
/// la même capacité mémoire soit allouée sur TOUTES les machines (Client ou Serveur).
/// Retirer cette contrainte en créant un système de table liant un identifiant "Serveur"
/// à un identifiant d'entité local.
/// Ainsi, l'entité 152355 sur le MatchServer pourrait correspondre à l'entité 10 sur le client.
/// </summary>
namespace BloodAndBileEngine
{
    public static partial class EntitiesManager
    {
        static Entity[] _entitiesArray; // Toutes les entités en mémoire sur cette machine.
                                        // à manipuler avec grand soin !
        const int MAX_ENTITY_COUNT = 20000; // Nombre max d'entités en mémoire sur cette machine.
        public static bool Initialised = false;
        public static void Initialise()
        {
            _entitiesArray = new Entity[MAX_ENTITY_COUNT];
            for (uint entityID = 0; entityID < MAX_ENTITY_COUNT; entityID++)
            {
                _entitiesArray[entityID] = new Entity();
                _entitiesArray[entityID].SetID(entityID);
            }
            InitialiseCommands();
        }

        /// <summary>
        /// Détruit toutes les entités.
        /// </summary>
        public static void Clear()
        {
            foreach (Entity e in _entitiesArray)
            {
                e.Destroy();
            }
        }

        public static int GetEntityCount()
        {
            int amount = 0;
            foreach (Entity entity in _entitiesArray)
            {
                if (!entity.Destroyed)
                {
                    amount++;
                }
            }
            return amount;
        }

        public static Entity GetEntityFromID(uint ID)
        {
            if (_entitiesArray == null || ID > _entitiesArray.Length)
            {
                return null;
            }
            else
            {
                return _entitiesArray[ID];
            }
        }

        public static uint GetNextID() // Cherche l'ID disponible la plus proche de 0.
        {
            uint ID = 0;
            while (!_entitiesArray[ID].Destroyed)
            {
                ID++;
            }

            return ID;
        }

        static void InitialiseCommands()
        {
            InputManager.AddHandler("SetEntityPosition", SetEntityPosition);
            InputManager.AddHandler("SetEntityRotation", SetEntityRotation);
            InputManager.AddHandler("KillEntity", KillEntity);
            
        }

        #region Commandes d'entité

        static void SetEntityPosition(object[] args)
        {
            if (args.Length < 4)
            {
                Debugger.Log("ERREUR : Pas assez de paramètres dans l'appel de la commande SetEntityPosition !", UnityEngine.Color.red);
                return;
            }

            uint ID;
            float posX;
            float posY;
            float posZ;

            // On vérifie que chaque argument soit du bon type. S'ils ne le sont pas, on les converti.
            if (args[0] is uint)
            {
                ID = (uint)args[0];
            }
            else
            {
                if (!(args[0] is string) || !uint.TryParse((string)args[0], out ID))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - L'ID de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            if (args[1] is float)
            {
                posX = (float)args[1];
            }
            else
            {
                if (!(args[1] is string) || !float.TryParse((string)args[1], out posX))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - La position X de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }


            if (args[2] is float)
            {
                posY = (float)args[2];
            }
            else
            {
                if (!(args[2] is string) || !float.TryParse((string)args[2], out posY))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - La position Y de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            if (args[3] is float)
            {
                posZ = (float)args[3];
            }
            else
            {
                if (!(args[3] is string) || !float.TryParse((string)args[3], out posZ))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - La position Z de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            // Tout est bon ! La position de l'entité peut être changée. Si l'entité est détruite, affichage d'un warning.

            Entity e = GetEntityFromID(ID);
            if (e == null || e.Destroyed)
            {
                Debugger.Log("WARNING : Tentative de modifier la position de l'entité " + ID + " qui n'existe pas ou est détruite !", UnityEngine.Color.yellow);
            }

            if (e != null)
            {
                SerializableVector3 newPos = new SerializableVector3(posX, posY, posZ);
                if (e.GetComponent(typeof(EntityMover)) != null)
                {
                    e.GetComponent<EntityMover>().Teleport(newPos.z, newPos.x);
                }
                else
                    e.Position = newPos;
            }
        }

        static void SetEntityRotation(object[] args)
        {
            if (args.Length < 5)
            {
                Debugger.Log("ERREUR : Pas assez de paramètres dans l'appel de la commande SetEntityRotation !", UnityEngine.Color.red);
                return;
            }

            uint ID;
            float rotX = 0f;
            float rotY = 0f;
            float rotZ = 0f;
            float rotW = 0f;

            // On vérifie que chaque argument soit du bon type. S'ils ne le sont pas, on les converti.
            if (args[0] is uint)
            {
                ID = (uint)args[0];
            }
            else
            {
                if (!(args[0] is string) || !uint.TryParse((string)args[0], out ID))
                {
                    Debugger.Log("ERREUR : Commande SetEntityPosition - L'ID de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            if (args[1] is float)
            {
                rotX = (float)args[1];
            }
            else
            {
                if (!(args[1] is string) || !float.TryParse((string)args[1], out rotX))
                {
                    Debugger.Log("ERREUR : Commande SetEntityRotation - La rotation X de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }


            if (args[2] is float)
            {
                rotY = (float)args[2];
            }
            else
            {
                if (!(args[2] is string) || !float.TryParse((string)args[2], out rotY))
                {
                    Debugger.Log("ERREUR : Commande SetEntityRotation - La rotation Y de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            if (args[3] is float)
            {
                rotZ = (float)args[3];
            }
            else
            {
                if (!(args[3] is string) || !float.TryParse((string)args[3], out rotZ))
                {
                    Debugger.Log("ERREUR : Commande SetEntityRotation - La rotation Z de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            if (args[4] is float)
            {
                rotW = (float)args[4];
            }
            else
            {
                if (!(args[4] is string) || !float.TryParse((string)args[4], out rotW))
                {
                    Debugger.Log("ERREUR : Commande SetEntityRotation - La rotation W de l'entité doit être un entier ou une chaine de caractères !");
                    return;
                }
            }

            // Tout est bon ! La position de l'entité peut être changée. Si l'entité est détruite, affichage d'un warning.

            Entity e = GetEntityFromID(ID);
            if (e == null || e.Destroyed)
            {
                Debugger.Log("WARNING : Tentative de modifier la position de l'entité " + ID + " qui n'existe pas ou est détruite !", UnityEngine.Color.yellow);
            }

            if (e != null)
            {
                SerializableQuaternion newRot = new SerializableQuaternion(rotX, rotY, rotZ, rotW);
                e.Rotation = newRot;
            }
        }

        static void KillEntity(object[] args)
        {
            if (args.Length == 0)
            {
                Debugger.Log("ERREUR : KillEntity nécessite un argument : l'ID de l'entité.", UnityEngine.Color.red);
                return;
            }

            if (args[0] is uint)
            {
                Entity e = GetEntityFromID((uint)args[0]);
                if (e != null)
                {
                    e.Destroy();
                    return;
                }
            }
            else if (args[0] is string)
            {
                uint id;
                if (uint.TryParse((string)args[0], out id))
                {
                    Entity e = GetEntityFromID(id);
                    if (e != null)
                    {
                        e.Destroy();
                        return;
                    }
                }
                else
                {
                    Debugger.Log("ERREUR : KillEntity - la chaine de caractère représentant l'ID de l'entité doit être un nombre !", UnityEngine.Color.red);
                    return;
                }
            }
            else
            {
                Debugger.Log("ERREUR : le type du premier argument de KillEntity doit être string ou uint !", UnityEngine.Color.red);
                return;
            }

            Debugger.Log("ERREUR : ID de l'entité invalide !", UnityEngine.Color.red);
        }
        

        #endregion
    }
}