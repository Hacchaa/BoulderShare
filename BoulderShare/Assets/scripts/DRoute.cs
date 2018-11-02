using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class DRoute : MonoBehaviour{
	public Data route;
	public InputField place;
	public Incline incline;
	public Phase1 phase1;
	public Dropdown gradeDD;
	public Date_DD dateDD;
	public Dropdown tryDD;
	public Toggle complete;
	public InputField gComment;
	public BoRouteLSManager bManager;
	private bool isAwakeCalled = false;

	void Awake (){
		isAwakeCalled = true;
	}
	public void Construction(){
		//上書き保存の場合、タイムスタンプは更新しない
		if (!bManager.IsLoaded()){
			route = new Data();
			route.timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
		}
		route.date = dateDD.GetDate();
		route.title = "";
		route.place = place.text;
		route.incline = incline.GetIncline();
		route.grade = gradeDD.value;
		route.tryCount = tryDD.value;
		route.isComplete = complete.isOn;
		route.scaleH2M = phase1.GetModelSize();
		route.globalComment = gComment.text;
	}

	//jsonをオブジェクトに変換
	public void ConstructionFromJson(string json){
		route = JsonUtility.FromJson<Data>(json);
	}

	public String ToJson(){
		if (isAwakeCalled){
			Construction();
			return JsonUtility.ToJson(route);
		}
		return "{}";
	}

	/*
	public void FromJson(string json, bool isBoRouteLoaded = false){
		ConstructionFromJson(json);

		Debug.Log("place:"+route.place);
		Debug.Log("timestamp:"+route.timestamp);
		Debug.Log("date:"+route.date);
		Debug.Log("grade:"+route.grade);
		Debug.Log("holds:"+route.holds);
		Debug.Log("hScenes:"+route.hScenes);
		dHolds.FromJson(route.holds);
		incline.SetIncline(route.incline);
		phase1.SetModelSize(route.scaleH2M); 

		if (!isBoRouteLoaded){
			dateDD.SetDate(route.date);
			dHScenes.FromJson(route.hScenes);
			place.text = route.place;
			gradeDD.value = route.grade;
			tryDD.value = route.tryCount;
			complete.isOn = route.isComplete;
			gComment.text = route.globalComment;
		}
	}*/

	public void LoadFirst(){
		incline.SetIncline(route.incline);
		phase1.SetModelSize(route.scaleH2M); 
	} 

	//ボルートが新規作成されていない場合の処理
	public void LoadThird(){
		dateDD.SetDate(route.date);
		place.text = route.place;
		gradeDD.value = route.grade;
		tryDD.value = route.tryCount;
		complete.isOn = route.isComplete;
		gComment.text = route.globalComment;
	}

	public static DataArr ConvertJsonToDRouteList(string json){
		return JsonUtility.FromJson<DataArr>(json);
	}

	public static string ConvertDRouteListToJson(DataArr dataArr){
		return JsonUtility.ToJson(dataArr);
	}

	public static string ConvertDRouteToJson(Data data){
		return JsonUtility.ToJson(data);
	}
	public static Data ConvertJsonToDRoute(string json){
		return JsonUtility.FromJson<Data>(json);
	}

	[Serializable]
	public class DataArr{
		public Data[] arr;
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


