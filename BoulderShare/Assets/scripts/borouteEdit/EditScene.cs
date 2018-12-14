using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditScene : MonoBehaviour, IUIComponent {
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private AttemptTreeView atv;
	[SerializeField]
	private GameObject twoDCamera;
	[SerializeField]
	private ThreeD threeD;

	private HScene2 curScene;
	private Vector3[] curPose;
	private Quaternion[] curRotate;

	private bool isPoseDetermined;

	void Awake(){
		isPoseDetermined = false;
	}


	public void SwitchDimension(){
		trans.Transition("EditPose");
	}

	public Vector3[] GetPose(){
		return curPose;
	}

	public Quaternion[] GetRotate(){
		return curRotate;
	}

	public void SetPose(Vector3[] pose){
		curPose = pose;
		isPoseDetermined = true;
	}

	public void SetRotate(Quaternion[] rot){
		curRotate = rot;
		isPoseDetermined = true;
	}

	public bool IsPoseDetermined(){
		return isPoseDetermined;
	}

	public void Submit(){
		if (curScene == null){
			curScene = new HScene2();
			curScene.SetOnHolds(twoDWallMarks.GetTouchInfo());

			if (isPoseDetermined){
				curScene.SavePose(curPose);
				curScene.SavePRotate(curRotate);
			}else{
				curScene.SavePose(threeD.GetModelPosition());
				curScene.SavePRotate(threeD.GetModelRotation());
			}
			atv.AddScene(curScene);
		}else{
			curScene.SetOnHolds(twoDWallMarks.GetTouchInfo());

			if (isPoseDetermined){
				curScene.SavePose(curPose);
				curScene.SavePRotate(curRotate);
			}else{
				curScene.SavePose(threeD.GetModelPosition());
				curScene.SavePRotate(threeD.GetModelRotation());
			}
		}
		Close();
	}

	public void Close(){
		isPoseDetermined = false;
		curScene = null;
		twoDWallMarks.ClearTouch();
		trans.Transition("AttemptTreeView");
	}

	public void ShowProc(){
		int t = AttemptTreeView.GetSceneType();

		if(!isPoseDetermined && t == (int)AttemptTreeView.SCENETYPE.EDIT){
			curScene = atv.GetCurScene();
			if (curScene != null){
				twoDWallMarks.SetTouchInfo(curScene.GetOnHolds());
				curPose = curScene.GetPose();
				curRotate = curScene.GetPRotate();
				isPoseDetermined = true;
			}
		}
		threeD.InitModelPose();

		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		twoDCamera.SetActive(true);
	}

	public void HideProc(){
		Hide();
	}

	public void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}

		twoDCamera.SetActive(false);
	}

}
