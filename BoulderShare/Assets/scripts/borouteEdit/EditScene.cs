using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditScene : SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private CameraManager cameraManager;

	void Awake(){
		/*
		float length = cameraUtil.GetWidthRate() * canvasScaler.referenceResolution.x;
		//Debug.Log(cameraUtil.GetWidthRate() + "," + canvasScaler.referenceResolution.x + "," + length);
		window.anchoredPosition = new Vector2(-length/2, -length/2);
		window.sizeDelta = new Vector2(length, length);*/
	}

	public void ToATV(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.View;
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}

	public void ToEditPose(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
		trans.Transition(ScreenTransitionManager.Screen.EditPose);
	}

	public void ToEditWallMark(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
		trans.Transition(ScreenTransitionManager.Screen.EditWallMark);
	}

	public override void OnPreShow(){
		cameraManager.Active2D();
		twoDWallMarks.ClearTouch();
		string[] str = makeAT.Get2DTouchMarks();
		if (str != null && str.Length > 0){
			twoDWallMarks.SetTouchInfo(str);
		}
		int ind = makeAT.GetIndex();
		if (makeAT.GetMode() == MakeAttemptTree.Mode.Edit || makeAT.GetMode() == MakeAttemptTree.Mode.Add){
			ind--;
		}
		HScene2 scene = hScenes.GetScene(ind);
		if (scene != null){
			twoDWallMarks.SetDummyTouchInfo(scene.GetOnHolds());
		}
		twoDWallMarks.IgnoreEvents();
	}

	public override void OnPreHide(){
		twoDWallMarks.ClearTouch();
	}
}
