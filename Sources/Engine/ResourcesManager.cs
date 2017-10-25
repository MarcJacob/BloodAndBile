using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ResourcesManager 
{
	private static Dictionary<String,UnityEngine.Object> tempGroup;
	private static UnityEngine.Object tempRes;
	private static String tempStr;
	private static Boolean tempBool;
	private static UnityEngine.Object[] tempTab;

	private static Dictionary<String,Dictionary<String,UnityEngine.Object>> resGroups;
	private static Dictionary<String,String> resList;


	public static void init()
	{
		resGroups = new Dictionary<String,Dictionary<String,UnityEngine.Object>>();
		resList = new Dictionary<String, String> ();
		resGroups.Add ("default",new Dictionary<String,UnityEngine.Object> ());
	}

	public static void load(String filePath,String name, String groupName)
	{
		resetTemp ();
		Debug.Log ("Tentative de load");
		tempRes = Resources.Load (filePath);
		if (tempRes == null) 
		{
			Debug.Log ("echec du loading de" + filePath  );
		} 
		else 
		{
			Debug.Log("ressource loadée :" + filePath + ": " + tempRes.name);
			tempGroup = getOrCreateGroup (groupName);
			tempGroup.Add (name, tempRes);
			resList.Add (name, groupName);
		}
	}

	public static void loadFolder (String folderPath, String groupName)
	{
		resetTemp ();
		tempTab = Resources.LoadAll (folderPath);
		if (tempTab == null) 
		{
			Debug.Log ("echec du loading de" + folderPath);
		} 
		else 
		{
			tempGroup = getOrCreateGroup (groupName);
			foreach (UnityEngine.Object o in tempTab) 
			{
				tempGroup.Add (o.name, o);
				resList.Add (o.name, groupName);
			}
		}
				
	}

	public static void unload(String resName)
	{
		tempStr = null;
		tempBool =resList.TryGetValue (resName, out tempStr);
		if (!tempBool) {
			Debug.Log ("Aucune ressource ne porte ce nom");
		} 
		else 
		{
			resGroups.TryGetValue(tempStr,out tempGroup);
			if (tempGroup == null) {
				//anormal
			} 
			else 
			{
				tempGroup.Remove (resName);
			}
		}
		resetTemp ();
	}

	public static void unloadGroup(String groupName)
	{
		resetTemp ();
		resGroups.TryGetValue (groupName, out tempGroup);
		if (tempGroup == null) 
		{
			Debug.Log ("Aucune group ne porte ce nom");
		} 
		else 
		{
			foreach (String resName in tempGroup.Keys) 
			{
				resList.Remove (resName);
			}
			resGroups.Remove (groupName);
		}
	}
	
	private static void resetTemp()
	{
		tempGroup = null;
		tempRes = null;
		tempStr = null;
		tempTab = null;
	}
		
	public static UnityEngine.Object getRes(String resName)
	{
		tempBool = resList.TryGetValue (resName, out tempStr);
		if (!tempBool) 
		{
			Debug.Log ("pas de ressources avec ce nom");
			return null;
		} 
		else 
		{
			resGroups.TryGetValue (tempStr, out tempGroup);
			tempGroup.TryGetValue (resName, out tempRes);
		}
		return (tempRes);
	}

	public static Dictionary<String,UnityEngine.Object> getResGroup(String groupName)
	{
		tempBool = resGroups.TryGetValue (groupName, out tempGroup);
		if (!tempBool) {
			return(null);
		} 
		else 
		{
			return(tempGroup);
		}
	}

	private static Dictionary<String,UnityEngine.Object> getOrCreateGroup(String groupName)
	{
		tempBool = resGroups.TryGetValue (groupName, out tempGroup);
		if (tempBool == false) 
		{
			tempGroup = new Dictionary<string, UnityEngine.Object> ();
			resGroups.Add(groupName,tempGroup);
		}
		return tempGroup;
	}
}
