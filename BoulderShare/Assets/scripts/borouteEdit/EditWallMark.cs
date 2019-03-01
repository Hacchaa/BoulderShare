﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditWallMark : SEComponentBase{
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private GameObject markOptions;
	[SerializeField]
	private Slider scaleSlider;
	[SerializeField]
	private ThreeDWallMarks threeDWallMarks;
	[SerializeField] private MakeAttemptTree makeAT;

	public void Submit(){
		Close();
	}

	public void ToEdit2DPose(){
		trans.Transition("EditScene");
	}

	public void ToATV(){
		trans.Transition("AttemptTreeView");
	}

	public void Close(){
		makeAT.Init();
		trans.Transition("MainView");
	}

	//画面遷移時の前処理
	public override void ShowProc(){
		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		twoDWallMarks.AcceptEvents();
		CloseMarkOptions();
	}

	public override void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
	}
	//画面遷移でこの画面を消す時の後処理
	public override void HideProc(){
		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.ReleaseFocus();

		threeDWallMarks.Synchronize();
		Hide();
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
