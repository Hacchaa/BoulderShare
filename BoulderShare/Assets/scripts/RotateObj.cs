using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateObj : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public Transform trans;
	private int finger ;
	private static float weight = 0.5f;

	// Use this for initialization
	void Start () {
		finger = Observer.FINGER_NONE;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			finger = data.pointerId;
		}
	}
	
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			//y軸に回転させる
			trans.Rotate(0, -data.delta.x * weight, 0);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = Observer.FINGER_NONE;
		}
	}
}
