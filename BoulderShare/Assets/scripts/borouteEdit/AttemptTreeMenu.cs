using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttemptTreeMenu : SEComponentBase{
	public enum Mode {View, Menu, Failure};
	public static Mode mode;
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
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private EditorPopup popup;
	[SerializeField] private List<GameObject> forView;
	[SerializeField] private List<GameObject> forMenu;
	[SerializeField] private List<GameObject> forFailure;
	[SerializeField] private List<GameObject> failureObjects;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private FailureCommentController fcc;

	private string warningText = "本当に削除しますか？";

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
	public void GoBackScreen(){
		trans.Back();
	}

	public void ToEdit(){
		makeAT.Init();
		makeAT.SetMode(MakeAttemptTree.Mode.Edit);
		makeAT.SetIndex(hScenes.GetCurIndex());
		makeAT.LoadScene(hScenes.GetCurScene());
		trans.Transition(ScreenTransitionManager.Screen.SceneEditor);
	}
	public void ToModifyMarks(){
		trans.Transition(ScreenTransitionManager.Screen.ModifyMarks);
	}

	public void ToATMenu(){
		mode = Mode.Menu;
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}

	public void ToTry(){
		trans.Transition(ScreenTransitionManager.Screen.TryView);
	}

	public void SaveAT(){
		hScenes.RegistCurHScenes();
		ToMainView();
	}

	public void AddPrev(){
		makeAT.Init();
		makeAT.SetMode(MakeAttemptTree.Mode.Add);
		makeAT.SetIndex(hScenes.GetCurIndex());
		trans.Transition(ScreenTransitionManager.Screen.SceneEditor);		
	}
	public void AddNext(){
		makeAT.Init();
		makeAT.SetMode(MakeAttemptTree.Mode.Add);
		makeAT.SetIndex(hScenes.GetCurIndex()+1);
		trans.Transition(ScreenTransitionManager.Screen.SceneEditor);		
	}
	public void ToRemove(){
		popup.Open(Remove, null, warningText,"", "削除", "キャンセル");
	}

	public void ToMainView(){
		hScenes.InitAT();
		trans.Transition(ScreenTransitionManager.Screen.MainView);
	}

	public void Remove(){
		int n = hScenes.GetNum();
		if(n == 0){
			return ;
		}else if(n == 1){
			twoDWallMarks.ClearTouch();
			humanModel.InitModelPose();
			ss.Delete();			
		}else{
			hScenes.SetCurIndex(0);
			Load(hScenes.GetCurScene());
			ss.SetTotalNum(hScenes.GetNum()-1);
			ss.Focus(0);
			cameraManager.Reset2DCamPosAndDepth();
		}
		hScenes.RemoveScene();
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
		comments.ShowDynamically();
		Switch2D3D(true);

		ActivateList(forView, false);
		ActivateList(forMenu, false);
		ActivateList(forFailure, false);

		switch(mode){
			case Mode.View: ActivateList(forView, true); break;
			case Mode.Menu: ActivateList(forMenu, true); break;
			case Mode.Failure: ActivateList(forFailure, true); break;
			default: ActivateList(forMenu, true); break;
		}
	}

	private void ActivateList(List<GameObject> list, bool b){
		foreach(GameObject obj in list){
			obj.SetActive(b);
		}
	}

	public override void OnPreHide(){
		hScenes.SetCurIndex(0);
		twoDWallMarks.ClearTouch();
		cameraManager.Reset2DCamPosAndDepth();
		humanModel.InitModelPose();
		scc.Init();
		comments.DontShowAll();

		ActivateList(forView, false);
		ActivateList(forMenu, false);
		ActivateList(forFailure, false);
	}

	public void Load(HScene2 scene){
		twoDWallMarks.ClearTouch();
		twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		humanModel.SetModelPose(scene.GetPose(), scene.GetRots());
		humanModel.SetCamAxisAsModelPos();
		scc.SetSceneComments(scene.GetComments());
		fcc.SetFailureComments(scene.GetFailureList());
	}

	public void NextComment(){
		comments.Next();
	}
}
