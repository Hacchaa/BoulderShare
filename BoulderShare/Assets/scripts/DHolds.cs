using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DHolds : MonoBehaviour{
	public DataArray holds;
	public static int HTYPE_NORMAL = 1;
	public static int HTYPE_FOOT = 2;
	public GameObject hold_Normal;
	public GameObject hold_Foot;

	private void Construction(){
		Data data;
		holds = new DataArray();
		holds.arr = new Data[transform.childCount];
		int i = 0;
		foreach(Transform child in transform){
			data = new Data();
			data.id = i;
			data.name = child.gameObject.name;
			data.x = (double)child.position.x;
			data.y = (double)child.position.y;
			data.z = (double)child.position.z;
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

	public String ToJson(){
		Construction();
		return JsonUtility.ToJson(holds);
	}


	public void FromJson(string json){
		int max = -1;
		holds = JsonUtility.FromJson<DataArray>(json);
		GameObject hold, target;
		target = null;
		for(int i = 0 ; i < holds.arr.Length ; i++){
			if (holds.arr[i].type == HTYPE_NORMAL){
				target = hold_Normal;
			}else if (holds.arr[i].type == HTYPE_FOOT){
				target = hold_Foot;
			}
			hold = Instantiate(target, Vector3.zero , Quaternion.identity, transform);
			hold.transform.localPosition = new Vector3((float)holds.arr[i].x, (float)holds.arr[i].y, (float)holds.arr[i].z);
			hold.transform.localScale = Vector3.one * (float)holds.arr[i].scale;
			hold.name = holds.arr[i].name;
			if (int.Parse(hold.name) > max){
				max = int.Parse(hold.name);
			}
		}
		GenerateHold2.SetNum(max+1);
	}

	[Serializable]
	public class DataArray{
		public Data[] arr;
	}

	[Serializable]
	public class Data {
		public int id;
		public double x, y, z;
		public double scale;
		public int type;
		public string name;
	}
}


