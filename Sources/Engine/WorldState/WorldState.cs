using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Un WorldState encapsule toutes les informations relatives à une instance de gameplay Blood & Bile, que ce soit
/// un match multijoueur, une mission solo...
/// 
/// Ce qu'il contient est, de son côté, abstrait. WorldState contient purement de l'information et ne met pas lui
/// même à jour ses informations. En revanche, il est possible de lui demander de "Simuler" ce qui doit se passer avec
/// le temps, notamment en mettant à jour les contrôleurs d'entité (cela est un exemple, WorldState n'est même pas au courant
/// de l'existence d'"entités" ou de "contrôleurs". 
/// </summary>
namespace BloodAndBileEngine.WorldState
{
    public class WorldState
    {
        List<IWorldStateData> Data; // Données du WorldState.

        public void Simulate(float deltaTime)
        {
            foreach(IWorldStateData data in Data)
            {
                data.Simulate(deltaTime);
            }
        } // Lance une simulation généralisée pour le temps donné.

        public T AddData<T>(T newData) where T : IWorldStateData
        {
            bool alreadyOwned = false;
            foreach(IWorldStateData data in Data)
            {
                if (data is T)
                {
                    alreadyOwned = true;
                }
            }

            if (alreadyOwned)
            {
                Debugger.Log("ERREUR - Ce WorldStateData est déjà présent dans ce WorldState !", UnityEngine.Color.red);
                return default(T);
            }
            else
            {
                Data.Add(newData);
                return newData;
            }
        } // Ajoute une IWorldStateData à la liste Data. Si Data contient déjà
        // un IWorldStateData du même type, alors l'opération est refusée.
        // De plus, si l'ajout est un succès, alors une référence vers l'élément ajouté est renvoyée.

        public T GetData<T>() where T : IWorldStateData
        {
            foreach(IWorldStateData data in Data)
            {
                if (data is T)
                {
                    return (T)data;
                }
            }

            Debugger.Log("Erreur - ce WorldState ne possède pas un IWorldStateData de ce type !", UnityEngine.Color.red);
            return default(T);
        } // Renvoi une référence vers le IWorldStateData demandé. Si un IWorldStateData du type demandé
        // n'est pas présent dans la liste Data, alors la valeur par défaut (généralement "null") est renvoyée.

        public WorldState()
        {
            Data = new List<IWorldStateData>();
        } // Constructeur par défaut. Initialise un WorldState vide.

        public WorldState(IWorldStateData[] data)
        {
            Data = new List<IWorldStateData>();
            foreach(IWorldStateData d in data)
            {
                Data.Add(d);
            }
        }

    }
}
