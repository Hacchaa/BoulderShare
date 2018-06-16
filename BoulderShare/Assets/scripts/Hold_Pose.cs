using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold_Pose : MonoBehaviour {
	public GameObject[] pHolds;
	public HScenes hScenes;

	// Use this for initialization
	void Start () {
		
	}
	
	public void Sync(){
		Hold[] curFocus = hScenes.GetCurHolds();
		Vector3 pivot = Vector3.zero;
		int div = 0;
		int n = 0;

		for(int i = (int)SceneFocus.Choice.RH ; i <= (int)SceneFocus.Choice.LF ; i++){
			if(curFocus[i] != null){
				pHolds[i].SetActive(true);
				pHolds[i].transform.localPosition = curFocus[i].gameObject.transform.localPosition;
				pivot += pHolds[i].transform.localPosition;
				n++;
				//Debug.Log("pos:"+curFocus[i].gameObject.transform.localPosition);
			}else{
				pHolds[i].SetActive(false);
			}
		}

		if (n != 0){
			pivot /= div;
		}

		transform.localPosition = pivot;
	}
}
