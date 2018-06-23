using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold_Pose : MonoBehaviour {
	public GameObject[] pHolds;
	private Vector3[] holdPos;
	private float[] holdR;
	public HScenes hScenes;
	public float heightRatio = 0.8f;
	public AvatarControl ac;
	private float holdRatio = 0.3f;
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
		int n = 0;
		float max = 0.0f;

		//中心座標を探す
		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				Vector3 pos = curFocus[i].gameObject.transform.localPosition;
				holdR[i] = curFocus[i].gameObject.transform.localScale.x * holdRatio / 2;
				pivot += pos;
				n++;
				float mag = pos.magnitude;
				if (max < mag){
					max = mag;
				}
			}
		}

		if (n != 0){
			pivot /= n;
		}

		//
		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				pHolds[i].SetActive(true);
				holdPos[i] = (curFocus[i].gameObject.transform.localPosition - pivot) / max * heightRatio + center;
				holdPos[i] += Vector3.forward * ac.CalcZPos(holdPos[i]);
				pHolds[i].transform.localPosition = holdPos[i];
				pHolds[i].transform.localScale = Vector3.one * holdR[i] * 2;
			}else{
				pHolds[i].SetActive(false);
			}
		}
	}
}
