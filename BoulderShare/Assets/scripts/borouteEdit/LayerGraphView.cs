using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerGraphView : SEComponentBase{
	[SerializeField] private ScreenTransitionManager trans;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private HScenes2 hScenes2;
	[SerializeField] private DAG dag;

	public override void OnPreShow(){
		cameraManager.Active2D();
		cameraManager.Set2DCamPos(dag.GetViewPos() + new Vector3(0.0f, 0.0f, -5f));
	}

	public override void OnPreHide(){

	}

	public void Construction(){
		dag.Construction(hScenes2.GetConvertedHScenesList());
	}

	public void DrawLayerGraph(){
		dag.ConvertLayerGraph();
	}

	public void ToMainView(){
		trans.Transition(ScreenTransitionManager.Screen.MainView);
	}

	public void Next(){
		dag.ShowNext();
	}

	public void Prev(){
		dag.ShowPrev();
	}
}
