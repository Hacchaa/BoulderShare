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
			child.GetComponent<Hold>().SwitchPhase(phase);
		}
	}

	public void SwitchVisibility(){
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
