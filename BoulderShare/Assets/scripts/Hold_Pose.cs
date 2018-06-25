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
		Vector3 center = new Vector3(0.0f, 0.6f, 0.0f);
		float ratio = size.transform.localScale.x;
		Vector3 tmp = Vector3.zero;
		int n = 0, m = 0;
		//中心座標を探す
		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				Vector3 pos = curFocus[i].gameObject.transform.localPosition;
				holdR[i] = curFocus[i].gameObject.transform.localScale.x / ratio / 2;
				pivot += pos;
				n++;
			}else{
				if (i == (int)SceneFocus.Choice.RH){
					tmp += new Vector3(0.4f, 1.2f, 0.0f);
				}else if (i == (int)SceneFocus.Choice.RF){
					tmp += new Vector3(0.4f, 0.15f, 0.0f);
				}else if (i == (int)SceneFocus.Choice.LH){
					tmp += new Vector3(-0.4f, 1.2f, 0.0f);
				}else if (i == (int)SceneFocus.Choice.LF){
					tmp += new Vector3(-0.4f, 0.15f, 0.0f);
				}
				m++;
			}
		}

		if (n != 0){
			pivot /= n;
		}
		if (m != 0){
			pivot += tmp / m;
		}

		//
		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				pHolds[i].SetActive(true);
				holdPos[i] = (curFocus[i].gameObject.transform.localPosition - pivot) / ratio + center;
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
