using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
	public delegate void CloseFunc();
	private CloseFunc func;
	private bool touchable;

	void Awake(){
		touchable = true;
	}
	public void Open(CloseFunc cf){
		gameObject.SetActive(true);
		func = new CloseFunc(cf);
	}

	public void Close(){
		if (touchable){
			gameObject.SetActive(false);
			func();
		}
	}

	public void OpenIgnoreTouch(){
		gameObject.SetActive(true);
		touchable = false;
	}

	public void CloseIgnoreTouch(){
		touchable = true;
		gameObject.SetActive(false);
	}
}
