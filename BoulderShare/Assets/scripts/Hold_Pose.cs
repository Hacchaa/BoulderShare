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

	public Vector3[] centerArr;
	// Use this for initialization
	void Awake () {
		holdPos = new Vector3[4];
		holdR = new float[4];
	}

	public Vector3[] GetHoldsPos(){
		return holdPos;
	}

	public Vector3 GetHoldPos(int index){
		return holdPos[index];
	}
	public float GetR(int index){
		return holdR[index];
	}

	public void Sync(){
		Hold[] curFocus = hScenes.GetCurHolds();
		Vector3 pivot = Vector3.zero;
		int index = 0;
		float ratio = size.transform.localScale.x;
		Vector3 tmp = Vector3.zero;
		int n = 0;
		//中心座標を探す
		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				Vector3 pos = curFocus[i].gameObject.transform.localPosition;
				holdR[i] = curFocus[i].gameObject.transform.localScale.x / ratio / 2;
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
		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				pHolds[i].SetActive(true);
				holdPos[i] = (curFocus[i].gameObject.transform.localPosition - pivot) / ratio + centerArr[index];
				holdPos[i] += Vector3.forward * ac.CalcZPos(holdPos[i]);
				pHolds[i].transform.localPosition = holdPos[i];
				pHolds[i].transform.localScale = Vector3.one * holdR[i] * 2 ;
			}else{
				pHolds[i].SetActive(false);
				holdPos[i] = Vector3.zero;
				holdR[i] = 0.0f;
			}
		}
	}
}
