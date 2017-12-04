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
        m.AddModule<EntitiesStateModule>(new EntitiesStateModule(m));
        m.AddModule<StateUpdateModule>(new StateUpdateModule(m)); // Création du module StateUpdate


        // Création du WorldState initial.

        BloodAndBileEngine.WorldState.WorldState startWorldState = m.GetModule<MapStateModule>().GetWorldState();
        // Objectif : initialise le startWorldState en fonction des informations dont on dispose.

        // INITIALISER TOUS LES WorldStateData ici !

        // Initialisation des cellules.

        if (BloodAndBileEngine.WorldState.Map.Maps == null)
            BloodAndBileEngine.WorldState.Map.LoadMaps();

        BloodAndBileEngine.WorldState.Map map = BloodAndBileEngine.WorldState.Map.Maps[UnityEngine.Random.Range(0, BloodAndBileEngine.WorldState.Map.Maps.Count-1)];
        
        // Création du CellSystem.

        BloodAndBileEngine.WorldState.CellSystem cellSystem;
        cellSystem = new BloodAndBileEngine.WorldState.CellSystem(map);
        startWorldState.AddData<BloodAndBileEngine.WorldState.CellSystem>(cellSystem);
        startWorldState.AddData<BloodAndBileEngine.WorldState.Map>(map);

        //Création des joueurs, placement sur la map, et assignation d'un connexionID pour chaque

        List<int> usedCells = new List<int>();
        foreach(int coId in players)
        {
            BloodAndBileEngine.WorldState.Cell cell = cellSystem.FindSpawnPoint(usedCells.ToArray());
            if (cell != null)
            {
                usedCells.Add(cell.ID);
                BloodAndBileEngine.Entity player = BloodAndBileEngine.EntityFactories.EntityFactory.BuildPlayer(startWorldState, cell.GetPosition(), Quaternion.identity, 0.5f, 2.0f);
                m.SetPlayerEntity(coId, (int)player.ID);
                Debug.Log("Entité joueur d'ID " + player.ID + " associée à la connexion d'ID " + coId);
            }
            else
                Debug.Log("Aucun point de spawn trouvé !");
        }

        return m;
    }
}
