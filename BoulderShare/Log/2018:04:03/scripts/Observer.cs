using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour {
	public Camera curCamera;
	private GameObject focusObj;
	public static float CAMERA_DEPTH_LL = 1.2f;
	public static float CAMERA_DEPTH_UL = 12.0f;
	public static float WALL_W = 6.0f;
	public static float WALL_H = 4.0f;

	void Start(){
		focusObj = null;
	}

	public Camera GetCamera(){
		return curCamera;
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
