using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeDFirstSettingView : SEComponentBase
{
	[SerializeField] private BorouteAndInformation borAndInfo;
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

	public void Init(int i, float s){
		float inc = Mathf.Clamp(i, incline.GetMin(), incline.GetMax());
		float size = Mathf.Clamp(s, modelSize.GetMin(), modelSize.GetMax());
		humanModel.SetModelSize(s);
		wallManager.CommitIncline((int)inc);
		isInit = true;
	}

	public void Close(){
		humanModel.SetModelSize(size);
		wallManager.SyncIncline();
		sManager.Back();
	}
	public void Submit(){
		isInit = true;
		wallManager.CommitIncline(threeDWall.GetIncline());
		sManager.Transition(ScreenTransitionManager.Screen.SceneEditor);

		borAndInfo.SetIncline(threeDWall.GetIncline());
		borAndInfo.SetHumanScale(humanModel.GetModelSize());
	}
	public override void OnPreShow(){
		size = humanModel.GetModelSize();
		incline.SyncInclineValue();
		humanModel.InitModelPose();
		humanModel.HideMarks();
		cameraManager.Active3D();
		humanModel.LookAtModel();
		modelSize.SyncModelSize();
		wallManager.HideTranslucentWall();
	}

	public override void OnPreHide(){
	}
}
