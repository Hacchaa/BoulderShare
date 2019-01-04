using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttemptTreeView : MonoBehaviour , IUIComponent{
	[SerializeField]
	private HScenes2 hScenes;
	[SerializeField]
	private SceneScroll ss;
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private ScreenTransitionManager trans;
	[SerializeField]
	private TwoDWallMarks twoDWallMarks;
	[SerializeField]
	private GameObject twoDCamera;
	[SerializeField]
	private GameObject threeDCamera;
	[SerializeField]
	private GameObject ac;
	[SerializeField]
	private TwoDWallImage twoDWallImage;
	[SerializeField]
	private ThreeD threeD;
	[SerializeField]
	private FailedListView failedListView;
	[SerializeField]
	private GameObject failedListButton;
	[SerializeField]
	private SceneCommentController scc;

	public enum SCENETYPE{EDIT = 0, ADD};
	private static int sceneType = 0;

	public void LoadingBorouteProc(){
		SyncSceneScroll();
		failedListView.SetIsUpdateNeed(true);
	}


	private void SyncSceneScroll(){
		ss.Delete();
		int index = 0;
		foreach(HScene2 scene in hScenes.GetScenes()){
			ss.Add(index);
			index++;
		}
		ss.Focus(0);
	}

	public void AddScene(HScene2 scene){
		hScenes.AddScene(scene);

		int index = hScenes.GetCurIndex();

		ss.Add(index);
		ss.Focus(index);
	}

	public HScene2 GetCurScene(){
		return hScenes.GetCurScene();
	}
	public int GetCurSceneIndex(){
		return hScenes.GetCurIndex();
	}

	public HScene2 GetScene(int index){
		return hScenes.GetScene(index);
	}

	public void RemoveScene(){
		int index = hScenes.GetCurIndex();
		ss.Remove(index);
		hScenes.RemoveScene();
		index = hScenes.GetCurIndex();
		ss.Focus(index);
	}

	public void NextScene(){
		HScene2 scene = hScenes.NextScene();
		int index = hScenes.GetCurIndex();
		if (index >= 0){
			Load(scene);
			ss.Focus(index);
		}
	}

	public void PrevScene(){
		HScene2 scene = hScenes.PrevScene();
		int index = hScenes.GetCurIndex();
		if (index >= 0){
			Load(scene);
			ss.Focus(index);
		}
	}

	public void ShowProc(){
		gameObject.SetActive(true);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		HScene2 scene = hScenes.GetCurScene();
		int index = hScenes.GetCurIndex();
		if (index >= 0){
			Load(scene);
			ss.Focus(index);
		}

		twoDWallImage.ResetCamPosAndDepth();
		twoDCamera.SetActive(true);
		threeD.ResetCamPos();
		ac.SetActive(false);

		failedListButton.SetActive(failedListView.IsExist());

	}

	public void HideProc(){
		twoDWallMarks.ClearTouch();
		twoDWallImage.ResetCamPosAndDepth();
		Hide();
	}

	public void Hide(){
		gameObject.SetActive(false);
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}

		twoDCamera.SetActive(false);
		threeDCamera.SetActive(false);
	}

	//画面遷移
	public void Transition(string str){
		trans.Transition(str);
	}

	public void ToEditScene(int type){
		sceneType = type;
		Transition("EditScene");
	}

	public static int GetSceneType(){
		return sceneType;
	}

	public void Load(HScene2 scene){
		twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		threeD.SetModelPose(scene.GetPose(), scene.GetPRotate());
		threeD.SetIsLookingActivate(scene.IsLookingActivate());
		scc.SetSceneComments(scene.GetComments());
	}
}
