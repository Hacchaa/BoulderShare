﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentOpen : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler{
	private Vector3 startPos ;
	private Vector3 endPos ;
	private float width ;
	public float duration = 1.0f;
	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	private bool isOpen = false;
	public RectTransform rect;
	public Shield shield;
	private float beginX;
	private int finger ;
	private static float THRESHOLD = 15.0f;

	// Use this for initialization
	void Start () {
		finger = Observer.FINGER_NONE;
		width = rect.rect.width;
		startPos = rect.localPosition;
		endPos = new Vector3(startPos.x+width, startPos.y, startPos.z);
	}

	public void OnPointerClick(PointerEventData data){
		Open();
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			//Debug.Log("Start:"+data.position.x);
			beginX = data.position.x;
			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){

	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			//Debug.Log("End:"+data.position.x);
			if (isOpen){
				if (data.position.x - beginX < -THRESHOLD){
					Open();
				}
			}else{
				if (data.position.x - beginX > THRESHOLD){
					Open();
				}
			}
			finger = Observer.FINGER_NONE;
			beginX = 0.0f;
		}
	}
	
	public void Open(){
		if (isOpen){
			shield.gameObject.SetActive(false);
		}else{
			shield.Open(this.Open);
		}
		StartCoroutine(OpenComment());
	}

	private IEnumerator OpenComment(){
		float startTime = Time.time;
		Vector3 moveDistance;

		if (isOpen){
			moveDistance = (startPos - endPos);
		}else{
			moveDistance = (endPos - startPos);
		}

		while((Time.time - startTime) < duration){
			if (isOpen){
				rect.localPosition = endPos + moveDistance 
					* animCurve.Evaluate((Time.time - startTime) / duration);
			}else{
				rect.localPosition = startPos + moveDistance 
					* animCurve.Evaluate((Time.time - startTime) / duration);
			}
			yield return 0;
		}

		if (isOpen){
			rect.localPosition = startPos;
			isOpen = false;
		}else{
			rect.localPosition = endPos;
			isOpen = true;
		}
	}
}
