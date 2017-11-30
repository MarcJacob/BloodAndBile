using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// Base de toutes les entités (= quelque chose de visible en jeu, disposant d'une position, d'une rotation, d'une taille (et hauteur ), 
/// d'une vitesse, et d'un identifiant unique)
/// </summary>
namespace BloodAndBileEngine
{
    public class Entity
    {
        public uint ID { get; private set; }
        public SerializableVector3 Position { get; set; }
        public SerializableQuaternion Rotation { get; set; }
        public float Size { get; set; }
        public float Height { get; set; }
        public bool Destroyed { get; set; }
        public int CurrentCellID { get; private set; } // Index de la cellule actuelle dans laquelle se trouve l'entité.
        // Est relatif à un CellSystem, donc il faut être sur de se trouver dans le bon match avant de relier
        // un objet Cell à cette entité avec cet attribut.

        WeakReference WorldStateWeakRef = new WeakReference(null);

        public Entity()
        {
            ID = 0;
            Position = UnityEngine.Vector3.zero;
            Rotation = UnityEngine.Quaternion.identity;
            Size = 1f;
            Height = 1f;
            Destroyed = true;
            CurrentCellID = 0;
        } // Etat par défaut des entités en mémoire.

        List<EntityComponent> Components = new List<EntityComponent>(); // Ensemble des Components possédés par cette Entité.

        /// <summary>
        /// Ajoute un Component du type spécifié à l'entité et en renvoi la référence.
        /// </summary>
        /// <typeparam name="T"> Type de component </typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public EntityComponent AddComponent(Type componentType)
        {

            object component = (EntityComponent)Activator.CreateInstance(componentType);
            if (!(component is EntityComponent))
            {
                Debugger.Log("ERREUR : Ce type n'est pas un type de EntityComponent valide !", UnityEngine.Color.red);
                return null;
            }
            EntityComponent casted = (EntityComponent)component;
            Components.Add(casted);
            casted.LinkEntity(this);
            casted.Initialise(GetWorldState());
            return casted;
        }

        /// <summary>
        /// Renvoie le Component du type indiqué s'il est possédé par cette entité.
        /// </summary>
        public EntityComponent GetComponent(Type componentType)
        {
            foreach (EntityComponent c in Components)
            {
                if (c.GetType() == componentType )
                {
                    return c;
                }
            }
            return null;
        }

        public EntityComponent[] GetComponents()
        {
            return Components.ToArray();
        }

        public void SetCellID(int id)
        {
            CurrentCellID = id;
        } // Change CurrentCellID. Appelé par un objet Cell quand celui ci perçoit
        // que cette entité est sortie de la cellule et se trouve dans une nouvelle.


        public void SetWorldState(BloodAndBileEngine.WorldState.WorldState worldState)
        {
            WorldStateWeakRef.Target = worldState;
        }

        public WorldState.WorldState GetWorldState()
        {
            return (WorldState.WorldState)(WorldStateWeakRef.Target);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns> Le nom et l'ID de 'entité </returns>
        public override string ToString()
        {
            return "Entity_" + ID;
        }

        public void Destroy()
        {
            Destroyed = true;
        }
        public void Update(float deltaTime)
        {
            foreach (EntityComponent component in Components)
            {
                component.Update(deltaTime);
            }
        }

        public static bool operator ==(Entity a, Entity b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj) // On sait que chaque entité a un ID unique par machine, donc on peut simplier la tâche de vérifier si deux référence de type Entity pointent vers le même objet.
        {
            return obj is Entity && ((Entity)obj).ID == ID;
        }
        public override int GetHashCode()
        {
            return (int)ID;
        }

        public void Reset()
        {
            Components.Clear();
        }
    }

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
    public class EntitiesManager
    {
        static Entity[] _entitiesArray; // Toutes les entités en mémoire sur cette machine.
        // à manipuler avec grand soin !
        const int MAX_ENTITY_COUNT = 20000; // Nombre max d'entités en mémoire sur cette machine.
        public static bool Initialised = false;
        public static void Initialise()
        {
            _entitiesArray = new Entity[MAX_ENTITY_COUNT];
            for (int entityID = 0; entityID < MAX_ENTITY_COUNT; entityID++)
            {
                _entitiesArray[entityID] = new Entity();
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
            if (_entitiesArray == null)
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
    }
}