
/**
 * <summary> Un ClientState est un objet qui est capable de se mettre à jour et qui doit pouvoir être activé et désactivé (OnEntry, OnExit) </summary>
 */ 
public interface IClientState
{
    void OnEntry(); // Quand cet état débute.
    void OnExit(); // Quand cet état se termine.
    void OnUpdate(); // Quand cet état est mit à jour.
}

