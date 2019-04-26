using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneEditorMark : SceneEditorComponent
{
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private Slider scaleSlider;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private TwoDWall twoDWall;

	//画面遷移時の前処理
	public override void OnPreShow(){
		if (!makeAT.IsWallMarkSet()){
			makeAT.SetWallMarks(wallManager.GetMasterWallMarks());
		}
		twoDWallMarks.AcceptEvents();
	}

	//画面遷移でこの画面を消す時の後処理
	public override void OnPreHide(){
		twoDWallMarks.IgnoreEvents();
		twoDWallMarks.ReleaseFocus();
	}

	public override void Regist(){
		wallManager.CommitWallMarks(twoDWall.GetWallMarks());
	}

	//2dmarkを選択する
	//EditWallMark/Selecter
	public void NextMark(){
		twoDWallMarks.Next();
	}

	public void PrevMark(){
		twoDWallMarks.Prev();
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
