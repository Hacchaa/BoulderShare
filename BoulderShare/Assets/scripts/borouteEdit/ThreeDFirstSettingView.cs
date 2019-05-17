using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeDFirstSettingView : SEComponentBase
{
	[SerializeField] private ScreenTransitionManager sManager;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private ThreeDWall threeDWall;
	[SerializeField] private InclineSetter incline;
	[SerializeField] private ModelSizeSetter modelSize;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private WallManager wallManager;
	[SerializeField] private bool isInit = false;
	private float size;

	public bool IsInit(){
		return isInit;
	}

	public void Close(){
		humanModel.SetModelSize(size);
		wallManager.SyncIncline();
		sManager.Back();
	}
	public void Submit(){
		isInit = true;
		wallManager.CommitIncline(threeDWall.GetIncline());
		AttemptTreeMenu.mode = AttemptTreeMenu.Mode.Menu;
		sManager.Transition(ScreenTransitionManager.Screen.AttemptTreeMenu);
	}
	public override void OnPreShow(){
		size = humanModel.GetModelSize();
		incline.SyncInclineValue();
		humanModel.InitModelPose();
		humanModel.HideMarks();
		cameraManager.Active3D();
		cameraManager.Reset3DCamPosAndDepth();
		modelSize.SyncModelSize();
		threeDWall.HideTranslucentWall();
	}

	public override void OnPreHide(){
	}
}
