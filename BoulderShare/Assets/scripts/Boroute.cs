using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーン遷移時ボルート受け渡し用オブジェクト
public class Boroute : MonoBehaviour {
	private string dRouteJson;
	private string dHScenesJson;
	private string dHoldsJson;
	private Sprite img;

	void Awake(){
		DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
	}
	public string GetDRoute(){
		return dRouteJson;
	}

	public void SetDRoute(string d){
		dRouteJson = d;
	}

	public Sprite GetImg(){
		return img;
	}

	public void SetImg(Sprite t){
		img = t;
	}


}
