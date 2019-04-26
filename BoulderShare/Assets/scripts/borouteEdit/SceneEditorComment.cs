using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEditorComment : SceneEditorComponent
{
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private SceneComments3D comments;
	[SerializeField] private HumanModel humanModel;
	
	public void CommentLookAtCamera(){
		scc.CommentLookAtCamera();
	}

	public override void OnPreShow(){
		List<MyUtility.SceneCommentData3D> list = makeAT.GetComments();
		if(list != null){
			scc.SetSceneComments(list);
		}else{
			scc.Init();
		}

		humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
		//humanModel.LookAtModel();
		humanModel.HideMarks();
		scc.AcceptEvents();
		comments.ShowDynamically();
		comments.SetShowAngle(SceneComments3D.ANGLE_EDIT);
	}

	public override void OnPreHide(){
		scc.IgnoreEvents();
		scc.Init();
		comments.DontShowAll();
		humanModel.InitModelPose();
	}

	public override void Regist(){
		makeAT.SetComments(scc.GetSceneComments());
	}

	public void AddComment(){
		scc.MakeComment();
	}
}
