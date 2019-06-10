using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttemptTreeView : SEComponentBase{
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private SceneScroll ss;
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private FailedListView failedListView;
	[SerializeField] private GameObject failedListButton;
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private SceneComments3D comments;
	[SerializeField] private Text dimText;
	[SerializeField] private GameObject for3D;
	[SerializeField] private CameraManager cameraManager;

	public void Switch2D3D(){
		Switch2D3D(cameraManager.Is2DActive());
	}
	private void Switch2D3D(bool isActive3D){
		if(isActive3D){
			dimText.text = "3D";
			cameraManager.Active3D();	
			humanModel.LookAtModel(CameraManager.CAMERA3D_DEPTH_DEF);
			for3D.SetActive(true);
		}else{
			dimText.text = "2D";
			cameraManager.Active2D();
			cameraManager.Reset2DCamPosAndDepth();
			for3D.SetActive(false);
		}		
	}
	private void SyncSceneScroll(){
		ss.Delete();
		int index = 0;
		ss.SetTotalNum(hScenes.GetNum());
		ss.Focus(0);
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

	public override void OnPreShow(){
		SyncSceneScroll();
		int index = 0;
		hScenes.SetCurIndex(index);
		HScene2 scene = hScenes.GetCurScene();
		if (scene != null){
			Load(scene);
			ss.Focus(index);
		}
		cameraManager.Active3D();
		cameraManager.Reset2DCamPosAndDepth();
		humanModel.LookAtModel(CameraManager.CAMERA3D_DEPTH_DEF);
		humanModel.HideMarks();
		failedListButton.SetActive(failedListView.IsExist());
		comments.ShowDynamically();
		Switch2D3D(true);
	}

	public override void OnPreHide(){
		hScenes.SetCurIndex(0);
		twoDWallMarks.ClearTouch();
		cameraManager.Reset2DCamPosAndDepth();
		humanModel.InitModelPose();
		scc.Init();
		comments.DontShowAll();
	}

	public void Load(HScene2 scene){
		twoDWallMarks.ClearTouch();
		twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		humanModel.SetModelPose(scene.GetPose(), scene.GetRots(), scene.GetRightHandAnim(), scene.GetLeftHandAnim());
		humanModel.SetCamAxisAsModelPos();
		scc.SetSceneComments(scene.GetComments());
	}

	public void NextComment(){
		comments.Next();
	}
}
