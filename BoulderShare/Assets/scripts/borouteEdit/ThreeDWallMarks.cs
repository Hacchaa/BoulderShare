using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDWallMarks : MonoBehaviour {
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private GameObject threeDHoldsPrefab;
	[SerializeField]
	private ThreeDWall threeDWall;
	//2dholdと3dholdのサイズ比（2d/3d）
	public static float THREED_MARK_SIZE = 4.8f;
	private Dictionary<string, GameObject> map;

	void Awake(){
		map = new Dictionary<string, GameObject>();
	}

	private void SetLayerRecursively(GameObject self, int layer){
		self.layer = layer;

		foreach(Transform t in self.transform){
			SetLayerRecursively(t.gameObject, layer);
		}
	}

	public void Remove3DMark(string name){
		if (map.ContainsKey(name)){
			GameObject obj = map[name];
			map.Remove(name);
			Destroy(obj);
		}
	}

	public void Make3DMark(float x, float y, float scale, string name){
		Vector3 vec = transform.TransformPoint(new Vector3(x, y, 0.0f));
		vec = threeDWall.CalcWallPoint(vec);
		GameObject obj = Instantiate(threeDHoldsPrefab, transform);
		obj.SetActive(true);
		obj.name = name;
		obj.transform.position = vec;
		obj.transform.localScale = Vector3.one * scale;
		//obj.transform.localRotation = Quaternion.Euler(new Vector3(90.0f, 180.0f, 0.0f));
		SetLayerRecursively(gameObject, LayerMask.NameToLayer("3D"));
		map.Add(name, obj);
	}

	//2dwallのマークを3dwallに同期
	public void Synchronize(){
		foreach(Transform child in twoDWallMarks.transform){
			if (map.ContainsKey(child.gameObject.name)){
				GameObject mark = map[child.gameObject.name];
				mark.transform.localPosition = child.localPosition;
				mark.transform.localScale = child.localScale * THREED_MARK_SIZE;
			}else{
				Make3DMark(child.localPosition.x, child.localPosition.y, child.localScale.x * THREED_MARK_SIZE, child.gameObject.name);
			}
		}

		foreach(Transform child in transform){
			if (!twoDWallMarks.IsMarkExist(child.name)){
				Remove3DMark(child.name);
			}
		}
	}

	public GameObject GetMarkObj(int type){
		string[] arr = twoDWallMarks.GetTouchInfo();

		if (string.IsNullOrEmpty(arr[type])){
			Debug.Log(type + " is null");
			return null;
		}

		Debug.Log(type + " is " + map[arr[type]].name);
		return map[arr[type]];
	}

	public GameObject GetMarkObj(string name){
		if (string.IsNullOrEmpty(name)){
			return null;
		}

		return map[name];
	}
}
