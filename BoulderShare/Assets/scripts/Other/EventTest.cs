using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EventTest : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger = MyUtility.FINGER_NONE;
	[SerializeField] private Camera cam;
	private float baseDepth;

	public void OnBeginDrag(PointerEventData data){
		//Debug.Log("onbeginDrag");
		if (finger == MyUtility.FINGER_NONE){
			finger = data.pointerId;

			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
		}
	}

	public float GetR(){
		return gameObject.transform.localScale.x / 2;
	}

	void FixedUpdate(){
		//Debug.Log("FixedUpdate");
	}
	void Update(){
		//Debug.Log("update");
	}

	void LateUpdate(){
		//Debug.Log("lateUpdate");
	}
	public virtual void OnDrag(PointerEventData data){
		//Debug.Log("OnDrag");
		if (finger == data.pointerId){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					baseDepth));
			Vector3 pOld = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x - data.delta.x, 
					data.position.y - data.delta.y, 
					baseDepth));

			Vector3 v = p - pOld;
			transform.Translate(v, Space.World);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = MyUtility.FINGER_NONE;
		}
	}

	private void OnTriggerEnter(Collider other){
		//Debug.Log("OnTriggerEnter");
	}
}
