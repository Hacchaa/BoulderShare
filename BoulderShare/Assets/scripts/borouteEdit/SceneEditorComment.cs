using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SceneEditorComment : SceneEditorComponent
{
	[SerializeField] private SceneCommentController3D scc;
	[SerializeField] private SceneComments3D comments;
	[SerializeField] private HumanModel humanModel;
	[SerializeField] private Transform focusField;
	[SerializeField] private Transform addCommentField;
	[SerializeField] private float moveFF = 220.0f;
	[SerializeField] private float offset = -60.0f;
	[SerializeField] private float innerFadeDuration = 0.1f;
	[SerializeField] private ScrollRect focusSR;

	private Sequence GetHideSeq(){
    	Sequence seq = DOTween.Sequence();

    	seq.OnStart(() =>
    	{	
			addCommentField.gameObject.SetActive(true);
			focusField.gameObject.SetActive(true);
    		
			focusField.localPosition = new Vector3(0.0f, moveFF/2.0f+offset, 0.0f);

    	})
    	.Append(focusField.DOLocalMoveY(-moveFF, innerFadeDuration).SetEase(Ease.InQuad).SetRelative())
    	.OnComplete(()=>
    	{
    		focusField.gameObject.SetActive(false);
    	});

    	return seq;
	}

	private Sequence GetShowSeq(){
    	Sequence seq = DOTween.Sequence();

    	seq.OnStart(() =>
    	{	
			addCommentField.gameObject.SetActive(true);
			focusField.gameObject.SetActive(true);

			focusField.localPosition = new Vector3(0.0f, -moveFF/2.0f+offset, 0.0f);
			focusSR.verticalNormalizedPosition = 1.0f;
    	})
    	.Append(focusField.DOLocalMoveY(moveFF, innerFadeDuration).SetEase(Ease.OutQuad).SetRelative())
    	.OnComplete(()=>
    	{
    		addCommentField.gameObject.SetActive(false);
    	});

    	return seq;
	}

	public void OpenFocusField(bool isAlreadyFocused){
		if (isAlreadyFocused){
			return ;
		}
		Sequence seq = DOTween.Sequence();

		seq.Append(GetShowSeq());
		seq.Play();
	}

	public void CloseFocusField(){
		Sequence seq = DOTween.Sequence();

		seq.Append(GetHideSeq());
		seq.Play();
	}
	
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

		humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations(), makeAT.GetRightHandAnim(), makeAT.GetLeftHandAnim());
		//humanModel.LookAtModel();
		humanModel.HideMarks();
		scc.AcceptEvents();
		comments.ShowDynamically();
		comments.SetShowAngle(SceneComments3D.ANGLE_EDIT);
		addCommentField.gameObject.SetActive(true);
		focusField.gameObject.SetActive(false);
	}

	public override void OnPreHide(){
		scc.IgnoreEvents();
		scc.Init();
		comments.DontShowAll();
		humanModel.InitModelPose();
		addCommentField.gameObject.SetActive(true);
		focusField.gameObject.SetActive(false);
	}

	public override void Regist(){
		makeAT.SetComments(scc.GetSceneComments());
	}

	public void AddComment(){
		scc.MakeComment();
	}
}
