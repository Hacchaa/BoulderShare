using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EditorPopup : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI supportText;
	[SerializeField] private TextMeshProUGUI rightButtonText;
	[SerializeField] private TextMeshProUGUI leftButtonText;
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private float fadeInDuration = 0.15f;
	[SerializeField] private float fadeOutDuration = 0.075f;

	private Action leftProc = null;
	private Action rightProc = null;

	public void Open(Action rightAction, Action leftAction, string title, string support, string rightBText, string leftBText){
		leftProc = leftAction;
		rightProc = rightAction;
		titleText.text = title;
		supportText.text = support;
		rightButtonText.text = rightBText;
		leftButtonText.text = leftBText;

		PlayOpenAnimation();
	}

	public void Close(bool isRight){
		PlayCloseAnimation(isRight);
	}

	private void PlayOpenAnimation(){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(() =>
		{
			gameObject.SetActive(true);
			canvasGroup.blocksRaycasts = true;
			canvasGroup.interactable = false;
			canvasGroup.alpha = 0.0f;
		})
		.Append(canvasGroup.DOFade(1.0f, fadeInDuration).SetEase(Ease.InQuad))
		.OnComplete(() =>
		{
			canvasGroup.interactable = true;
		});
		seq.Play();
	}

	private void PlayCloseAnimation(bool isRight){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(() =>
		{
			canvasGroup.blocksRaycasts = true;
			canvasGroup.interactable = false;
			canvasGroup.alpha = 1.0f;
		})
		.Append(canvasGroup.DOFade(0.0f, fadeOutDuration).SetEase(Ease.OutQuad))
		.OnComplete(() =>
		{
			titleText.text = "";
			supportText.text = "";
			rightButtonText.text = "";
			leftButtonText.text = "";

			if (isRight){
				if(rightProc != null){
					rightProc();
					rightProc = null;
				}
			}else{
				if (leftProc != null){
					leftProc();
					leftProc = null;
				}
			}
			gameObject.SetActive(false);
		});

		seq.Play();
	}

	public void PushLeftBtn(){
		PlayCloseAnimation(false);
	}

	public void PushRightBtn(){
		PlayCloseAnimation(true);
	}
}
