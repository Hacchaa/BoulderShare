using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EditPose : SEComponentBase{
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private HScenes2 hScenes;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private FaceObjSelector foSelector;
	[SerializeField] private Transform copyModel;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	private bool isShadowLoaded = false;

	private int index = -1;

	public void ToEditComment(){
		makeAT.SetPose(threeD.GetModelPosition(),threeD.GetModelRotation());
		trans.Transition("Edit3DSceneComment");
	}
	public void ToEditScene(){
		makeAT.SetPose(threeD.GetModelPosition(),threeD.GetModelRotation());
		trans.Transition("EditScene");		
	}
	public void ToATV(){
		makeAT.SetPose(threeD.GetModelPosition(),threeD.GetModelRotation());
		trans.Transition("AttemptTreeView");		
	}
	public void To3DSetting(){
		makeAT.SetPose(threeD.GetModelPosition(),threeD.GetModelRotation());
		trans.Transition("ThreeDFirstSettingView");			
	}


	public void CopyJustBeforeScene(){
		int index = hScenes.GetCurIndex();
		if (AttemptTreeView.GetSceneType() == (int)AttemptTreeView.SCENETYPE.EDIT){
			index--;
		}
		HScene2 scene = hScenes.GetScene(index);

		if (scene != null){
			threeD.SetModelPose(scene.GetPose(), scene.GetRots());
		}
	}

	public override void ShowProc(){
		twoDWallMarks.SetTouchInfo(makeAT.Get2DTouchMarks());
		int index = hScenes.GetCurIndex();
		if (AttemptTreeView.GetSceneType() == (int)AttemptTreeView.SCENETYPE.EDIT){
			index--;
		}
		HScene2 scene = hScenes.GetScene(index);
		if (scene != null){
			threeD.CopyModelPose(copyModel, scene.GetPose(), scene.GetRots());	
			isShadowLoaded = true;
		}

		if(makeAT.IsPoseSet()){
			threeD.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
		}else{
			threeD.CorrectModelPose();
		}

		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		threeD.LookAtModel();

		gameObject.SetActive(true);
		foSelector.Init();
	}

	public void SwitchShadow(){
		if(isShadowLoaded){
			copyModel.gameObject.SetActive(!copyModel.gameObject.activeSelf);
		}
	}

	public override void HideProc(){
		threeD.InitModelPose();
		foSelector.Init();
		Hide();
	}

	public override void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		isShadowLoaded = false;
		copyModel.gameObject.SetActive(false);
	}

	public void SelectFO(){
		int n = Enum.GetNames(typeof(VRIKController.FullBodyMark)).Length - 1;
		index++;
		if (index > n){
			index = 0;
			foSelector.Release();
			return ;
		}
		while(!foSelector.IsFaceObj((VRIKController.FullBodyMark)index)){
			index++;
			if (index > n){
				index = 0;
				foSelector.Release();
				return ;
			}
		}
		foSelector.Regist((VRIKController.FullBodyMark)index);
	}
}
