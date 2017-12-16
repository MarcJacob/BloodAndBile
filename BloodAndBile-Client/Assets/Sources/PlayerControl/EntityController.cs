using BloodAndBileEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe s'occupant de donner le contrôle d'une entité au joueur local.
/// Utilise des commandes "PlayerControl" pour informer le WorldState local ou en ligne de ce qu'il fait et de l'état
/// actuel de l'entité contrôlée qu'il "propose".
/// </summary>
public class EntityController : MonoBehaviour
{

    Vector3 CameraOffset = new Vector3(0, 1, -2); // Offset de la caméra par rapport à l'entité.
    float WorldstateEntityUpdateFrequency = 10f;
    Actor ControlledActor;
    bool SpellCasted = false;
    object Target = null;

    private void Start()
    {
        // Rendre la caméra principale enfant de gameObject.
        Camera.main.transform.parent = gameObject.transform;
        // Récupération de l'actor.
        ControlledActor = GetComponent<Actor>();
        ControlledActor.SetTrackPosition(false);
    }

    float CurrentWorldStateEntityUpdateTimer = 0f;

    private void Update()
    {
        CurrentWorldStateEntityUpdateTimer += Time.deltaTime;
        if (CurrentWorldStateEntityUpdateTimer >= 1f / WorldstateEntityUpdateFrequency)
        {
            CurrentWorldStateEntityUpdateTimer = 0f;
            OnWorldstateUpdate();
        }

        UpdateCameraPosition();

        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime, 0f, Input.GetAxis("Vertical") * Time.deltaTime);
        transform.Rotate(0f, Input.GetAxis("Mouse X"), 0f);
        Camera.main.transform.Rotate(-Input.GetAxis("Mouse Y"), 0f, 0f);
        Dictionary<uint, KeyCode> SpellKeys = ((BloodAndBileEngine.SpellComponent)ControlledActor.GetControlledEntity().GetComponent(typeof(BloodAndBileEngine.SpellComponent))).GetSpellKeyCodes();
        foreach (uint sId in SpellKeys.Keys)
        {
            if(Input.GetKeyDown(SpellKeys[sId]))
            {
                ((BloodAndBileEngine.SpellComponent)ControlledActor.GetControlledEntity().GetComponent(typeof(BloodAndBileEngine.SpellComponent))).SetSelectedSpellId(sId);
                BloodAndBileEngine.Debugger.Log("Sort sélectionné : " + BloodAndBileEngine.SpellsManager.GetSpellByID(sId).Name, Color.magenta);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Debugger.Log("Ca clique !");
            SpellCasted = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debugger.Log("Ca Raycast !");
                if (hit.collider.gameObject.GetComponent<Actor>() != null && hit.collider.gameObject.GetComponent<Actor>().GetControlledEntity() != null)
                {
                    Target = hit.collider.gameObject.GetComponent<Actor>().GetControlledEntity().ID;
                }
                else
                {
                    Target = new SerializableVector3(hit.point.x, hit.point.y, hit.point.z);
                }
                
            }

        }
            
    }

    /// <summary>
    /// Met à jour la position de la caméra.
    /// TODO : passer ça dans un script à part de gestion de caméra.
    /// </summary>
    void UpdateCameraPosition()
    {
        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, CameraOffset, Time.deltaTime * 2);
    }


    /// <summary>
    /// Exécuté régulièrement pour mettre à jour le WorldState local ou en ligne avec des informations
    /// importantes (position, rotation...).
    /// </summary>
    void OnWorldstateUpdate()
    {
        // Mise à jour de la position
        SendPlayerControlCommand("SetEntityPosition", ControlledActor.GetControlledEntity().ID, transform.position.x, transform.position.y, transform.position.z);
        SendPlayerControlCommand("SetEntityRotation", ControlledActor.GetControlledEntity().ID, transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        if(SpellCasted)
        {
            uint spellID = ((SpellComponent)ControlledActor.GetControlledEntity().GetComponent(typeof(SpellComponent))).SelectedSpellId;

            if (SpellsManager.GetSpellByID(spellID).Flags.HasFlag(SpellFlags.TARGETS_POSITION))
                SendPlayerControlCommand("CastSpell", ControlledActor.GetControlledEntity().ID, spellID, 'p', ((SerializableVector3)Target).x, ((SerializableVector3)Target).y, ((SerializableVector3)Target).z);
            else if (SpellsManager.GetSpellByID(spellID).Flags.HasFlag(SpellFlags.TARGETS_ENTITY))
                SendPlayerControlCommand("CastSpell", ControlledActor.GetControlledEntity().ID, spellID, 'e', (uint)Target);
            else if(SpellsManager.GetSpellByID(spellID).Flags.HasFlag(SpellFlags.TARGETS_SELF))
                SendPlayerControlCommand("CastSpell", ControlledActor.GetControlledEntity().ID, spellID, 's');
            SpellCasted = false;
            Target = null;
        }
    }

    void SendPlayerControlCommand(string command, params object[] args)
    {
        object[] newArgs = new object[args.Length + 1];
        newArgs[0] = command;
        for(int i = 1; i < newArgs.Length; i++)
        {
            newArgs[i] = args[i - 1];
        }

        BloodAndBileEngine.InputManager.SendCommand("PlayerControl", newArgs);
    }
}
