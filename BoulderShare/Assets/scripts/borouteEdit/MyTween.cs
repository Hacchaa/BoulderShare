using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MyTween : MonoBehaviour
{
	private static float duration = 0.1f;
	public static Sequence GetUIFadeOutSeq(List<GameObject> fadeOutList, List<Image> fadeOutCoverList, float dur = 0.0f
			, Action OnPostStartAction = null, Action OnPostCompleteAction = null){
		if (dur == 0.0f){
			dur = duration;
		}
		Sequence seq = DOTween.Sequence();

		seq.OnStart(()=>
		{
			foreach(GameObject obj in fadeOutList){
				obj.SetActive(true);
			}
			foreach(Image img in fadeOutCoverList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 0.0f);
				img.color = c;
				img.gameObject.SetActive(true);
			}
			if (OnPostStartAction != null){
				OnPostStartAction();
			}		
		});
		foreach(Image img in fadeOutCoverList){
			seq.Join(img.DOFade(1.0f, dur).SetEase(Ease.OutQuad));
		}
		seq.OnComplete(()=>
		{
			foreach(GameObject obj in fadeOutList){
				obj.SetActive(false);
			}
			foreach(Image img in fadeOutCoverList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
				img.gameObject.SetActive(false);
			}
			if (OnPostCompleteAction != null){
				OnPostCompleteAction();
			}	
		});

		return seq;
	}
	public static Sequence GetUIFadeInSeq(List<GameObject> fadeInList, List<Image> fadeInCoverList, float dur = 0.0f
			, Action OnPostStartAction = null, Action OnPostCompleteAction = null){
		if (dur == 0.0f){
			dur = duration;
		}
		Sequence seq = DOTween.Sequence();

		seq.OnStart(()=>
		{
			foreach(GameObject obj in fadeInList){
				obj.SetActive(true);
			}
			foreach(Image img in fadeInCoverList){
				Color c = img.color;
				c = new Color(c.r, c.g, c.b, 1.0f);
				img.color = c;
				img.gameObject.SetActive(true);
			}
			if (OnPostStartAction != null){
				OnPostStartAction();
			}		
		});
		foreach(Image img in fadeInCoverList){
			seq.Join(img.DOFade(0.0f, dur).SetEase(Ease.InQuad));
		}
		seq.OnComplete(()=>
		{
			foreach(Image img in fadeInCoverList){
				img.gameObject.SetActive(false);
			}
			if (OnPostCompleteAction != null){
				OnPostCompleteAction();
			}	
		});

		return seq;
	}
}
