using System;
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
        m.AddModule<EntitiesManagerModule>(new EntitiesManagerModule(m));
        m.AddModule<StateUpdateModule>(new StateUpdateModule(m)); // Création du module StateUpdate
        m.AddModule<MatchOutcomeManagerModule>(new MatchOutcomeManagerModule(m));

        // Création du WorldState initial.

        BloodAndBileEngine.WorldState.WorldState startWorldState = m.GetModule<MapStateModule>().GetWorldState();
        // Objectif : initialise le startWorldState en fonction des informations dont on dispose.

        // INITIALISER TOUS LES WorldStateData ici !


        // Ajout du EntityFactory
        BloodAndBileEngine.WorldState.WorldStateData.WorldEntityFactory factory = new BloodAndBileEngine.WorldState.WorldStateData.WorldEntityFactory(startWorldState);
        startWorldState.AddData(factory);

        // Initialisation des cellules.

        if (BloodAndBileEngine.WorldState.Map.Maps == null)
            BloodAndBileEngine.WorldState.Map.LoadMaps();
        BloodAndBileEngine.WorldState.Map map = BloodAndBileEngine.WorldState.Map.Maps[UnityEngine.Random.Range(0, BloodAndBileEngine.WorldState.Map.Maps.Count-1)];
        
        // Création du CellSystem.

        BloodAndBileEngine.WorldState.CellSystem cellSystem;
        cellSystem = new BloodAndBileEngine.WorldState.CellSystem(map);
        startWorldState.AddData<BloodAndBileEngine.WorldState.CellSystem>(cellSystem);
        startWorldState.AddData<BloodAndBileEngine.WorldState.Map>(map);


        return m;
    }
}
