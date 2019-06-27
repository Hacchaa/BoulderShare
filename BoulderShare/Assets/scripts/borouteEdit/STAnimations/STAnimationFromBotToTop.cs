using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public abstract class STAnimationFromBotToTop : STAnimationBase
{
	[SerializeField] private float duration = 0.25f;

	public override void Animate(){
		float height = fromRect.rect.height;
		//Debug.Log("height:" + height);
		Sequence seq = DOTween.Sequence();
		seq.OnStart( () =>
		{	
			PrioritizeTo();
			canvasGroup.blocksRaycasts = false;
			fromRect.gameObject.SetActive(true);
			toRect.gameObject.SetActive(true);
			toRect.localPosition = new Vector3(0.0f, -height, 0.0f);
			if (OnPostStartAction != null){
				OnPostStartAction();
			}
		})
		.Append(toRect.DOLocalMoveY(height,duration).SetEase(Ease.OutQuart).SetRelative())
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
