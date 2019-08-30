using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerGraphView : SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private HScenes2 hScenes;
	[SerializeField] private DAG dag;
	[SerializeField] private LayerGraphController lgController;
	[SerializeField] private RectTransform headRect;
	[SerializeField] private RectTransform topRect;
	[SerializeField] private RectTransform canvasRect;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private float switchDuration = 0.9f;
	private float startTime;
	private bool isLoop;

	public override void OnPreShow(){
		float y = 0.5f;
		float h = 0.5f - (headRect.sizeDelta.y + topRect.sizeDelta.y) / canvasRect.sizeDelta.y;
		lgController.SetViewportRect(y, h);
		lgController.ResetLGCameraPos();
		cameraManager.Active3D();
		cameraManager.Reset3DCamPosAndDepth();
		lgController.SetCameraActive(true);
	}

	public override void OnPreHide(){
		isLoop = false;
		lgController.ResetViewportRect();
		lgController.SetCameraActive(false);
		//humanModel.DeleteShadows();
	}

	public void Construction(){
		dag.Construction(hScenes.GetATList(), hScenes.GetConvertedScenes());
	}

	public void DrawLayerGraph(){
		dag.ConvertLayerGraph();
	}

	public void ToRouteDetailView(){
		trans.Transition(ScreenTransitionManager.Screen.RouteDetailView);
	}

	public void Next(){
		dag.ShowNext();
		startTime -= switchDuration;
		hScenes.LoadHScenes(dag.GetShowIndex());
		hScenes.SetCurIndex(-1);
	}

	public void Prev(){
		dag.ShowPrev();
		startTime -= switchDuration;
		hScenes.LoadHScenes(dag.GetShowIndex());
		hScenes.SetCurIndex(-1);
	}


	public void StartATAnimation(){
		int index = dag.GetShowIndex();
		if (index < 0 || index > hScenes.GetATNum() - 1){
			return ;
		}
		isLoop = true;
		hScenes.LoadHScenes(index);
		hScenes.SetCurIndex(-1);
		StartCoroutine(SwitchingSceneAnim());
	}

	private IEnumerator SwitchingSceneAnim(){
		startTime = Time.time;

		while(isLoop){
			if (Time.time - startTime >= switchDuration){
				SwitchScene();
				startTime = Time.time;
			}
			yield return null;
		}
	}

	public void EndATAnimation(){
		isLoop = false;
	}

	private void SwitchScene(){
		HScene2 scene = hScenes.NextSceneWithLoop();

		//twoDWallMarks.ClearTouch();
		///twoDWallMarks.SetTouchInfo(scene.GetOnHolds());
		humanModel.SetModelPose(scene.GetPose(), scene.GetRots(), scene.GetRightHandAnim(), scene.GetLeftHandAnim());
		//humanModel.SetCamAxisAsModelPos();
		//scc.SetSceneComments(scene.GetComments());
		//fcc.SetFailureComments(scene.GetFailureList());
	}


	private void ShowShadows(int index){
		if (index < 0 || index > hScenes.GetATNum() - 1){
			return ;
		}
		MyUtility.AttemptTree tree = hScenes.GetAT(index);
		Dictionary<int, MyUtility.Scene> map = hScenes.GetConvertedScenes();
		Vector3[][] pos = new Vector3[tree.idList.Count][];
		Quaternion[][] rots = new Quaternion[tree.idList.Count][];

		int i = 0;
		foreach(int id in tree.idList){
			pos[i] = map[id].pose;
			rots[i] = map[id].rots;
			i++;
		}

		//humanModel.ShowShadows(pos, rots);
	}

	public void LoadAT(){
		hScenes.LoadHScenes(dag.GetShowIndex());
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.Menu;
		trans.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}
}
