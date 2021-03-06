﻿using System;
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
    bool TrackRotation = true;
    public void SetTrackPosition(bool track)
    {
        TrackPosition = track;
    }
    public void SetTrackRotation(bool track)
    {
        TrackRotation = track;
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
            Vector3 xzPosition = transform.position;
            xzPosition.y = 0;
            Vector3 xzEntityPosition = GetControlledEntity().Position;
            xzEntityPosition.y = 0;
            float xzSquareDist = (xzPosition - xzEntityPosition).sqrMagnitude;
            if (TrackPosition)
            {
                // "Lerper" constamment vers la position de l'entité.
                if ((transform.position - entity.Position).sqrMagnitude < 1)
                {
                    transform.position = Vector3.Lerp(transform.position, entity.Position, Time.deltaTime * 4);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, entity.Position, Time.deltaTime / 2);
                }
            }
            else if (xzSquareDist > 16)
            {
                transform.position = entity.Position;
            }

            if (TrackRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, entity.Rotation, Time.deltaTime * 6);
            }

            // Toujours appliquer la bonne hauteur à la position de l'entité
            float height = GetControlledEntity().GetWorldState().GetData<BloodAndBileEngine.WorldState.CellSystem>().GetCellFromPosition(transform.position.z, transform.position.x).GetHeightFrom2DCoordinates(transform.position.z, transform.position.x);
            transform.Translate(0f, height - transform.position.y, 0f);
        }

        if (GetControlledEntity().Destroyed)
        {
            ReactToEvent("Death");
            Die();
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
        if (AnimationController != null)
        AnimationController.Play("Death");

        Dying = true;
    }

    void Die()
    {
        if (GetComponent<EntityController>() != null)
        {
            GetComponent<EntityController>().OnEntityDeath();
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        BloodAndBileEngine.InputManager.RemoveHandler("ExecuteActorEvent", OnActorEvent);
    }
}