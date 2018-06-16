using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DHScenes : MonoBehaviour {
	public DataArray scenes;
	public HScenes hscenes;
	// Use this for initialization
	void Start () {
		
	}

	public void Construction(){
		scenes = new DataArray();
		scenes.arr = new Data[hscenes.GetNum()];
		int i = 0;
		foreach(HScene scene in hscenes.GetScenes()){
			Data data = new Data();
			data.id = i;
			
			data.holdsOnHand = scene.GetOnHolds();
			data.comments = scene.GetComments();

			data.pose = scene.GetPose();
			data.rotate = scene.GetPRotate();
			data.posable = scene.IsPose();
			scenes.arr[i] = data;
			i++;
		}
	}

	public string ToJson(){
		Construction();
		return JsonUtility.ToJson(scenes);
	}


	public void FromJson(string json){
		scenes = JsonUtility.FromJson<DataArray>(json);
		List<HScene> list = new List<HScene>();

		for(int i = 0 ; i < scenes.arr.Length ; i++){
			Data data = scenes.arr[i];
			HScene scene = new HScene();

			scene.SetOnHolds(data.holdsOnHand);
			scene.SaveComments(data.comments);

			if (data.posable){
				scene.SavePose(data.pose);
				scene.SavePRotate(data.rotate);
			}
			list.Add(scene);
		}

		hscenes.Construction(list);
		
	}
	
	[Serializable]
	public class DataArray{
		public Data[] arr;
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
