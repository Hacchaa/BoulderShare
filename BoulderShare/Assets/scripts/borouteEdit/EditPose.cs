using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EditPose : SEComponentBase{
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private HScenes2 hScenes;
	[SerializeField]
	private HumanModel humanModel;
	[SerializeField] private MakeAttemptTree makeAT;
	//[SerializeField] private FaceObjSelector foSelector;
	[SerializeField] private Transform copyModel;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private VRIKController vrIK;
	private bool isShadowLoaded = false;

	private int index = -1;

	public void ToEditComment(){
		makeAT.SetPose(humanModel.GetModelPosition(),humanModel.GetModelRotation());
		trans.Transition(ScreenTransitionManager.Screen.Edit3DSceneComment);
	}
	public void ToEditScene(){
		makeAT.SetPose(humanModel.GetModelPosition(),humanModel.GetModelRotation());
		trans.Transition(ScreenTransitionManager.Screen.EditScene);		
	}
	public void ToATV(){
		makeAT.SetPose(humanModel.GetModelPosition(),humanModel.GetModelRotation());
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.View;
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);		
	}
	public void To3DSetting(){
		makeAT.SetPose(humanModel.GetModelPosition(),humanModel.GetModelRotation());
		trans.Transition(ScreenTransitionManager.Screen.ThreeDFirstSettingView);			
	}

	public void CopyJustBeforeScene(){
		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);

		if (scene != null){
			humanModel.SetModelPose(scene.GetPose(), scene.GetRots());
		}
	}

	public void CorrectModelPose(){
		humanModel.CorrectModelPose();
	}

	public override void OnPreShow(){
		cameraManager.Active3D();
		twoDWallMarks.SetTouchInfo(makeAT.Get2DTouchMarks());
		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);
		isShadowLoaded = false;
		if (scene != null){
			humanModel.CopyModelPose(copyModel, scene.GetPose(), scene.GetRots());	
			isShadowLoaded = true;
		}

		if(makeAT.IsPoseSet()){
			humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
		}else{
			humanModel.CorrectModelPose();
		}
		humanModel.ShowMarks();
		humanModel.LookAtModel();
		//foSelector.Init();
		index = -1;
		copyModel.gameObject.SetActive(false);
	}
	
	public override void OnPreHide(){
		twoDWallMarks.ClearTouch();
		humanModel.InitModelPose();
		//foSelector.Init();
		isShadowLoaded = false;
		index = -1;
		copyModel.gameObject.SetActive(false);
	}

	public void SwitchShadow(){
		if(isShadowLoaded){
			copyModel.gameObject.SetActive(!copyModel.gameObject.activeSelf);
		}
	}
/*
	public void SelectFO(){
		int n = Enum.GetNames(typeof(VRIKController.FullBodyMark)).Length - 1;
		index++;
		if (index > n){
			index = 0;
			//foSelector.Release();
			return ;
		}
		while(!vrIK.IsFaceObj((VRIKController.FullBodyMark)index)){
			index++;
			if (index > n){
				index = 0;
				//foSelector.Release();
				return ;
			}
		}
		//foSelector.Regist((VRIKController.FullBodyMark)index);
	}*/
}
