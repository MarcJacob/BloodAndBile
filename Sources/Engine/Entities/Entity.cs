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


        bool IDSet = false;
        public void SetID(uint id)
        {
            if (!IDSet)
            {
                ID = id;
                IDSet = true;
            }
            else
            {
                Debugger.Log("ERREUR : Impossible d'assigner l'ID d'une même entité plus d'une fois !", UnityEngine.Color.red);
            }
        }
    }
}