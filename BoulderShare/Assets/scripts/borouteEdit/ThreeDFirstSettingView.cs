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
	private float size;

	public void Close(){
		humanModel.SetModelSize(size);
		wallManager.SyncIncline();
		sManager.Back();
	}
	public void Submit(){
		wallManager.CommitIncline(threeDWall.GetIncline());
		sManager.Back();
	}
	public override void OnPreShow(){
		size = humanModel.GetModelSize();
		incline.SyncInclineValue();
		humanModel.InitModelPose();
		cameraManager.Active3D();
		cameraManager.Reset3DCamPosAndDepth();
		modelSize.SyncModelSize();
	}

	public override void OnPreHide(){
	}
}
