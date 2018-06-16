using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarControl : MonoBehaviour {
	public int incline;
	public Transform leftHand = null;
	public Transform rightHand = null;
	public Transform leftFoot = null;
	public Transform rightFoot = null;
	public enum BODYS{BODY=0,RH,RF,RE,RK,LH,LF,LE,LK};
	// Use this for initialization
	void Start () {
		//incline = 90;
	}
	
	public int GetIncline(){
		return incline;
	}

	//(0, 0)を中心にz軸をincline度だけ傾けた時のz座標を返す
	public float CalcZPos(Vector2 p){
		return -(p.y - 1) * Mathf.Tan(Mathf.Deg2Rad * (incline-90)); 
	}

	public float CalcBodyZPos(Vector2 p){
		//左手と右手のz座標の内小さい方にする
		//return (leftHand.position.z < rightHand.position.z) ? leftHand.position.z : rightHand.position.z;

		if (incline >= 90){
			return (leftHand.position.z + rightHand.position.z) / 2 ;
		}else{
			return (leftFoot.position.z < rightFoot.position.z) ? leftFoot.position.z : rightFoot.position.z;
		}
	}
}
