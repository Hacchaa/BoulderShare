using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class DRoute : MonoBehaviour{
	public Data route;
	public DHolds dHolds;
	public DHScenes dHScenes;
	public InputField title;
	public Incline incline;
	public Phase1 phase1;

	private void Construction(){
		route = new Data();
		
		route.time = System.DateTime.Now.ToString("yyyyMMddHHmmss");
		route.title = title.text;
		route.holds = dHolds.ToJson();
		route.hScenes = dHScenes.ToJson();
		route.incline = incline.GetIncline();
		route.scaleH2M = phase1.GetModelSize();
	}

	public String ToJson(){
		Construction();
		return JsonUtility.ToJson(route);
	}


	public void FromJson(string json){
		route = JsonUtility.FromJson<Data>(json);
		
		Debug.Log("title:"+route.title);
		Debug.Log("time:"+route.time);
		Debug.Log("holds:"+route.holds);
		Debug.Log("hScenes:"+route.hScenes);
		dHolds.FromJson(route.holds);
		dHScenes.FromJson(route.hScenes);
		Debug.Break();
		title.text = route.title;
		incline.SetIncline(route.incline);
		phase1.SetModelSize(route.scaleH2M);
	}

	[Serializable]
	public class Data {
		public string time;
		public string title;
		public string holds;
		public string hScenes;
		public int incline;
		public float scaleH2M;
	}
}


