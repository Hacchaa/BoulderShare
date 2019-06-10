using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GarbageFor2DMark : MonoBehaviour , IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	[SerializeField] private Transform garbageIcon;
	[SerializeField] private float moveDist = 20.0f;
	[SerializeField] private float animDuration = 0.1f;
	private int latestRemovedID = -1;
	private Action OnRemoveAction = null;

	public void AddOnRemoveAction(Action a){
		if(a != null){
			OnRemoveAction += a;
		}
	}

	public int GetLatestRemovedID(){
		if (latestRemovedID >= 0){
			return latestRemovedID;
		}
		return -1;
	}

	public void ResetOnRemoveAction(){
		OnRemoveAction = null;
	}
	public void OnDrop(PointerEventData data){
		//Debug.Log("OnDrop");
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			if (OnRemoveAction != null){
				OnRemoveAction();
			}
			GameObject obj = data.pointerDrag;
			latestRemovedID = int.Parse(obj.name);
			twoDWallMarks.DeleteMark(obj.name);
			PointerExitAnimation();
		}
	}

	public void OnPointerEnter(PointerEventData data){
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			PointerEnterAnimation();
		}
	}

	public void OnPointerExit(PointerEventData data){
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			PointerExitAnimation();
		}
	}

	public void PointerEnterAnimation(){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(()=>
		{
			garbageIcon.localPosition = Vector3.zero;
		})
		.Append(garbageIcon.DOLocalMoveY(-moveDist, animDuration))
		.SetRelative();

		seq.Play();
	}

	public void PointerExitAnimation(){
		Sequence seq = DOTween.Sequence();

		seq.OnStart(()=>
		{
			garbageIcon.localPosition = new Vector3(0.0f, -moveDist, 0.0f);
		})
		.Append(garbageIcon.DOLocalMoveY(moveDist, animDuration))
		.SetRelative()
		.OnComplete(()=>
		{
			garbageIcon.localPosition = Vector3.zero;
		});

		seq.Play();
	}
}
