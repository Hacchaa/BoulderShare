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
	public ThreeDModel model;
	public SpriteRenderer rend;

	public void Submit(){
		string json = dRoute.ToJson();
		Debug.Log(json);

		string path = Application.persistentDataPath + Observer.ROUTEPATH + dRoute.route.time ;
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

		filepath = Application.persistentDataPath + Observer.WALLPATH;
		string toPath = path + "/Wall.png";

		try {
            if (File.Exists(filepath) && !File.Exists(toPath)){
            	File.Move(filepath, toPath);
            }
        } 
        catch (Exception e) 
        {
            Debug.Log("The process failed: " + e.ToString());
        }
	}

	public void Open(){
		hScenes.Save();
		gameObject.SetActive(true);
		shield.OpenIgnoreTouch();
		model.ChangeMode((int)ThreeDModel.Mode.DEFAULT);
	}

	public void Close(){
		gameObject.SetActive(false);
		shield.CloseIgnoreTouch();
		model.ChangeMode((int)ThreeDModel.Mode.WINDOW);
	}


	public void Load(){
		string path = Application.persistentDataPath + Observer.ROUTEPATH + "20180723194745";
		string routeJson = File.ReadAllText(path + "/route.txt");
		//obs.InitHoldsAndScenes();
		dRoute.FromJson(routeJson);
		Texture2D texture = new Texture2D(0, 0);
		texture.LoadImage(LoadBytes(path + "/Wall.png"));
		rend.sprite = Sprite.Create(
                texture, 
                new Rect(0.0f, 0.0f, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f),
                texture.height/4);
		//phaseを変える
		shield.CloseIgnoreTouch();
		obs.SwitchPhase((int)Observer.Phase.HOLD_EDIT);
	}

	byte[] LoadBytes(string path) {
		FileStream fs = new FileStream(path, FileMode.Open);
		BinaryReader bin = new BinaryReader(fs);
		byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
		bin.Close();
		return result;
	}
}
