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
	[SerializeField]
	private SceneCommentController3D scc;
	[SerializeField] private RectTransform window;
	[SerializeField] private CameraUtility cameraUtil;
	[SerializeField] private CanvasScaler canvasScaler;
	private HScene2 curScene;
	[SerializeField] private Vector3[] curPose;
	private Quaternion[] curRot;

	[SerializeField]
	private bool isPoseDetermined = false;
	private bool isCurLookingAct = false;
	//コメントかポーズのシーンから帰ってきたかどうか(sceneを読み込んでから一度showProcが呼ばれたかどうか)
	private bool isAlreadyLoaded = false;

	void Awake(){
		float length = cameraUtil.GetWidthRate() * canvasScaler.referenceResolution.x;
		//Debug.Log(cameraUtil.GetWidthRate() + "," + canvasScaler.referenceResolution.x + "," + length);
		window.anchoredPosition = new Vector2(-length/2, -length/2);
		window.sizeDelta = new Vector2(length, length);
	}

	public void DeleteScene(){
		if (curScene != null){
			atv.RemoveScene();
		}
		Close();
	}


	public void SwitchDimension(){
		trans.Transition("EditPose");
	}

	public Vector3[] GetPose(){
		return curPose;
	}

	public Quaternion[] GetRots(){
		return curRot;
	}

	public void SetPose(Vector3[] pose, Quaternion[] rots){
		curPose = pose;
		curRot = rots;
		isPoseDetermined = true;
	}

	public bool IsCurLookingActivate(){
		return isCurLookingAct;
	}

	public void SetIsCurLookingActivate(bool b){
		isCurLookingAct = b;
	}

	public bool IsPoseDetermined(){
		return isPoseDetermined;
	}

	public void Submit(){
		if (curScene == null){
			curScene = new HScene2();
			curScene.SetOnHolds(twoDWallMarks.GetTouchInfo());
			curScene.SaveComments(scc.GetSceneComments());

			if (isPoseDetermined){
				curScene.SavePose(curPose, curRot);
				curScene.SetIsLookingActivate(isCurLookingAct);
			}else{
				curScene.SavePose(threeD.GetModelPosition(), threeD.GetModelRotation());
				curScene.SetIsLookingActivate(isCurLookingAct);
			}
			atv.AddScene(curScene);
		}else{
			curScene.SetOnHolds(twoDWallMarks.GetTouchInfo());
			curScene.SaveComments(scc.GetSceneComments());

			if (isPoseDetermined){
				curScene.SavePose(curPose, curRot);
				curScene.SetIsLookingActivate(isCurLookingAct);
			}else{
				curScene.SavePose(threeD.GetModelPosition(), threeD.GetModelRotation());
				curScene.SetIsLookingActivate(isCurLookingAct);
			}
		}
		Close();
	}

	public void Close(){
		isPoseDetermined = false;
		isCurLookingAct = false;
		isAlreadyLoaded = false;
		curScene = null;
		twoDWallMarks.ClearTouch();
		trans.Transition("AttemptTreeView");
	}

	public void ShowProc(){
		int t = AttemptTreeView.GetSceneType();

		if(!isAlreadyLoaded){
			scc.Init();
			if(t == (int)AttemptTreeView.SCENETYPE.EDIT){
				//Debug.Log("1");
				curScene = atv.GetCurScene();
				if (curScene != null){
					//Debug.Log("2");
					twoDWallMarks.SetTouchInfo(curScene.GetOnHolds());
					curPose = curScene.GetPose();
					curRot = curScene.GetRots();
					isCurLookingAct = curScene.IsLookingActivate();
					isPoseDetermined = true;
					scc.SetSceneComments(curScene.GetComments());
				}
			}
		}

		if (isPoseDetermined){
			threeD.SetModelPose(curPose, curRot);
		}else{
			threeD.InitModelPose();
		}
		threeD.ResetCamPos();
		threeD.LookAtModel();

		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}
		twoDCamera.SetActive(true);

		isAlreadyLoaded = true;
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

	public void ToEditSceneComment(){
		trans.Transition("EditSceneComment");
	}

}
