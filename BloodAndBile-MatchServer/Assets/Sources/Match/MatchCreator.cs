using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            // Map de test : 2 cellules

            float[] cellData = new float[]
            {
                // Positions    // Dimensions   // Hauteurs
                0f, 0f, 0f,     10f, 10f,       0f, 0f ,     // Cellule dans l'angle de la map de 10 x 10 plate.
                0f, 0f, 10f,    10f, 10f,       1f, 1f ,     // Cellule de 10x10 pentue.   
            };

            // Création du CellSystem.

            BloodAndBileEngine.WorldState.CellSystem cellSystem;
            cellSystem = new BloodAndBileEngine.WorldState.CellSystem(cellData);
            startWorldState.AddData<BloodAndBileEngine.WorldState.CellSystem>(cellSystem);

        //

        //...

        return m;
    }
}
