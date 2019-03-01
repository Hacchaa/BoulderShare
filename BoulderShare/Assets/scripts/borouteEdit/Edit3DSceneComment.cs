using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit3DSceneComment : SEComponentBase{
	[SerializeField]
	private List<GameObject> externalUIComponents;
	[SerializeField]
	private SceneCommentController3D scc;
	[SerializeField]
	private ThreeDWall threeDWall;
	[SerializeField]
	private GameObject colorSetter;
	[SerializeField]
	private GameObject FontSizeSetter;
	[SerializeField]
	private ScreenTransitionManager sManager;
	[SerializeField] private SceneComments3D comments;
	[SerializeField] private MakeAttemptTree makeAT;
	[SerializeField] private ThreeD threeD;

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
		sManager.Transition("EditPose");
	}
	public void ToATV(){
		makeAT.SetComments(scc.GetSceneComments());
		sManager.Transition("AttemptTreeView");
	}
	public void ToMainView(){
		Submit();
		sManager.Transition("MainView");
	}
	public void ToEditWallMark(){
		Submit();
		sManager.Transition("EditWallMark");
	}

	public override void ShowProc(){
		List<MyUtility.SceneCommentData3D> list = makeAT.GetComments();
		if(list != null){
			scc.SetSceneComments(list);
		}else{
			scc.Init();
		}
		threeD.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(true);
		}

		threeD.LookAtModel();
		this.gameObject.SetActive(true);

		colorSetter.SetActive(false);
		FontSizeSetter.SetActive(false);
		scc.AcceptEvents();
		comments.ShowDynamically();
		comments.SetShowAngle(SceneComments3D.ANGLE_EDIT);
	}

	public override void HideProc(){
		scc.IgnoreEvents();
		scc.Init();
		colorSetter.SetActive(false);
		FontSizeSetter.SetActive(false);
		comments.DontShowAll();
		threeD.InitModelPose();
		Hide();
	}

	public override void Hide(){
		foreach(GameObject obj in externalUIComponents){
			obj.SetActive(false);
		}
		this.gameObject.SetActive(false);
	}

	public void AddComment(){
		scc.MakeComment();
	}
}
