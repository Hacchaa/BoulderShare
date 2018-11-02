using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Post : MonoBehaviour {
	public Observer obs;
	public HScenes hScenes;
	public Shield shield;
	public ThreeDModel model;
	public SpriteRenderer rend;
	public BoRouteLSManager bManager;


	public void Start(){
		if(bManager.IsLoaded()){
			bManager.BoRouteLoadThird();
		}
	}

	public void Submit(){
		bManager.SaveBoRoute();
		SceneManager.LoadScene("routeview");
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

/*
	public void Load(){
		string path = Application.persistentDataPath + Observer.ROUTEPATH + "20180816154722";
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
	}*/
}
