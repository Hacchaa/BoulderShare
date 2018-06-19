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
}
