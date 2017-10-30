using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Base de toutes les entités (= quelque chose de visible en jeu, disposant d'une position, d'une rotation, d'une taille (et hauteur ), 
/// d'une vitesse, et d'un identifiant unique)
/// </summary>
namespace BloodAndBileEngine
{
    public class Entity
    {
        public uint ID { get; private set; }
        protected static uint LastID = 0; // Les ID d'entité sont distribuées au niveau machine ! Pour un MatchServer, il n'y aura qu'une seule
        // entité 0 pour tous les matchs, par exemple.
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Size { get; set; }
        public float Height { get; set; }
        public bool Destroyed { get; private set; }
        public int CurrentCellID { get; private set; } // Index de la cellule actuelle dans laquelle se trouve l'entité.
        // Est relatif à un CellSystem, donc il faut être sur de se trouver dans le bon match avant de relier
        // un objet Cell à cette entité avec cet attribut.

        List<EntityComponent> Components = new List<EntityComponent>(); // Ensemble des Components possédés par cette Entité.

        /// <summary>
        /// Ajoute un Component du type spécifié à l'entité et en renvoi la référence.
        /// </summary>
        /// <typeparam name="T"> Type de component </typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public T AddComponent<T>(T component) where T : EntityComponent
        {
            Components.Add(component);
            component.Initialise();
            return component;
        }

        /// <summary>
        /// Renvoie le Component du type indiqué s'il est possédé par cette entité.
        /// </summary>
        /// <typeparam name="T"> Type de component </typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : EntityComponent
        {
            foreach(EntityComponent c in Components)
            {
                if (c is T)
                {
                    return (T)c;
                }
            }
            return null;
        }

        public EntityComponent[] GetComponents()
        {
            return Components.ToArray();
        }

        public Entity(Vector3 pos, Quaternion rot, float size, float height)
        {
            ID = LastID++;
            Position = pos;
            Rotation = rot;
            Size = size;
            Height = height;
            Destroyed = false;
            CurrentCellID = 0;
        }

        public void SetCellID(int id)
        {
            CurrentCellID = id;
        } // Change CurrentCellID. Appelé par un objet Cell quand celui ci perçoit
        // que cette entité est sortie de la cellule et se trouve dans une nouvelle.

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
            foreach(EntityComponent component in Components)
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
    }
}