using UnityEngine;
using System.Collections;
/// <summary>
/// Base de toutes les entités (= quelque chose de visible en jeu, disposant d'une position, d'une rotation, d'une taille (et hauteur ), 
/// d'une vitesse, et d'un identifiant unique), classe abstraite.
/// </summary>
public abstract class Entity
{
    public int ID { get; private set; }
    protected static int LastID = 0;
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public float Size { get; set; }
    public float Height { get; set; }
    public bool Destroyed { get; private set; }


    public Entity(Vector3 pos, Quaternion rot, float size, float height, World world)
    {
        ID = LastID++;
        Position = pos;
        Rotation = rot;
        Size = size;
        Height = height;
        world.GetCellFromPosition(pos).Entities.add(this);
        Destroyed = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> Le nom et l'ID de 'entité </returns>
    public override string ToString()
    {
        return "Entity " + ID;
    }

    public void Destroy()
    {
        Destroyed = true;   
    }
    public abstract void Update();

}
