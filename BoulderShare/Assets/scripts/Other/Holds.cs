using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holds : MonoBehaviour {
	private bool isInit = false;

	void Awake(){
		isInit = true;
	}

	public bool IsInit(){
		return isInit;
	}
	
	public void InitHolds(){
		foreach (Transform child in transform ){
			GameObject.Destroy(child.gameObject);
		}
	}

	public void SwitchPhase(int phase){
		foreach(Transform child in transform){
			child.GetComponent<Hold>().SwitchPhase(phase);
		}
	}

	public void SwitchVisibility(){
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
