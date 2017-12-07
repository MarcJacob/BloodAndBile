BloodAndBile

Principe du jeu :

Jeu de combat en ligne à la troisième personne, alliant stratégie, réflexes et apprentissage.
Chaque joueur incarne un "Mage" composé de 4 Humeurs : Le Sang, le Phlegm, la Bile Jaune et la Bile Noire.
Le but du jeu est d'utiliser ces ressources pour incanter des sorts offensifs, défensifs, économiques, pour prendre le contrôle de la carte et déséquilibrer les humeurs du Mage adverse et le vaincre pour remporter le match.

Modalités de développement :

Lien vers le cahier des charges : https://onedrive.live.com/view.aspx?resid=63A61E186A7C2B3D!621&ithint=file%2Cdocx&app=Word&authkey=!AGh8aTLBMQ5UatE

Développé en C# sur Unity. Pour le réseau, utilisation de l'API bas niveau UNet ("Transport Layer API").

Equipe :

Marc Jacob - Chef de projet, Développeur & Game Designer
Ilan Piperno - Développeur
Manuel Picon Bravo - Développeur
Martin Dalery - Développeur
Tom Toulier - Infographiste 3D & 2D

Le répertoire est divisé en une branche "Trunk" et une branche "Sandbox" par développeur. La branche Trunk sert de lieu de développement principal pour le développeur concerné. Lorsqu'une "Milestone" importante est atteinte, les branches Trunk sont fusionnées dans la branche "master", puis les branches Trunk sont mises à jour avec le contenu de master. "Sandbox" est une branche utilisée de manière optionnelle étant faite pour la prise de risques : si besoin, celle ci peut être supprimée et re-créée. Cela permet de n'effectuer que le développement "contrôlé" dans Trunk et ainsi d'être certain d'avoir un historique des commits sans trop de coquilles.

Le projet est découpé en quatre sous-projets :

- Un sous projet C# "Sources" contenant du code important partagé entre tous les autres sous-projets
- Un sous projet Unity "Client" contenant les données de l'exécutable Client, qui sera utilisé par les joueurs.
- Un sous projet Unity "Master Server" contenant les données de l'exécutable Master Server, qui sera utilisé par le jeu pour répartir les clients sur des Match Servers et pour stocker des informations de compte.
- Un sous projet Unity "Match Server" contenant les données de l'exécutable Match Server, qui sera utilisé par le jeu pour héberger des matchs de Blood & Bile en ligne

Réalisé dans le cadre du projet tuteuré à l'IUT Informatique Lyon 1, durant le troisième semestre.
