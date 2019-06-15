using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ATFailureView : SEComponentBase{
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private SceneScroll ss;
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private FailedListView failedListView;
	[SerializeField] private GameObject failedListButton;
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private Text dimText;
	[SerializeField] private GameObject for3D;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private List<GameObject> failureObjects;
	[SerializeField] private FailureCommentController fcc;
	[SerializeField] private WallManager wallManager;

	public void AddFailureComment(){
		string inputText = inputField.text;

		if (!string.IsNullOrEmpty(inputText)){
			hScenes.GetCurScene().AddFailureComment(inputText);
		}
	}
	public void OpenFailureFrame(){
		foreach(GameObject obj in failureObjects){
			obj.SetActive(true);
		}
		inputField.text = "";
	}
	public void CloseFailureFrame(){
		foreach(GameObject obj in failureObjects){
			obj.SetActive(false);
		}
		Load(hScenes.GetCurScene());
	}

	public void ToATMenu(){
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}

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
		//failedListButton.SetActive(failedListView.IsExist());
		scc.ShowDynamically();
		Switch2D3D(true);
		wallManager.ShowTranslucentWall();
	}

	public override void OnPreHide(){
		hScenes.SetCurIndex(0);
		twoDWallMarks.ClearTouch();
		cameraManager.Reset2DCamPosAndDepth();
		humanModel.InitModelPose();
		scc.Init();
		scc.DontShowAll();
	}

	public void Load(HScene2 scene){
		twoDWallMarks.ClearTouch();
		twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		humanModel.SetModelPose(scene.GetPose(), scene.GetRots(), scene.GetRightHandAnim(), scene.GetLeftHandAnim());
		humanModel.SetCamAxisAsModelPos();
		scc.SetSceneComments(scene.GetComments());
		fcc.SetFailureComments(scene.GetFailureList());
	}

	public void NextComment(){
		scc.Next();
	}
}
