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
	[SerializeField] private Image focusFieldCover;
	[SerializeField] private Transform addCommentField;
	[SerializeField] private float moveDist = 50.0f;
	[SerializeField] private float innerFadeDuration = 0.1f;
	[SerializeField] private ScrollRect focusSR;

	private Sequence GetHideSeq(bool isFocused){
    	Sequence seq = DOTween.Sequence();
    	Transform target = null;

    	if (isFocused){
			target = focusField;
    	}else{
			target = addCommentField;   			
    	}

    	seq.OnStart(() =>
    	{	
    		if (isFocused){
    			addCommentField.gameObject.SetActive(false);
    			focusField.gameObject.SetActive(true);
    		}else{
     			addCommentField.gameObject.SetActive(true);
    			focusField.gameObject.SetActive(false);   			
    		}
    		
			Color c = focusFieldCover.color;
			c = new Color(c.r, c.g, c.b, 0.0f);
			focusFieldCover.color = c;
			focusFieldCover.gameObject.SetActive(true);

			target.localPosition = Vector3.zero;

    	})
    	.Append(target.DOLocalMoveY(-moveDist, innerFadeDuration).SetEase(Ease.InQuad).SetRelative())
    	.Join(focusFieldCover.DOFade(1.0f, innerFadeDuration));

    	return seq;
	}

	private Sequence GetShowSeq(bool isFocused){
    	Sequence seq = DOTween.Sequence();
    	Transform target = null;

    	if (isFocused){
			target = addCommentField;
    	}else{
			target = focusField;   			
    	}

    	seq.OnStart(() =>
    	{	
    		if (isFocused){
    			addCommentField.gameObject.SetActive(true);
    			focusField.gameObject.SetActive(false);
    		}else{
     			addCommentField.gameObject.SetActive(false);
    			focusField.gameObject.SetActive(true);   			
    		}
    		
			Color c = focusFieldCover.color;
			c = new Color(c.r, c.g, c.b, 1.0f);
			focusFieldCover.color = c;
			focusFieldCover.gameObject.SetActive(true);

			target.localPosition = new Vector3(0.0f, -moveDist, 0.0f);
			focusSR.verticalNormalizedPosition = 1.0f;
    	})
    	.Append(target.DOLocalMoveY(moveDist, innerFadeDuration).SetEase(Ease.OutQuad).SetRelative())
    	.Join(focusFieldCover.DOFade(0.0f, innerFadeDuration))
    	.OnComplete(()=>
    	{
    		focusFieldCover.gameObject.SetActive(false);
    	});

    	return seq;
	}

	public void OpenFocusField(bool isAlreadyFocused){
		Sequence seq = DOTween.Sequence();

		seq.Append(GetHideSeq(isAlreadyFocused));
		seq.Append(GetShowSeq(false));
		seq.Play();
	}

	public void CloseFocusField(){
		Sequence seq = DOTween.Sequence();

		seq.Append(GetHideSeq(true));
		seq.Append(GetShowSeq(true));
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

		humanModel.SetModelPose(makeAT.GetPositions(), makeAT.GetRotations());
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
