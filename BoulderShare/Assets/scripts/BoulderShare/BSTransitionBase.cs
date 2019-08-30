using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BSTransitionBase : MonoBehaviour, ITransitionable
{
	[SerializeField] protected RectTransform screenRoot;
	protected CanvasGroup cg;
	protected Canvas canvas;
	public abstract void TransitionLerp(float t);
	public abstract void Ready(bool isReady);
	public abstract void Complete(bool isReady);

	public void Init(){
		cg = screenRoot.GetComponent<CanvasGroup>();
		canvas = screenRoot.GetComponent<Canvas>();
	}
	
	public RectTransform GetRoot(){
		return screenRoot;
	}

	public int GetSortingOrder(){
		return canvas.sortingOrder;
	}

	public void SetSortingOrder(int o){
		canvas.sortingOrder = o;
	}

	public void DeActivate(bool isBlocksRaycasts){
		screenRoot.gameObject.SetActive(false);
		cg.blocksRaycasts = isBlocksRaycasts;
	}
	public void Activate(bool isBlocksRaycasts){
		screenRoot.gameObject.SetActive(true);
		cg.blocksRaycasts = isBlocksRaycasts;
	}

}
