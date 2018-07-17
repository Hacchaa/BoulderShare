using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarControl : MonoBehaviour {
	private int incline;
	public enum BODYS{RH=0,RF,RE,RK,LH,LF,LE,LK,BODY};
	public Transform plane;
	public TransformObj[] tObjs;

	// Use this for initialization
	void Start () {
		incline = 90;
		SetIncline(incline);
	}
	
	public void Init(){
		for (int i = 0; i < tObjs.Length ; i++){
			tObjs[i].SetFixed(false);
		}
	}
	public int GetIncline(){
		return incline;
	}

	public void SetIncline(int value){
		incline = value;
		plane.localRotation = Quaternion.Euler(-value, 0, 0);
	}

	//(0, 0)を中心にz軸をincline度だけ傾けた時のz座標を返す
	public float CalcZPos(Vector2 p){
		if (incline == 90){
			return plane.localPosition.z; 
		}
		return -(p.y - plane.localPosition.y) * Mathf.Tan(Mathf.Deg2Rad * (incline-90)) + plane.localPosition.z; 
	}
}
