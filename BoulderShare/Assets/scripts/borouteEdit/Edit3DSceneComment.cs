using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit3DSceneComment : SEComponentBase{
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private ScreenTransitionManager sManager;
	[SerializeField] private SceneComments3D comments;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private GameObject forLoopMode;

	public void CommentLookAtCamera(){
		scc.CommentLookAtCamera();
	}
	public void Submit(){
		makeAT.SetComments(scc.GetSceneComments());
		makeAT.Make();
		makeAT.Init();
	}
	public void ToEditPose(){
		makeAT.SetComments(scc.GetSceneComments());
		sManager.Transition(ScreenTransitionManager.Screen.EditPose);
	}
	public void ToATV(){
		makeAT.SetComments(scc.GetSceneComments());
		sManager.Transition(ScreenTransitionManager.Screen.AttemptTreeView);
	}
	public void ToMainView(){
		Submit();
		sManager.Transition(ScreenTransitionManager.Screen.MainView);
	}
	public void ToEditWallMark(){
		MakeAttemptTree.Mode m = makeAT.GetMode();
		int n = makeAT.GetIndex();
		Submit();
		makeAT.SetMode(m);
		makeAT.SetIndex(n+1);
		sManager.Transition(ScreenTransitionManager.Screen.EditWallMark);
	}

	public override void OnPreShow(){
		List<MyUtility.SceneCommentData3D> list = makeAT.GetComments();
		if(list != null){
			scc.SetSceneComments(list);
		}else{
			scc.Init();
		}
		cameraManager.Active3D();
		humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
		humanModel.LookAtModel();
		humanModel.HideMarks();
		scc.AcceptEvents();
		comments.ShowDynamically();
		comments.SetShowAngle(SceneComments3D.ANGLE_EDIT);

		if (makeAT.GetMode() == MakeAttemptTree.Mode.Add || makeAT.GetMode() == MakeAttemptTree.Mode.Loop){
			forLoopMode.SetActive(true);
		}else{
			forLoopMode.SetActive(false);
		}
	}

	public override void OnPreHide(){
		scc.IgnoreEvents();
		scc.Init();
		comments.DontShowAll();
		humanModel.InitModelPose();
	}

	public void AddComment(){
		scc.MakeComment();
	}
}
