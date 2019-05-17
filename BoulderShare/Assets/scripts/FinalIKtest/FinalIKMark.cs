﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKMark : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	private Vector3 offset;
	private float baseDepth;
	private Vector3 defaultPos;
	[SerializeField]
	private Transform avatar;
	private bool isDragging = false;

	// Use this for initialization
	void Awake(){
		finger = FINGER_NONE;
		//transform.position = avatar.position;
		defaultPos = transform.localPosition;
	}
	void Start(){

	}	

	public void Init(){
		transform.localPosition = defaultPos;
	}

	//avatarの位置に移動させる
	public void Correct(){
		if (!isDragging){
			if (avatar != null){
				//transform.position = avatar.position;
			}
		}
	}
	public Vector3 GetPosition(){
		return transform.localPosition;
	}
	public void SetPosition(Vector3 p){
		transform.localPosition = p;
	}


	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
			offset = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			offset = transform.parent.InverseTransformPoint(offset);
			offset = offset - transform.localPosition;
			isDragging = true;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//float len = (cam.transform.position - transform.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			p = transform.parent.InverseTransformPoint(p);
		
			transform.localPosition = p - offset;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = FINGER_NONE;
			isDragging = false;
		}
	}
}