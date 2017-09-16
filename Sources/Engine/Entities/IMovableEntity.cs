using UnityEngine;
using System.Collections;

/// <summary>
/// Interface que toutes les entités ouvant se déplacer doivent implémenter. Contient la méthode Move(Vector3 dest).
/// </summary>
public interface IMovableEntity
{
    void Move(Vector3 dest);
}
