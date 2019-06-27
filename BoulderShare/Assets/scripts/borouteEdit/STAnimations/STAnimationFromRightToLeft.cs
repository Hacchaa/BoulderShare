using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public abstract class STAnimationFromRightToLeft : STAnimationBase
{
	[SerializeField] private float duration = 0.25f;
	[SerializeField] private float fromWidthRate = 0.3f;

	public override void Animate(){
		float width = fromRect.rect.width;
		float widthFrom = width * fromWidthRate;
		//Debug.Log("width:" + width);
		Sequence seq = DOTween.Sequence();
		seq.OnStart( () =>
		{	
			PrioritizeTo();
			canvasGroup.blocksRaycasts = false;
			fromRect.gameObject.SetActive(true);
			toRect.gameObject.SetActive(true);
			toRect.localPosition = new Vector3(width, 0.0f, 0.0f);
			if (OnPostStartAction != null){
				OnPostStartAction();
			}
		})
		.Append(toRect.DOLocalMoveX(-width,duration).SetEase(Ease.OutQuart).SetRelative())
		.Join(fromRect.DOLocalMoveX(-widthFrom, duration).SetEase(Ease.OutQuart).SetRelative())
		.OnComplete( () =>
		{
			canvasGroup.blocksRaycasts = true;
			fromRect.gameObject.SetActive(false);

			if (OnPostCompleteAction != null){
				OnPostCompleteAction();
			}
		});

		seq.Play();
	}
}
