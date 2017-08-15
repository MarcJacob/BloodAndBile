using System;
using System.Collections.Generic;


/**
 * <summary> Un Message est un "paquet" de données étiquetté par un type (byte).
 * Les Messages sont faits pour être lus par des MessageReader (A ne pas confondre avec MessageReceiver !) afin que leur données soient
 * transmises aux fonctions Handler correspondantes à leur type. </summary>
 */ 
public class NetworkMessage
{
    public short Type; // Ce type de message n'a pas de "Type" car il n'est jamais envoyé. Le type est à définir dans les classes filles.

    public NetworkMessage(byte type)
    {
        Type = type;
    }
}

/* COMMENT CREER UN NOUVEAU TYPE DE MESSAGE ?
 * Pour pratiquement chaque situation d'envoie de données (sur le réseau ou pas), il faudra un type de message bien particulier 
 * (cela dit, moins il en existe, mieux c'est).
 * 
 * Pour créer un nouveau type de message, créer une nouvelle classe héritant de NetworkMessage.. et c'est bon. Ensuite, il suffit d'envoyer le
 * message sur le réseau et, en utilisant son type, de le réceptionner puis de le caster.
 */