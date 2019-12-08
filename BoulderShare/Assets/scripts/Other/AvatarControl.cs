using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarControl : MonoBehaviour {
	private int incline ;
	public enum BODYS{NONE=-1,RH,LH,RF,LF,RE,LE,RK,LK,BODY};
	public Transform inclineObj;
	public TransformObj[] acObjs;
	public Transform lightObj;
	private const float offset = 90.0f;
	// Use this for initialization
	void Start () {
		if (inclineObj != null){
			SetIncline(90);
		}
	}
	
	public void Init(){
		for (int i = 0; i < acObjs.Length ; i++){
			if (acObjs[i] != null){
				acObjs[i].SetFixed(false);
			}
		}
	}

	public bool IsFixed(int t){
		return acObjs[t].IsFixed();
	}

	public void SetFixed(int t, bool b){
		acObjs[t].SetFixed(b);
	}

	public Vector3 GetPos(int index){
		return acObjs[index].GetTargetPos();
	}

	public int GetIncline(){
		return incline;
	}

	public void SetIncline(int value){
		incline = value;
		inclineObj.localRotation = Quaternion.Euler(-value + offset, 0, 0);
		lightObj.localRotation = Quaternion.Euler(140-value, 0, 0);
	}

	//(0, 0)を中心にz軸をincline度だけ傾けた時のz座標を返す
	public float CalcZPos(Vector2 p){
		if (incline == 90){
			return inclineObj.localPosition.z; 
		}
		return -(p.y - inclineObj.localPosition.y) * Mathf.Tan(Mathf.Deg2Rad * (incline-90)) + inclineObj.localPosition.z; 
	}
}
