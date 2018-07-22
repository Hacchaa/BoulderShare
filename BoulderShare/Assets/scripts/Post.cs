using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

public class Post : MonoBehaviour {
	public DRoute dRoute;
	public Observer obs;
	public HScenes hScenes;
	public Shield shield;
	private const string OUTPUTPATH = "/route/";

	public void Submit(){
		string json = dRoute.ToJson();
		Debug.Log(json);

		string path = Application.persistentDataPath + OUTPUTPATH + dRoute.route.time ;
		if (!Directory.Exists(path)){
			Directory.CreateDirectory(path);
			/*
			FileSystemAccessRule rule = new FileSystemAccessRule(
			    new NTAccount("everyone"),
			    FileSystemRights.FullControl, 
			    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
			    PropagationFlags.None,
			    AccessControlType.Allow);
      	
			  DirectorySecurity security = Directory.GetAccessControl(path);
			  security.SetAccessRule(rule);
			  Directory.SetAccessControl(path, security);*/
		}
		string filepath =  path + "/route.txt";
		File.WriteAllText(filepath, json);
	}

	public void Open(){
		hScenes.Save();
		gameObject.SetActive(true);
		shield.OpenIgnoreTouch();
	}

	public void Close(){
		gameObject.SetActive(false);
		shield.CloseIgnoreTouch();
	}


	public void Load(){
		string path = Application.persistentDataPath + OUTPUTPATH + "20180722214543/route.txt";
		string routeJson = File.ReadAllText(path);
		//obs.InitHoldsAndScenes();
		dRoute.FromJson(routeJson);

		//phaseを変える
		gameObject.SetActive(false);
		obs.SwitchPhase((int)Observer.Phase.HOLD_EDIT);
	}
}
