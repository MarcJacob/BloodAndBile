﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * <summary> Classe "Factory" qui se charge de la création d'un match. </summary>
 */
public static class MatchCreator
{
    public static Match CreateMatch(int[] players)
    {
        Match m = new Match();
        m.SetPlayerConnectionIDs(players);
        m.AddModule<MapStateModule>(new MapStateModule(m)); // Création du World State lié au match
        m.AddModule<StateUpdateModule>(new StateUpdateModule(m)); // Création du module StateUpdate


        // Création du WorldState initial.

        BloodAndBileEngine.WorldState.WorldState startWorldState = m.GetModule<MapStateModule>().GetWorldState();
        // Objectif : initialise le startWorldState en fonction des informations dont on dispose.

        // INITIALISER TOUS LES WorldStateData ici !

        // Initialisation des cellules.

        if (BloodAndBileEngine.WorldState.Map.Maps == null)
            BloodAndBileEngine.WorldState.Map.LoadMaps();

        BloodAndBileEngine.WorldState.Map map = BloodAndBileEngine.WorldState.Map.Maps[(int)UnityEngine.Random.Range(0.0f, BloodAndBileEngine.WorldState.Map.Maps.Count)];
        
        // Création du CellSystem.

        BloodAndBileEngine.WorldState.CellSystem cellSystem;
        cellSystem = new BloodAndBileEngine.WorldState.CellSystem(map.ConstructionData);
        startWorldState.AddData<BloodAndBileEngine.WorldState.CellSystem>(cellSystem);
        startWorldState.AddData<BloodAndBileEngine.WorldState.Map>(map);

        //




        //...

        return m;
    }
}
