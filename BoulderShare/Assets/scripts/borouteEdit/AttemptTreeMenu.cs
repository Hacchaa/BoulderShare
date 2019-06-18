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
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private Text dimText;
	[SerializeField] private GameObject for3D;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private EditorPopup popup;
	[SerializeField] private List<GameObject> forView;
	[SerializeField] private List<GameObject> forMenu;
	[SerializeField] private List<GameObject> forFailure;
	[SerializeField] private List<GameObject> failureObjects;
	[SerializeField] private List<GameObject> forEmpty;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private ModifyMarks modifyMarks;
	[SerializeField] private ATWarning atWarning;

	[SerializeField] private Slider sceneSlider;

	private string warningWithRemove = "現在のシーンを削除しますか？";

	public void UpdateWarning(Dictionary<string, int> modMap){

		foreach(HScene2 scene in hScenes.GetScenes()){

			string[] hold = scene.GetOnHolds();
			int[] types = scene.GetWarningType();

			//Debug.Log("scene.name:"+scene.GetID());
			for(int i = 0 ; i < hold.Length ; i++){
				//Debug.Log("i="+i);
				if (!string.IsNullOrEmpty(hold[i]) && modMap.ContainsKey(hold[i])){
					if (types[i] != -1){
						types[i] = types[i] | modMap[hold[i]];
					}else{
						types[i] = modMap[hold[i]];
					}
					Debug.Log("type "+types[i]);
				}
			}
			scene.SetWarningType(types);
		}
	}

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

	public void SaveAT(){
		hScenes.RegistCurHScenes();
		hScenes.InitAT();
		ToMainView();
	}

	public void AddFirst(){
		makeAT.Init();
		trans.Transition(ScreenTransitionManager.Screen.SceneEditor);
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
		popup.Open(Remove, null, warningWithRemove,"", "削除", "キャンセル");
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
		}else{/*
			hScenes.SetCurIndex(0);
			Load(hScenes.GetCurScene());
			ss.SetTotalNum(hScenes.GetNum()-1);
			ss.Focus(0);
			cameraManager.Reset2DCamPosAndDepth();*/
		}
		hScenes.RemoveScene();

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
		int n = hScenes.GetNum();
		ss.Delete();
		int index = 0;
		ss.SetTotalNum(n);
		ss.Focus(0);

		sceneSlider.minValue = 1;
		sceneSlider.maxValue = n;
		sceneSlider.value = 1;
	}

	public void NextScene(){
		int index = hScenes.GetCurIndex();
		ShowSceneAt(index + 1);
	}

	public void PrevScene(){
		int index = hScenes.GetCurIndex();
		ShowSceneAt(index - 1);
	}

	public void ChangeSceneSliderVal(float val){
		ShowSceneAt((int)val-1);
	}

	public void ShowSceneAt(int index){
		//Debug.Log("show at " + index);
		HScene2 scene = hScenes.GetScene(index);

		if (scene == null){
			return ;
		}
		hScenes.SetCurIndex(index);
		Load(scene);
		ss.Focus(index);
		sceneSlider.value = index + 1;
	}

	public void ShowFirstScene(){
		ShowSceneAt(0);
	}

	public void ShowLastScene(){
		//Debug.Log("hSCenes.Getnum "+hScenes.GetNum());
		ShowSceneAt(hScenes.GetNum() - 1);
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

		scc.ShowDynamically();
		Switch2D3D(true);

		ActivateList(forView, false);
		ActivateList(forMenu, false);
		ActivateList(forFailure, false);
		ActivateList(forEmpty, false);

		switch(mode){
			case Mode.View: ActivateList(forView, true); break;
			case Mode.Menu:
				if (hScenes.IsATEmpty()){
					ActivateList(forEmpty, true);
				}else{
					ActivateList(forMenu, true); 
				}
				break;
			case Mode.Failure: ActivateList(forFailure, true); break;
			default: ActivateList(forMenu, true); break;
		}
		wallManager.ShowTranslucentWall();
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
		scc.DontShowAll();

		ActivateList(forView, false);
		ActivateList(forMenu, false);
		ActivateList(forFailure, false);
	}

	public void Load(HScene2 scene){
		twoDWallMarks.ClearTouch();
		twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		humanModel.SetModelPose(scene.GetPose(), scene.GetRots(), scene.GetRightHandAnim(), scene.GetLeftHandAnim());
		humanModel.SetCamAxisAsModelPos();
		scc.SetSceneComments(scene.GetComments());

		//ワーニングテキストがある場合、表示
		if(scene.HasWarning()){
			atWarning.gameObject.SetActive(true);

			string title = "このシーンで使用しているマークが変更されました。";
			string sup = "";
			int[] types = scene.GetWarningType();
			for(int i = 0 ; i < types.Length ; i++){
				if (types[i] != -1){
					string line = "";
					if (i == (int)TwoDMark.HFType.RH){
						line += "右手のマーク";
					}else if (i == (int)TwoDMark.HFType.LH){
						line += "左手のマーク";
					}else if (i == (int)TwoDMark.HFType.RF){
						line += "右足のマーク";
					}else if (i == (int)TwoDMark.HFType.LF){
						line += "左足のマーク";
					}
					if (!string.IsNullOrEmpty(sup)){
						sup += "\r\n";
					}
					sup += line + modifyMarks.GetWarningTailText(types[i]);
				}
			}
			atWarning.SetWarning(title, sup);
		}else{
			atWarning.gameObject.SetActive(false);
			atWarning.Clear();
		}
	}

	public void OpenWarning(){
		atWarning.OpenWarning();
	}

	public void NextComment(){
		scc.Next();
	}
}
