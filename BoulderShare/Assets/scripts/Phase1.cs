using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase1 : MonoBehaviour {
	public GameObject model;
	public GameObject slider;
	public GameObject holdOpe;
	public enum TYPE{HOLDOPERATION=0, MODELSIZE};
	public static int curType = 0;
	public Observer observer;
	// Use this for initialization
	void Start () {
		
	}

	public void SwitchSubMenu(int type){
		if (type == (int)TYPE.HOLDOPERATION){
			model.SetActive(false);
			slider.SetActive(false);
			holdOpe.SetActive(true);
		}else if (type == (int)TYPE.MODELSIZE){
			model.SetActive(true);
			slider.SetActive(true);
			holdOpe.SetActive(false);
		}
		curType = type;
		observer.ReleaseFocus();
	}
	
	public void SliderOpe(float val){
		model.transform.localScale = Vector3.one * val;
	}
}
