﻿using System;
using System.Collections.Generic;
using BloodAndBileEngine.WorldState;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;

/// <summary>
/// S'occupe de mettre à jour la partie "Map" du WorldState local. Surtout utilisé lors du StateConstruction
/// - Chargement de la map en fonction de l'ID reçu
/// - Chargement des cellules du CellSystem
/// </summary>
public class MapStateUpdater : IStateUpdateReceiver
{
    public MapStateUpdater(WorldState world)
    {
        World = world;
    }

    public WorldState GetWorldState()
    {
        return World;
    }

    public void OnStateUpdate(StateUpdateMessage stateUpdate)
    {

    }

    public void OnStateConstruction(StateConstructionMessage stateConstruction)
    {
        float[] data;
        if (stateConstruction.GetStateConstructionInfo("CELL_CONSTRUCTION_DATA").Length > 0)
        {
            data = (float[])(stateConstruction.GetStateConstructionInfo("CELL_CONSTRUCTION_DATA")[0].Information); // Prend l'information du premier éléments des StateUpdateObjects de type "CELL_CONSTRUCTION_DATA" et le cast en float[].
            BuildCellSystem(data);
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("WARNING - Pas de données de construction des cellules !", UnityEngine.Color.yellow);
        }

        int mapID;
        if (stateConstruction.GetStateConstructionInfo("MAP_ID").Length > 0)
        {
            mapID = (int)(stateConstruction.GetStateConstructionInfo("MAP_ID")[0].Information); // Prend l'information du premier éléments des StateUpdateObjects de type "MAP_ID" et le cast en int.
            LoadMap(mapID);
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("WARNING - Pas d'identifiant de map !", UnityEngine.Color.yellow);
        }

        

    }

    void BuildCellSystem(float[] data)
    {
        BloodAndBileEngine.Debugger.Log("Construction des cellules...");
        World.AddData<CellSystem>(new CellSystem(data));
    }

    void LoadMap(int mapID)
    {
        BloodAndBileEngine.Debugger.Log("Chargement de la carte...");
        // TODO : Instancier le modèle de la map.
    }

    WorldState World;
}
