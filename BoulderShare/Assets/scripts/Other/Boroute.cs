using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//シーン遷移時ボルート受け渡し用オブジェクト
public class Boroute : MonoBehaviour {
	private string dRouteJson;
	private string dHScenesJson;
	private string dHoldsJson;
	private Sprite img;
	private bool isRouteTemp = false;

	void Awake(){
		DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
	}
	public string GetDRoute(){
		return dRouteJson;
	}

	public void SetDRoute(string d){
		dRouteJson = d;
	}

	public string GetDHolds(){
		return dHoldsJson;
	}

	public void SetDHolds(string d){
		dHoldsJson = d;
	}

	public string GetDHScenes(){
		return dHScenesJson;
	}

	public void SetDHScenes(string d){
		dHScenesJson = d;
	}

	public Sprite GetImg(){
		return img;
	}

	public void SetImg(Sprite t){
		img = t;
	}

	public void SetIsRouteTemp(bool b){
		isRouteTemp = b;
	}

	public bool IsRouteTemp(){
		return isRouteTemp;
	}
}
