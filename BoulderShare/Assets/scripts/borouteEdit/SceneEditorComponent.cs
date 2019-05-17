using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public abstract class SceneEditorComponent : MonoBehaviour
{
	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private float animDuration = 0.1f;
	[SerializeField] private bool is2D;
    protected MakeAttemptTree makeAT;

    public void SetMakeAT(MakeAttemptTree m){
        makeAT = m;
    }

	public bool Is2D(){
		return is2D;
	}

    public Sequence GetShowFromRightSeq(){
    	float w = rectTransform.rect.width;

    	Sequence seq = DOTween.Sequence();

    	seq.OnStart(() =>
    	{	
    		gameObject.SetActive(true);
    		transform.localPosition = new Vector3(w/16.0f, 0.0f, 0.0f);

    	})
    	.Append(transform.DOLocalMoveX(-w/16.0f, animDuration));
    	seq.SetRelative();

    	return seq;
    }
    public Sequence GetShowFromLeftSeq(){
    	float w = rectTransform.rect.width;
    	Sequence seq = DOTween.Sequence();

    	seq.OnStart(() =>
    	{	
    		gameObject.SetActive(true);
    		transform.localPosition = new Vector3(-w/16.0f, 0.0f, 0.0f);
    	})
    	.Append(transform.DOLocalMoveX(w/16.0f, animDuration));
    	seq.SetRelative();

    	return seq;
    }

    public Sequence GetHideToLeftSeq(){
    	float w = rectTransform.rect.width;

    	Sequence seq = DOTween.Sequence();
    	seq.OnStart(() =>
		{
		})
    	.Append(transform.DOLocalMoveX(-w/16.0f, animDuration));

    	seq.OnComplete(() =>
    	{
            transform.localPosition = Vector3.zero;
    		gameObject.SetActive(false);
    	});

    	return seq;
    }

    public Sequence GetHideToRightSeq(){
    	float w = rectTransform.rect.width;

    	Sequence seq = DOTween.Sequence();
    	seq.OnStart(() =>
		{

		})
    	.Append(transform.DOLocalMoveX(w/16.0f, animDuration));

    	seq.OnComplete(() =>
    	{
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
    	});

    	return seq;
    }

    public void Hide(){
    	gameObject.SetActive(false);
    }

    public void Show(){
        gameObject.SetActive(true);

    }

    public abstract void OnPreShow();
    public abstract void OnPreHide();
    public abstract void Regist();
}
