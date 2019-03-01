﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttemptTreeView : SEComponentBase{
	[SerializeField]private HScenes2 hScenes;
	[SerializeField]private SceneScroll ss;
	[SerializeField]private List<GameObject> externalUIComponents;
	[SerializeField]private ScreenTransitionManager trans;
	[SerializeField]private TwoDWallMarks twoDWallMarks;
	[SerializeField]private GameObject twoDCamera;
	[SerializeField]private GameObject threeDCamera;
	[SerializeField]private GameObject ac;
	[SerializeField]private TwoDWallImage twoDWallImage;
	[SerializeField] private ThreeD threeD;
	[SerializeField] private FailedListView failedListView;
	[SerializeField] private GameObject failedListButton;
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private SceneComments3D comments;
	[SerializeField] private CameraUtility threeDCameraUtility;

	public enum SCENETYPE{EDIT = 0, ADD};
	private static int sceneType = -1;


	private void SyncSceneScroll(){
		ss.Delete();
		int index = 0;
		ss.SetTotalNum(hScenes.GetNum());
		ss.Focus(0);
	}
/*
	public void AddScene(HScene2 scene){
		hScenes.AddScene(scene);

		int index = hScenes.GetCurIndex();

		ss.Add(index);
		ss.Focus(index);
	}

	public void AddSceneLast(HScene2 scene){
		hScenes.AddSceneLast(scene);
		int index = hScenes.GetCurIndex();

		ss.Add(index);
		ss.Focus(index);
	}

	public HScene2 GetCurScene(){
		return hScenes.GetCurScene();
	}
	public int GetCurSceneIndex(){
		return hScenes.GetCurIndex();
	}

	public HScene2 GetScene(int index){
		return hScenes.GetScene(index);
	}*/

	public void RemoveScene(){
		int index = hScenes.GetCurIndex();
		ss.Remove(index);
		hScenes.RemoveScene();
		index = hScenes.GetCurIndex();
		ss.Focus(index);
	}

	public void NextScene(){
		HScene2 scene = hScenes.NextScene();
		int index = hScenes.GetCurIndex();
		if (index >= 0){
			Load(scene);
			ss.Focus(index);
		}
	}

	public void PrevScene(){
		HScene2 scene = hScenes.PrevScene();
		int index = hScenes.GetCurIndex();
		if (index >= 0){
			Load(scene);
			ss.Focus(index);
		}
	}

	public override void ShowProc(){
		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		SyncSceneScroll();
		HScene2 scene = hScenes.GetCurScene();
		int index = hScenes.GetCurIndex();
		if (index >= 0){
			Load(scene);
			ss.Focus(index);
		}

		twoDWallImage.ResetCamPosAndDepth();
		twoDCamera.SetActive(false);
		threeD.LookAtModel();
		threeDCameraUtility.SetActive(true);
		ac.SetActive(false);

		failedListButton.SetActive(failedListView.IsExist());
		comments.ShowDynamically();
	}

	public override void HideProc(){
		twoDWallMarks.ClearTouch();
		twoDWallImage.ResetCamPosAndDepth();
		threeD.InitModelPose();
		scc.Init();
		comments.DontShowAll();
		Hide();
	}

	public override void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		threeDCameraUtility.SetActive(false);
		twoDCamera.SetActive(false);
		threeDCamera.SetActive(false);
	}

	public static int GetSceneType(){
		return sceneType;
	}

	public void Load(HScene2 scene){
		Debug.Log("load");
		twoDWallMarks.ClearTouch();
		twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		threeD.SetModelPose(scene.GetPose(), scene.GetRots());
		threeD.LookAtModel();
		scc.SetSceneComments(scene.GetComments());
	}

	public void NextComment(){
		comments.Next();
	}
}
