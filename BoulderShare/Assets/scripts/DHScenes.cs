using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DHScenes : MonoBehaviour {
	public DataArray scenes;
	public HScenes hscenes;
	[SerializeField]
	private Issues issues;

	private void Construction(){
		hscenes.Save();
		scenes = new DataArray();
		scenes.failedList = issues.GetList();
		scenes.numOfCreatingHScene = HScene.GetNum();
		scenes.arr = new Data[hscenes.GetNum()];
		int i = 0;
		foreach(HScene scene in hscenes.GetScenes()){
			Data data = new Data();
			data.id = scene.GetID();
			
			data.holdsOnHand = scene.GetOnHolds();
			data.comments = scene.GetComments();

			data.pose = scene.GetPose();
			data.rotate = scene.GetPRotate();
			data.posable = scene.IsPose();
			scenes.arr[i] = data;
			i++;
		}
	}

	public String ToJson(){
		Construction();
		return JsonUtility.ToJson(scenes);
	}

	public String GetEmptyJson(){
		DataArray data = new DataArray();
		data.arr = new Data[0];
		data.failedList = new List<string>();
		return JsonUtility.ToJson(data);
	}



	public void FromJson(string json){
		scenes = JsonUtility.FromJson<DataArray>(json);
		List<HScene> list = new List<HScene>();

		for(int i = 0 ; i < scenes.arr.Length ; i++){
			Data data = scenes.arr[i];
			HScene scene = new HScene();
			scene.SetID(data.id);
			scene.SetOnHolds(data.holdsOnHand);
			scene.SaveComments(data.comments);

			if (data.posable){
				scene.SavePose(data.pose);
				scene.SavePRotate(data.rotate);
			}
			list.Add(scene);
		}

		if(list.Count == 0){
			list.Add(new HScene());
		}

		hscenes.Construction(list, scenes.numOfCreatingHScene);
		issues.Regist(scenes.failedList);
	}

	public static DataArray ConvertJsonToDHScenes(string json){
		return JsonUtility.FromJson<DataArray>(json);
	}
	
	[Serializable]
	public class DataArray{
		public Data[] arr;
		public List<string> failedList;
		//最初からこのシーンが保存されるまでに作られたシーンの数
		public int numOfCreatingHScene;
	}

	[Serializable]
	public class Data {
		public int id;
		public string[] holdsOnHand;
		public List<string> comments;
		public Vector3[] pose;
		public Quaternion[] rotate;
		public bool posable;
	}
}
