using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarControl : MonoBehaviour {
	public int incline;
	public enum BODYS{RH=0,RF,RE,RK,LH,LF,LE,LK,BODY};
	public Transform plane;
	// Use this for initialization
	void Start () {
		//incline = 90;
	}
	
	public int GetIncline(){
		return incline;
	}

	//(0, 0)を中心にz軸をincline度だけ傾けた時のz座標を返す
	public float CalcZPos(Vector2 p){
		return -(p.y - plane.localPosition.y) * Mathf.Tan(Mathf.Deg2Rad * (incline-90)) + plane.localPosition.z; 
	}
}
