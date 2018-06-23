using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Observer : MonoBehaviour {
	public Camera curCamera;
	private GameObject focusObj;
	public const float WALL_W = 6.0f;
	public const float WALL_H = 4.0f;
	public static int currentPhase = 1;
	public static int FINGER_NONE = -10;
	public enum Phase{HOLD_EDIT=1, SCENE_EDIT};
	public Holds holds;
	public HScenes hScenes ;
	public GameObject[] phaseArr;

	void Start(){
		currentPhase = 1;
	}

	public void InitHoldsAndScenes(){
		holds.InitHolds();
		hScenes.InitScenes();
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

		phaseArr[currentPhase-1].SetActive(false);
		phaseArr[phase-1].SetActive(true);

		//GameObject.Find("Phase"+currentPhase).SetActive(false);
		//GameObject.Find("Phase"+phase).SetActive(true);
		currentPhase = phase;

		holds.SwitchPhase(phase);
	}
}
