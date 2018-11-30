using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreeDHolds : MonoBehaviour {
	[SerializeField]
	private GameObject threeDHoldsPrefab;
	[SerializeField]
	private GameObject twoDHolds;
	//2dholdと3dholdのサイズ比（2d/3d）
	private static float THREED_HOLD_SIZE = 4.0f;
	private Dictionary<string, GameObject> map;
	[SerializeField]
	private HScenes hScenes;
	[SerializeField]
	private Vector3[] centerPos;
	[SerializeField]
	private Vector3[] offsetFromBody;
	[SerializeField]
	private Vector3[] offsetOnHand;
	private Hold[] curHolds;
	[SerializeField]
	private GameObject threeDModel;
	[SerializeField]
	private AvatarControl ac;

	void Awake(){
		map = new Dictionary<string, GameObject>();
		curHolds = new Hold[4];
	}
	public void Make3DHold(float x, float y, float scale, string name){
		float z = ac.CalcZPos(new Vector2(x, y));
		GameObject obj = Instantiate(threeDHoldsPrefab, transform);
		obj.name = name;
		obj.transform.localPosition = new Vector3(x, y, z);
		obj.transform.localScale = Vector3.one * scale;
		obj.transform.localRotation = Quaternion.Euler(new Vector3(90.0f, 180.0f, 0.0f));
		SetLayerRecursively(gameObject, LayerMask.NameToLayer("3D_GUI"));
		map.Add(name, obj);
	}

	private void SetLayerRecursively(GameObject self, int layer){
		self.layer = layer;

		foreach(Transform t in self.transform){
			SetLayerRecursively(t.gameObject, layer);
		}
	}

	public void Remove3DHold(string name){
		if (map.ContainsKey(name)){
			GameObject obj = map[name];
			map.Remove(name);
			Destroy(obj);
		}
	}

	public void UpdateHolds(){
		foreach(Transform child in twoDHolds.transform){
			if (map.ContainsKey(child.gameObject.name)){
				GameObject hold = map[child.gameObject.name];
				hold.transform.localPosition = child.localPosition;
				hold.transform.localScale = child.localScale * THREED_HOLD_SIZE;
			}else{
				Make3DHold(child.position.x, child.position.y, child.localScale.x * THREED_HOLD_SIZE, child.gameObject.name);
			}
		}
	}

	//現在のホールドの位置とボディの位置を計算して返す
	public Vector3[] CalcBodyPositions(){
		Vector3[] pos = new Vector3[Enum.GetNames(typeof(AvatarControl.BODYS)).Length-1];
		curHolds = hScenes.GetCurHolds();
		int index = 0;
		Vector3 pivot = Vector3.zero;
		int n = 0;
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if(curHolds[i] != null){
				pos[i] = map[curHolds[i].gameObject.name].transform.position;
				//pos[i] = curHolds[i].gameObject.transform.position;
				n++;
				index += (int)Mathf.Pow(2, i);
				Debug.Log(i+", "+pos[i]);
				pos[i] = threeDModel.transform.InverseTransformPoint(pos[i] + GetR(i) * offsetOnHand[i] / 2);
				Debug.Log("before"+ map[curHolds[i].gameObject.name].transform.localPosition);
				Debug.Log("after"+pos[i]);
				pivot += pos[i];
			}
		}
		if (n != 0){
			pivot /= n;
		}
		Debug.Log("index"+index);
		Debug.Log("pivot"+pivot);
		Debug.Log("center"+centerPos[index]);
		pos[(int)AvatarControl.BODYS.BODY] = pivot + centerPos[index];

		//手足位置の微調整
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			//ホールドをつかんでいない手足の位置
			if(curHolds[i] == null){
				pos[i] = offsetFromBody[i] + pos[(int)AvatarControl.BODYS.BODY];
			}
		}
		
		//左右肘膝
		for(int i = (int)AvatarControl.BODYS.RE ; i <= (int)AvatarControl.BODYS.LK ; i++){
				pos[i] = (pos[i-4] + pos[(int)AvatarControl.BODYS.BODY]) / 2;
				pos[i].z = 0.0f;
		}
		return pos;
	}

	public float GetR(int index){
		string name ;
		if (curHolds[index] == null){
			return 0.0f;
		}
		name = curHolds[index].gameObject.name;
		if (map.ContainsKey(name)){
			return map[name].transform.localScale.x / THREED_HOLD_SIZE / 2;
		}else{
			return 0.0f;
		}
	}

	public Vector3 GetPos(int index){
		string name ;
		if (curHolds[index] == null){
			return Vector3.zero;
		}
		name = curHolds[index].gameObject.name;
		if (map.ContainsKey(name)){
			return map[name].transform.position;
		}else{
			return Vector3.zero;
		}
	}
}
