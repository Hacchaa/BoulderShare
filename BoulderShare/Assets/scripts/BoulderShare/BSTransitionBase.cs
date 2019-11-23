using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BSTransitionBase : MonoBehaviour, ITransitionable
{
	[SerializeField] protected RectTransform screenRoot;
	protected SEComponentBase screen;
	protected CanvasGroup cg;
	protected Canvas canvas;
	public abstract void TransitionLerp(float t);

	public void Init(){
		cg = screenRoot.GetComponent<CanvasGroup>();
		canvas = screenRoot.GetComponent<Canvas>();
		screen = screenRoot.GetComponent<SEComponentBase>();
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
		screen.HideScreen();
		cg.blocksRaycasts = isBlocksRaycasts;
	}
	public void Activate(bool isBlocksRaycasts){
		screen.ShowScreen();
		cg.blocksRaycasts = isBlocksRaycasts;
	}

	public void UpdateScreen(){
		screen.OnPreShow();
	}

	public virtual void Ready(bool isReverse){
		screen.OnPreTransition();
	}

	public virtual void Complete(bool isShowed){
		screen.OnPostTransition(isShowed);
	}
}
