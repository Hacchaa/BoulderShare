using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditScene : SEComponentBase{
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private GameObject twoDCamera;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private HScenes2 hScenes;
	void Awake(){
		/*
		float length = cameraUtil.GetWidthRate() * canvasScaler.referenceResolution.x;
		//Debug.Log(cameraUtil.GetWidthRate() + "," + canvasScaler.referenceResolution.x + "," + length);
		window.anchoredPosition = new Vector2(-length/2, -length/2);
		window.sizeDelta = new Vector2(length, length);*/
	}

	public void ToATV(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
		trans.Transition("AttemptTreeView");
	}

	public void ToEditPose(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
		trans.Transition("EditPose");
	}

	public void ToEditWallMark(){
		makeAT.Set2DTouchMarks(twoDWallMarks.GetTouchInfo());
		trans.Transition("EditWallMark");
	}

	public override void ShowProc(){
		twoDWallMarks.ClearTouch();
		string[] str = makeAT.Get2DTouchMarks();
		if (str != null && str.Length > 0){
			twoDWallMarks.SetTouchInfo(str);
		}
		
		HScene2 scene = hScenes.GetCurScene();
		if (scene != null){
			twoDWallMarks.SetDummyTouchInfo(scene.GetOnHolds());
		}

		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		twoDCamera.SetActive(true);
	}

	public override void HideProc(){
		twoDWallMarks.ClearTouch();
		Hide();
	}

	public override void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}

		twoDCamera.SetActive(false);
	}

}
