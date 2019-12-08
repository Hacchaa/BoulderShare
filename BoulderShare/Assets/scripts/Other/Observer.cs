using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Observer : MonoBehaviour {
	public Camera curCamera;
	private GameObject focusObj;
	public static int currentPhase = 1;
	public static int FINGER_NONE = -10;
	public enum Phase{HOLD_EDIT=0, SCENE_EDIT};
	public Holds holds;
	public HScenes hScenes ;
	public GameObject[] phaseArr;
	public ThreeDModel threeDModel;
	public Phase1 phase1;
	public const string WALLPATH = "/Wall.png";
	public const string ROUTEPATH = "/route/";
	public BoRouteLSManager bManager;
	[SerializeField]
	private SpriteRenderer wallImg;

	void Awake(){
		currentPhase = 0;
	}

	void Start(){
		HScene.SetNum(0);
		bManager.LoadBoRoute();
		if (bManager.IsLoaded()){
			bManager.BoRouteLoadFirst();
		}
		phase1.FirstProc();
	}
/*
	public void InitHoldsAndScenes(){
		holds.InitHolds();
		hScenes.InitScenes();
	}*/

	public Bounds GetWallBounds(){
		return wallImg.bounds;
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

	public void ReleaseFocus(){
		if (focusObj != null){
			focusObj.SetActive(false);
			focusObj = null;
		}
	}

	public void SwitchPhase(int phase){
		ReleaseFocus();
/*
		for(int i = 0 ; i < phaseArr.Length ; i++){
			if (i == phase-1){
				phaseArr[i].SetActive(true);
			}else{
				phaseArr[i].SetActive(false);
			}
		}*/
		phase1.SwitchSubMenu((int)Phase1.TYPE.DEFAULT);

		if(phase == (int)Observer.Phase.HOLD_EDIT){
			phaseArr[(int)Observer.Phase.HOLD_EDIT].SetActive(true);
		}else{
			phaseArr[(int)Observer.Phase.HOLD_EDIT].SetActive(false);
		} 

		if(phase == (int)Observer.Phase.SCENE_EDIT){
			phaseArr[(int)Observer.Phase.SCENE_EDIT].SetActive(true);
			threeDModel.ChangeMode((int)ThreeDModel.Mode.WINDOW);
		}else{
			phaseArr[(int)Observer.Phase.SCENE_EDIT].SetActive(false);
			threeDModel.ChangeMode((int)ThreeDModel.Mode.DEFAULT);
		} 
		//GameObject.Find("Phase"+currentPhase).SetActive(false);
		//GameObject.Find("Phase"+phase).SetActive(true);
		currentPhase = phase;

		holds.SwitchPhase(phase);
	}
}
