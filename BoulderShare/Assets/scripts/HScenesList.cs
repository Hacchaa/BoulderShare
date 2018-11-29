using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HScenesList : MonoBehaviour {
	[SerializeField]
	private List<string> hsList;
	[SerializeField]
	private DHScenes dHScenes;

	void Awake(){
		hsList = new List<string>();
	}
	
	public string ToJson(){
		Data data = new Data();
		data.hScenesList = hsList;
		return JsonUtility.ToJson(data);
	}

	public void FromJson(string json){
		hsList = JsonUtility.FromJson<Data>(json).hScenesList;
		if (hsList.Count == 0){
			hsList.Add(dHScenes.GetEmptyJson());
		}
		LoadLatestHScenes();
	}

	public void LoadLatestHScenes(){
		dHScenes.FromJson(hsList[hsList.Count-1]);
	}
	//HScenesの登録
	public void RegistCurHScenes(){
		string json = dHScenes.ToJson();
		hsList.Add(json);
	}

	public string GetLatestHScenes(){
		int n = hsList.Count - 1;
		if (n >= 0){
			return hsList[hsList.Count-1];
		}
		return "";
	}
	public string GetEmptyJson(){
		Data data = new Data();
		data.hScenesList = new List<String>();
		return JsonUtility.ToJson(data);
	}
	public static List<string> ConvertJsonToList(string json){
		return JsonUtility.FromJson<Data>(json).hScenesList;
	}
	[Serializable]
	public class Data{
		public List<string> hScenesList;
	}
}
