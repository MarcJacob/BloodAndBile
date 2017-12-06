using System;
using System.Collections.Generic;
using BloodAndBileEngine.WorldState;
using BloodAndBileEngine.Networking.Messaging.NetworkMessages;
using UnityEngine;

/// <summary>
/// S'occupe de mettre à jour la partie "Map" du WorldState local. Surtout utilisé lors du StateConstruction
/// - Chargement de la map en fonction de l'ID reçu
/// - Chargement des cellules du CellSystem
/// 
/// Contient également le WorldState local.
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
        int mapID;
        if (stateConstruction.GetStateConstructionInfo("MAP_ID").Length > 0)
        {
            mapID = (int)(stateConstruction.GetStateConstructionInfo("MAP_ID")[0].Information); // Prend l'information du premier éléments des StateUpdateObjects de type "MAP_ID" et le cast en int.
            LoadMap(mapID);
            BuildCellSystem(mapID);
        }
        else
        {
            BloodAndBileEngine.Debugger.Log("WARNING - Pas d'identifiant de map !", UnityEngine.Color.yellow);
        }

        

    }

    void BuildCellSystem(int mapId)
    {
        BloodAndBileEngine.Debugger.Log("Construction des cellules...");
        World.AddData<CellSystem>(new CellSystem(Map.Maps[mapId]));
    }

    void LoadMap(int mapID)
    {
        BloodAndBileEngine.Debugger.Log("Chargement de la carte...");
        World.AddData<Map>(Map.Maps[mapID]);
        GameObject.Instantiate((GameObject) Resources.Load(Map.Maps[mapID].MapPrefabPath));
    }

    WorldState World;
}
