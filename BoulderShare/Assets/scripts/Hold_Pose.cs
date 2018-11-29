using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold_Pose : MonoBehaviour {
	public GameObject[] pHolds;
	private Vector3[] holdPos;
	private float[] holdR;
	public HScenes hScenes;
	public AvatarControl ac;
	public GameObject size;
	private static float rateFrom2DTo3D = 0.25f;
	private static float threeDSize = 0.25f;

	public  Vector3[] centerArr;
	// Use this for initialization
	void Awake () {
		holdPos = new Vector3[4];
		holdR = new float[4];
	}

	public Vector3[] GetHoldsPosa(){
		return holdPos;
	}

	public Vector3 GetHoldPosa(int index){
		return holdPos[index];
	}
	public float Geta(int index){
		return holdR[index] * threeDSize / 2;
	}

	public void Synca(){
		Hold[] curFocus = hScenes.GetCurHolds();
		Vector3 pivot = Vector3.zero;
		int index = 0;
		float ratio = size.transform.localScale.x;
		Vector3 tmp = Vector3.zero;
		int n = 0;
		//中心座標を探す
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if(curFocus[i] != null){
				Vector3 pos = curFocus[i].gameObject.transform.localPosition;
				holdR[i] = curFocus[i].gameObject.transform.localScale.x / ratio / rateFrom2DTo3D ;
				pivot += pos;
				n++;
				index += (int)Mathf.Pow(2, i);
			}
		}
		//Debug.Log("index:"+index);
		//Debug.Log("arr:"+centerArr[index]);
		if (n != 0){
			pivot /= n;
		}
		//
		for(int i = (int)AvatarControl.BODYS.RH ; i <= (int)AvatarControl.BODYS.LF ; i++){
			if(curFocus[i] != null){
				pHolds[i].SetActive(true);
				holdPos[i] = (curFocus[i].gameObject.transform.localPosition - pivot) / ratio + centerArr[index];
				holdPos[i] = curFocus[i].gameObject.transform.localPosition;
				holdPos[i] += Vector3.forward * ac.CalcZPos(holdPos[i]);
				pHolds[i].transform.localPosition = holdPos[i];
				pHolds[i].transform.localScale = Vector3.one * holdR[i];
			}else{
				pHolds[i].SetActive(false);
				holdPos[i] = Vector3.zero;
				holdR[i] = 0.0f;
			}
		}
	}
}
