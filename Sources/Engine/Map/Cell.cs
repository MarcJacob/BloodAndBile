using System;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Cette classe contient l'enssemble des Entity qui doivent etre contenues sur cette cellule.
 * Sera utilisé pour le pathfinding
 */

public class Cell
{
    List<Entity> Entities;
    Vector3 Coordinates;

    //Compléter lorsqu'Ilan aura terminé et transféré sa classe Entity

    public Cell(float posX, float posY, float high)
    {
        posX *= World.SizeCellX;
        posY *= World.SizeCellY; 
        Coordinates = new Vector3(posY, high, posX);
        Entities = new List<Entity>();
    }



}
