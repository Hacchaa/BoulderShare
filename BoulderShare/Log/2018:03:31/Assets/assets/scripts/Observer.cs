using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour {
	public Camera camera;
	private GameObject focusObj;

	void Start(){
		focusObj = null;
	}

	public Camera GetCamera(){
		return camera;
	}

	public void FocusObject(GameObject obj){
		if (focusObj != null){
			focusObj.SetActive(false);
		}

		obj.SetActive(true);
		focusObj = obj;
	}

	public GameObject GetFocusObject(){
		return focusObj;
	}

	public void ReleaseFocus(){
		if (focusObj != null){
			focusObj.SetActive(false);
			focusObj = null;
		}
	}
}
