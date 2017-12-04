using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloodAndBileEngine;

/// <summary>
/// Le EntityRenderer gère l'affichage des entités à l'écran, en gérant des GameObject disposants du
/// component "Actor".
/// </summary>
public static class EntityRenderer
{
    static public void OnEntityCreation(uint EntityID)
    {
        Entity e = EntitiesManager.GetEntityFromID(EntityID);
        if (e == null)
        {
            Debugger.Log("ERREUR : Cet ID d'entité n'existe pas, ou alors la mémoire des entités n'a pas été initialisée !", UnityEngine.Color.red);
            return;
        }
        else if (!e.Destroyed)
        {
            Debugger.Log("ERREUR : Cet ID d'entité est déjà occupé !", UnityEngine.Color.red);
            return;
        }

        // Création du GameObject

        UnityEngine.GameObject entityGO = UnityEngine.GameObject.Instantiate<UnityEngine.GameObject>((UnityEngine.GameObject)UnityEngine.Resources.Load("Prefabs/EntityPrefab"));
        // Ajout du component Actor & assignation de l'entité

        entityGO.AddComponent<Actor>().SetEntityID(EntityID);

        // Terminé.
    }
}
