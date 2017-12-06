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
/// Les évènements déclencheurs sont transmis à travers la commande d'input "ExecuteActorEvent <IDEntitée> <NomEvenement>".
/// Celle ci signale TOUS les Actor qu'un évènement de ce nom est lancé. Tous les acteurs liés à l'entité d'ID IDEntitée y réagiront.
/// </summary>
public class Actor : MonoBehaviour
{
    uint LinkedEntityID;
    bool EntityIDInitialized = false;
    public string ActorName; // Nom de l'acteur.

    Animator AnimationController; // Contrôleur d'animations de ce GameObject.

    bool Dying = false;
    bool TrackPosition = true; // Cet actor doit-il constamment suivre la position de l'entité ou seulement
    //  en cas de trop grande différence de position ? False notamment lorsque l'Actor est contrôlé par le joueur.

    public void SetTrackPosition(bool track)
    {
        TrackPosition = track;
    }

    private void Start()
    {
        BloodAndBileEngine.InputManager.AddHandler("ExecuteActorEvent", OnActorEvent);
    }

    private void Update()
    {
        if (EntityIDInitialized)
        {
            BloodAndBileEngine.Entity entity = GetControlledEntity();
            if (TrackPosition)
            {
                // "Lerper" constamment vers la position de l'entité.
                transform.position = Vector3.Lerp(transform.position, entity.Position, Time.deltaTime / 2);
            }

            if ((transform.position - entity.Position).sqrMagnitude > 25)
            {
                transform.position = entity.Position;
            }
        }
    }

    public void SetEntityID(uint id)
    {
        LinkedEntityID = id;
        EntityIDInitialized = true;
    }

    public BloodAndBileEngine.Entity GetControlledEntity()
    {
        return BloodAndBileEngine.EntitiesManager.GetEntityFromID(LinkedEntityID);
    }
    void OnActorEvent(object[] args)
    {
        if (args.Length < 2)
        {
            BloodAndBileEngine.Debugger.Log("ERREUR - Pas assez d'arguments pour la commande d'évènement d'acteur.", Color.red);
            return;
        }
        uint entityID;
        if (args[0] is uint)
            entityID = (uint)args[0];
        else
            uint.TryParse((string)args[0], out entityID);
        string eventName = (string)args[1];

        if (entityID == GetControlledEntity().ID)
        {
            ReactToEvent(eventName);
        }
    }

    // Ajouter ici les différentes "réactions" aux évènements.
    void ReactToEvent(string eventName)
    {
        switch(eventName)
        {
            case ("Death"):
                OnEntityDeath();
                break;

        }
    }

    void OnEntityDeath()
    {
        AnimationController.Play("Death");

        Dying = true;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        BloodAndBileEngine.InputManager.RemoveHandler("ExecuteActorEvent", OnActorEvent);
    }
}