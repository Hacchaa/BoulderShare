using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView: SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private HScenes2 hScenes2;
	[SerializeField] private GameObject forEdit;
	[SerializeField] private SceneSelectView ssView;
	public override void OnPreShow(){
		cameraManager.Active2D();
		cameraManager.Reset2DCamPosAndDepth();
		if (hScenes2.GetNum() == 0){
			forEdit.SetActive(false);
		}else{
			forEdit.SetActive(true);
		}
	}

	public override void OnPreHide(){

	}

	public void ToLayerGraph(){
		trans.Transition(ScreenTransitionManager.Screen.LayerGraphView);
	}

	public void ToMakeAT(){
		makeAT.Init();
		makeAT.SetMode(MakeAttemptTree.Mode.Loop);
		trans.Transition(ScreenTransitionManager.Screen.EditWallMark);
	}

	public void ToPost(){
		trans.Transition(ScreenTransitionManager.Screen.Post);
	}

	public void ToTry(){
		trans.Transition(ScreenTransitionManager.Screen.TryView);
	}

	public void To3DSetting(){
		trans.Transition(ScreenTransitionManager.Screen.ThreeDFirstSettingView);
	}
}
