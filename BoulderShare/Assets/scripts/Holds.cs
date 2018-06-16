using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holds : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void InitHolds(){
		foreach (Transform child in transform ){
			GameObject.Destroy(child.gameObject);
		}
	}

	public void SwitchPhase(int phase){
		foreach(Transform child in transform){
			if(phase == (int)Observer.Phase.HOLD_EDIT){
				child.Find("Phase2").gameObject.SetActive(false);
			}else if(phase == (int)Observer.Phase.SCENE_EDIT){
				child.Find("Hold_Scale").gameObject.SetActive(false);
				child.Find("Phase2").gameObject.SetActive(true);
			}
		}
	}
}
