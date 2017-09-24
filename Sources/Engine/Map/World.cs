using System;
using UnityEngine;

/* Cette classe est utilisée pour gérer l'ensemble des entités qui seront utilisées lors d'un match.
 * 
 * 
 * */

public class World
{
    private Cell[,] Map;
    public static int SizeCellX { get; private set; }
    public static int SizeCellY { get; private set; }
    private int SizeMapX;
    private int SizeMapY;
    private int NBCellX;
    private int NBCellY;



    /// <summary>
    /// Constructeur de world :
    /// Initialise les cellules
    /// </summary>
    /// <param name="sizeMapX">Taille de la carte chargée dans la partie en abscisses</param>
    /// <param name="sizeMapY">Taille de la carte chargée dans la partie en ordonnées</param>
    public World(int sizeMapX, int sizeMapY)
    {
        SizeMapX = sizeMapX;
        SizeMapY = sizeMapY;
        NBCellX = SizeMapX / SizeCellX + 1;
        NBCellY = SizeMapY / SizeCellY + 1;
        for (int i = 0; i < NBCellX; i++ )
        {
            for (int j = 0; j < NBCellY; j++)
            {
                Map[i, j] = new Cell((float) i, (float) j, 0); 
            }
        }
    }


    // A ecrire pour Ilan
   public Cell GetCellFromPosition(Vector3 position)
    {
        return Map[(int)(position.z / SizeCellX), (int)(position.x / SizeCellY)];
    }
}
