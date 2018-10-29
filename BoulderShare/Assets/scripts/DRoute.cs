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
	public InputField place;
	public Incline incline;
	public Phase1 phase1;
	public Dropdown gradeDD;
	public Date_DD dateDD;
	public Dropdown tryDD;
	public Toggle complete;
	public InputField gComment;

	private void Construction(){
		route = new Data();
		
		route.timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
		route.date = dateDD.GetDate();
		route.title = "";
		route.place = place.text;
		route.holds = dHolds.ToJson();
		route.hScenes = dHScenes.ToJson();
		route.incline = incline.GetIncline();
		route.grade = gradeDD.value;
		route.tryCount = tryDD.value;
		route.isComplete = complete.isOn;
		route.scaleH2M = phase1.GetModelSize();
		route.globalComment = gComment.text;
	}

	public String ToJson(){
		Construction();
		return JsonUtility.ToJson(route);
	}


	public void FromJson(string json){
		route = JsonUtility.FromJson<Data>(json);
		
		Debug.Log("place:"+route.place);
		Debug.Log("timestamp:"+route.timestamp);
		Debug.Log("date:"+route.date);
		Debug.Log("grade:"+route.grade);
		Debug.Log("holds:"+route.holds);
		Debug.Log("hScenes:"+route.hScenes);
		dateDD.SetDate(route.date);
		dHolds.FromJson(route.holds);
		dHScenes.FromJson(route.hScenes);
		place.text = route.place;
		incline.SetIncline(route.incline);
		gradeDD.value = route.grade;
		tryDD.value = route.tryCount;
		complete.isOn = route.isComplete;
		phase1.SetModelSize(route.scaleH2M);
		gComment.text = route.globalComment;
	}

	[Serializable]
	public class Data {
		public string timestamp;
		public string date;
		public string title;
		public string place;
		public string holds;
		public string hScenes;
		public string globalComment;
		public int incline;
		public int grade;
		public int tryCount;
		public bool isComplete;
		public float scaleH2M;
	}
}


