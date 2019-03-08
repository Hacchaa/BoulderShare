using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditWallMark : SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private GameObject markOptions;
	[SerializeField] private Slider scaleSlider;
	[SerializeField] private ThreeDWallMarks threeDWallMarks;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWall twoDWall;

	public void ToEdit2DPose(){
		wallManager.CommitWallMarks(twoDWall.GetWallMarks());
		trans.Transition(ScreenTransitionManager.Screen.EditScene);
	}

	public void ToATV(){
		wallManager.CommitWallMarks(twoDWall.GetWallMarks());
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeView);
	}

	public void Close(){
		wallManager.CommitWallMarks(makeAT.GetWallMarks());
		makeAT.Init();
		trans.Transition(ScreenTransitionManager.Screen.MainView);
	}

	//画面遷移時の前処理
	public override void OnPreShow(){
		if (!makeAT.IsWallMarkSet()){
			makeAT.SetWallMarks(wallManager.GetMasterWallMarks());
		}
		cameraManager.Active2D();
		twoDWallMarks.AcceptEvents();
		CloseMarkOptions();
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.ReleaseFocus();
	}

	//2dmarkを選択する
	//EditWallMark/Selecter
	public void NextMark(){
		twoDWallMarks.Next();
	}

	public void PrevMark(){
		twoDWallMarks.Prev();
	}

	public void OpenMarkOptions(){
		markOptions.SetActive(true);
		SetScaleSliderVal();
	}

	public void CloseMarkOptions(){
		markOptions.SetActive(false);
	}

	//2dマークの大きさを決めるスライダー
	//EditWallMark/MarkOptions/ScaleSlider
	public void OnScaleSlider(float val){
		TwoDMark mark = twoDWallMarks.GetFocus();
		mark.transform.localScale = Vector3.one * val;
	}

	public void SetScaleSliderVal(){
		TwoDMark mark = twoDWallMarks.GetFocus();
		scaleSlider.value = mark.transform.localScale.x;
	}

}
