using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ResManager
{

	private static Dictionary< String, String > preparedFoldersPaths; //chemins préparés
	private static Dictionary< String, UnityEngine.Object > resources;//dictionnaire de ressources
	private static System.IO.DirectoryInfo resDir; //chemin absolu du dossier Resources
	private static Dictionary< String, List< String > > resourcesTree; // dictionnaire de groupes de ressources

	//variables temporaires
	private static bool tempBoo;
	private static List< String > tempLst;
	private static String tempStr;
	private static UnityEngine.Object tempObj;
	private static UnityEngine.Object[] tempTab;

	// fonctions d'initialisation 

	public static void init()
	{
		///<summary>
		/// fonction init à appeler avant toute utilisation des autres fonctions 
		/// </summary>
		preparedFoldersPaths = new Dictionary< String, String> ();
		resources = new Dictionary< String, UnityEngine.Object > ();
		resDir = new System.IO.DirectoryInfo (System.IO.Directory.GetCurrentDirectory () + "/Assets/Resources/");
		tempLst = new List< String > ();
		resourcesTree = new  Dictionary< String, List< String > > ();
		//groups = new Dictionary< String, List< String > > ();
		initPaths ();
	}

	private static void initPaths() //fonction qui analyse le répertoire Resources et enregistre le path de chaque dossier
	{
		///<summary>
		/// fonction qui analyse le répertoire Resources et enregistre le path de chaque dossier
		/// </summary>
		System.IO.DirectoryInfo tempDir;
		List<System.IO.DirectoryInfo> tempDirLst = new List<System.IO.DirectoryInfo> ();
		preparedFoldersPaths.Add ( "", "" );
		tempDirLst.Add(resDir);
		while( tempDirLst.Count != 0 )
		{
			tempDir = tempDirLst [0];
			tempDirLst.RemoveAt (0);
			preparedFoldersPaths.Add (tempDir.Name, tempDir.FullName.Substring (resDir.FullName.Length) + "/");
			resourcesTree.Add( tempDir.Name, new List<String>() );
			foreach( System.IO.FileInfo f in tempDir.GetFiles() )
			{
				if ( !(f.Extension  == ".meta") )
				{
					resourcesTree [tempDir.Name].Add ( System.IO.Path.GetFileNameWithoutExtension( f.Name ) );
				}
			}
			foreach( System.IO.DirectoryInfo d in tempDir.GetDirectories() )
			{
				tempDirLst.Add ( d );
				resourcesTree [tempDir.Name].Add ( d.Name );
			}
		}
		if ( preparedFoldersPaths.ContainsKey( "Resources" ) )
		{
			preparedFoldersPaths ["Resources"] = "";
		}
	}


	//fonction de LOAD

	public static bool load( String folderName, String fileName) // 
	{
		///<summary>
		/// charge le fichier fileName contenu dans le dossier folderName ( nom seul, pas de chemin ) 
		/// renvoie true si le fichier a été chargé
		/// </summary>
		if ( preparedFoldersPaths.ContainsKey( folderName ) ) 
		{
			tempObj = Resources.Load( preparedFoldersPaths[folderName] + fileName );
			if ( tempObj != null ) 
			{
				resources.Add ( fileName, tempObj );
				return true;
			}
			else
			{
				Debug.Log ( "no such file in " + folderName + " directory" );
			}
		}
		else
		{
			Debug.Log ( "no such directory" );
		}
		return false;
	}

	public static bool load( String folderName)
	///<summary>
	/// cette méthode charge le dossier folderName ( nom seul , pas de chemin )
	/// le dossier folderName doit être un sous dossier de Resources
	/// retourne true si la méthode a réussi
	/// </summary>
	{
		if( preparedFoldersPaths.ContainsKey( folderName ) )
		{
			tempLst.Add (folderName);
			while(tempLst.Count != 0 )
			{
				tempStr = tempLst [0];
				tempLst.RemoveAt ( 0 );
				tempObj = Resources.Load (preparedFoldersPaths [folderName]);
				if( resourcesTree.ContainsKey( tempStr ) )
				{
					foreach( String s in resourcesTree[tempStr] )
					{
						if( preparedFoldersPaths.ContainsKey( s ) )
						{
							tempLst.Add ( s );
						}
						else
						{
							if ( !resources.ContainsKey( s ) ) 
							{
								load (tempStr, s);
							}
						}
					}
				}
			}
		}
		else
		{
			Debug.Log ("Ce dossier n'existe pas, désolé ma couille");
		}
		return (false);		
	}

	public static void unload( String fileOrFolder )
	///<summary>>
	/// décharge le fichier ou le dossier (et sous dossiers) dont le nom est envoyé en paramètre
	/// si il n'existe pas, ou n'est pas chargé, aucune erreur n'est générée
	/// </summary>
	{
		tempLst.Add ( fileOrFolder );
		while(tempLst.Count != 0 )
		{
			tempStr = tempLst [0];
			tempLst.RemoveAt ( 0 );
			resources.Remove( tempStr);
			if( resourcesTree.ContainsKey( tempStr ) )
			{
				foreach( String s in resourcesTree[tempStr] )
				{
					tempLst.Add ( s );
				}
			}
					
		}

	}

	public static void unloadAll()
	///<summary>>
	/// décharge tout les fichiers chargés
	/// </summary>
	{
		resources.Clear (); 	
	}


	// fonctions de get <3

	public static UnityEngine.Object get( String resourceName)
	///<summary>
	/// renvoie la resource de nom resourceName si elle est chargée
	/// renvoie null sinon
	/// </summary>
	{
		resources.TryGetValue ( resourceName, out tempObj );
		return tempObj;
	}
		
	// il n'y a pas de fonction pour récupérer un dossier
	// elle pourra être ajoutée si elle est nécéssaire
	// fonctions de debug 

	public static void printPreparedPaths()
	{
		///<summary>
		/// affiche les chemins préparés par la classe pour accéder au dossiers
		/// utile pour tester si tout les dossiers sont biens détéctés 
		/// </summary>
		foreach(KeyValuePair< String, String> entry in preparedFoldersPaths)
		{
			Debug.Log ("|" + entry.Key + "| : |" + entry.Value + "|");
		}
	}

	public static void printLoadedResources()
	{
		///<summary>
		/// affiche toutes les ressources chargées 
		/// </summary>
		foreach(KeyValuePair< String, UnityEngine.Object > entry in resources)
		{
			Debug.Log ("|" + entry.Key + "| : |" + entry.Value + "|");
		}
		Debug.Log ( resources.Count + " resources loaded" );
	}

	public static void printResourcesTree()
	{
		///<summary>
		/// affiche l'arbre des dossiers et fichier de Resources
		/// utile pour tester si les dossiers sont bien détéctés
		/// </summary>
		foreach( KeyValuePair< String, List< String > > entry in resourcesTree)
		{
			Debug.Log ("|" + entry.Key + "| : |" + entry.Value.Count + " elements | ");
			foreach( String o in entry.Value )
			{
				Debug.Log (o + " , ");
			}
		}
		Debug.Log( resourcesTree.Count + " entries in the resources tree" );
	}
}

