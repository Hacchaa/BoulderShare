using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phase1 : MonoBehaviour {
	public GameObject model;
	public GameObject slider;
	public GameObject holdOpe;
	public GameObject inclineObj;
	public ThreeDModel threeDModel;
	public enum TYPE{DEFAULT=-1,HOLDOPERATION, MODELSIZE, INCLINE};
	public static int curType = 0;
	public Observer observer;
	public Slider msSlider;
	private Camera cam;
	public TransformWall3 wall;
	// Use this for initialization
	void Start () {
		cam = observer.GetCamera();
	}

	public void SwitchSubMenu(int type){
		if (type == (int)TYPE.HOLDOPERATION){
			holdOpe.SetActive(true);
		}else{
			holdOpe.SetActive(false);
		}
		
		if (type == (int)TYPE.MODELSIZE){
			model.SetActive(true);
			slider.SetActive(true);
			threeDModel.ChangeMode((int)ThreeDModel.Mode.MODEL_SIZE);
			//カメラを動かせなくして、正面に配置する
			wall.ResetCamPosAndDepth();
			wall.IgnoreTouch(true);
		}else{
			model.SetActive(false);
			slider.SetActive(false);
			threeDModel.ChangeMode((int)ThreeDModel.Mode.DEFAULT);
			wall.IgnoreTouch(false);
		}
		
		if (type == (int)TYPE.INCLINE){
			inclineObj.SetActive(true);
		}else{
			inclineObj.SetActive(false);
		}
		curType = type;
		observer.ReleaseFocus();
	}
	
	public void SetModelSizeSlider(float val){
		model.transform.localScale = Vector3.one * val;
	}

	public void SetModelSize(float val){
		model.transform.localScale = Vector3.one * val;
		msSlider.value = val;
	}

	public float GetModelSize(){
		return model.transform.localScale.x;
	}
}
