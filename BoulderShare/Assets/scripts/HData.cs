using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HData : MonoBehaviour{
	public DataArray holds;
	public static int HTYPE_NORMAL = 1;
	public static int HTYPE_FOOT = 2;
	public GameObject hold_Normal;
	public GameObject hold_Foot;

	private void SetHolds(){
		Data data;
		holds = new DataArray();
		holds.arr = new Data[transform.childCount];
		int i = 0;
		foreach(Transform child in transform){
			data = new Data();
			data.id = i;
			data.x = (double)child.position.x;
			data.y = (double)child.position.y;
			data.scale = (double)child.localScale.x;
			if (child.tag == "Hold_Normal"){
				data.type = HTYPE_NORMAL;
			}else if (child.tag == "Hold_Foot"){
				data.type = HTYPE_FOOT;
			}
			holds.arr[i] = data;
			i++;
		}
	}

	public String ToJSON(){
		SetHolds();
		return JsonUtility.ToJson(holds);
	}

	public void SaveHolds(){
		SetHolds();
		//Debug.Log(JsonUtility.ToJson(holds));
		//PlayerPrefs.SetString(Observer.KEY_HOLDS, JsonUtility.ToJson(holds));
	}

	public void Call(){
		SetHolds();
		Debug.Log(JsonUtility.ToJson(holds));
	}

	public void InitHolds(Transform parent, string json){
		holds = JsonUtility.FromJson<DataArray>(json);
		GameObject hold, target;
		target = null;
		for(int i = 0 ; i < holds.arr.Length ; i++){
			if (holds.arr[i].type == HTYPE_NORMAL){
				target = hold_Normal;
			}else if (holds.arr[i].type == HTYPE_FOOT){
				target = hold_Foot;
			}
			hold = Instantiate(target, Vector3.zero , Quaternion.identity, parent);
			hold.transform.localPosition = new Vector3((float)holds.arr[i].x, (float)holds.arr[i].y, 0.0f);
			hold.transform.localScale = Vector3.one * (float)holds.arr[i].scale;
		}
	}

	[Serializable]
	public class DataArray{
		public Data[] arr;
	}

	[Serializable]
	public class Data {
		public int id;
		public double x, y;
		public double scale;
		public int type;
	}
}


