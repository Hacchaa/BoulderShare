﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public abstract class STAnimationFromRightToLeftReverse : STAnimationBase
{
	[SerializeField] private float duration = 0.25f;

	public override void Animate(){
		float width = fromRect.rect.width;
		//Debug.Log("width:" + width);
		Sequence seq = DOTween.Sequence();
		seq.OnStart( () =>
		{	
			PrioritizeFrom();
			canvasGroup.blocksRaycasts = false;
			fromRect.gameObject.SetActive(true);
			toRect.gameObject.SetActive(true);
			fromRect.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			toRect.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			if (OnPostStartAction != null){
				OnPostStartAction();
			}
		})
		.Append(fromRect.DOLocalMoveX(width,duration).SetEase(Ease.OutQuad).SetRelative())
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