using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un Actor est un component de gameobject permettant à celui ci d'être "lié" à une entité et de répondre à certains
/// évènements déclencheurs provenants de celle ci (exemple : mort, dégats pris, déplacement...).
/// 
/// L'Actor lui même maintient le lien en disposant d'une référence directe vers l'entité dans le World local,
/// qui est mit à jour par le client ou par des messages StateUpdate du MatchServer.
/// 
/// Contient également des informations supplémentaires que l'Actor détermine par lui même.
/// Exemple : direction de déplacement. L'entitée locale se contente de mettre sa position à jour. En revanche, avant
/// cela, elle envoi son ancienne et sa nouvelle position à l'acteur à travers un callback, ce qui permet à ce dernier
/// de déterminer dans quelle direction l'entité s'est déplacée. Cela peut être utile pour déterminer l'animation
/// à jouer.
/// </summary>
public class Actor : MonoBehaviour
{
    // Entity LinkedEntity; // Entity liée à cet Actor.

    string ActorName; // Nom de l'acteur.

    Animator AnimationController; // Contrôleur d'animations de ce GameObject.

    private void Update()
    {
        
    }

    void PollEvents()
    {

    }

    // Lance une mise à jour de l'acteur par l'entité liée.
    void UpdateFromLinkedEntity()
    {

    }

    void OnEntityDeath()
    {
        AnimationController.Play("Death");
    }

    /// <summary>
    /// Déclenchée lorsque l'entité change de position.
    /// </summary>
    /// <param name="previousPosition"> Position précédente </param>
    /// <param name="newPosition"> Nouvelle position </param>
    void OnEntityMoving(Vector3 previousPosition, Vector3 newPosition)
    {
        // TODO : jouer une animation différente en fonction de l'orientation de l'entitée et de la direction de déplacement.
    }
}