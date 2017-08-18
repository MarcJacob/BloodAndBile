
/**
 * <summary> Classe mère des états possibles du client. Chaque état est exécuté en trois étape à chaque image : Inputs & Update.
 * Init est exécuté à la première image. Exit est exécutée lorsque que l'état change. </summary>
 */ 
public abstract class ClientState
{
    public abstract void Init();
    public abstract void Inputs();
    public abstract void Update();
    public abstract void Exit();
}

/*

    Comment créer un nouvel état  ("Client State") ?
    Créer une classe du nom de cet état (ne pas oublier le suffixe "State" !), et la faire hériter de la classe ClientState.

    Toute classe héritant de ClientState DOIT implémenter les méthodes Init, Inputs et Update, même ci celles ci sont vides !
    Pour tester votre état de client, exécutez la méthode statique Client.ChangeState(new <NomDeVotreEtat>(<arguments constructeur>).

    Avant de créer un nouvel état, soyez certain que cela est nécessaire. Créer un nouvel état ne devrait être fait QUE quand cela permet 
    d'accomplir une tâche importante qui a peu de rapports avec les tâches accomplies par les autres états (par exemple, pas besoin de séparer
    les états "Post match : victoire" et "Post match : défaite".

*/