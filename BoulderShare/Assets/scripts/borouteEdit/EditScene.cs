﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditScene : MonoBehaviour, IUIComponent {
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private AttemptTreeView atv;
	[SerializeField]
	private GameObject twoDCamera;
	[SerializeField]
	private ThreeD threeD;

	private HScene2 curScene;
	private Vector3[] curPose;
	private Quaternion[] curRotate;

	[SerializeField]
	private bool isPoseDetermined = false;
	private bool isCurLookingAct = false;

	void Awake(){
	}

	public void DeleteScene(){
		if (curScene != null){
			atv.RemoveScene();
		}
		Close();
	}


	public void SwitchDimension(){
		trans.Transition("EditPose");
	}

	public Vector3[] GetPose(){
		return curPose;
	}

	public Quaternion[] GetRotate(){
		return curRotate;
	}

	public void SetPose(Vector3[] pose){
		curPose = pose;
		isPoseDetermined = true;
	}

	public void SetRotate(Quaternion[] rot){
		curRotate = rot;
		isPoseDetermined = true;
	}

	public bool IsCurLookingActivate(){
		return isCurLookingAct;
	}

	public void SetIsCurLookingActivate(bool b){
		isCurLookingAct = b;
	}

	public bool IsPoseDetermined(){
		return isPoseDetermined;
	}

	public void Submit(){
		if (curScene == null){
			curScene = new HScene2();
			curScene.SetOnHolds(twoDWallMarks.GetTouchInfo());

			if (isPoseDetermined){
				curScene.SavePose(curPose);
				curScene.SavePRotate(curRotate);
				curScene.SetIsLookingActivate(isCurLookingAct);
			}else{
				curScene.SavePose(threeD.GetModelPosition());
				curScene.SavePRotate(threeD.GetModelRotation());
				curScene.SetIsLookingActivate(isCurLookingAct);
			}
			atv.AddScene(curScene);
		}else{
			curScene.SetOnHolds(twoDWallMarks.GetTouchInfo());

			if (isPoseDetermined){
				curScene.SavePose(curPose);
				curScene.SavePRotate(curRotate);
				curScene.SetIsLookingActivate(isCurLookingAct);
			}else{
				curScene.SavePose(threeD.GetModelPosition());
				curScene.SavePRotate(threeD.GetModelRotation());
				curScene.SetIsLookingActivate(isCurLookingAct);
			}
		}
		Close();
	}

	public void Close(){
		isPoseDetermined = false;
		isCurLookingAct = false;
		curScene = null;
		twoDWallMarks.ClearTouch();
		trans.Transition("AttemptTreeView");
	}

	public void ShowProc(){
		int t = AttemptTreeView.GetSceneType();

		if(!isPoseDetermined && t == (int)AttemptTreeView.SCENETYPE.EDIT){
			Debug.Log("1");
			curScene = atv.GetCurScene();
			if (curScene != null){
				Debug.Log("2");
				twoDWallMarks.SetTouchInfo(curScene.GetOnHolds());
				curPose = curScene.GetPose();
				curRotate = curScene.GetPRotate();
				isCurLookingAct = curScene.IsLookingActivate();
				isPoseDetermined = true;
			}
		}
		threeD.InitModelPose();

		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		twoDCamera.SetActive(true);
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}

		twoDCamera.SetActive(false);
	}

}