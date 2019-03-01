using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HFControllerComponent : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int FINGER_NONE = -10;
	private static int finger = FINGER_NONE;
	[SerializeField]
	private TwoDMark.HFType bodyType;
	[SerializeField]
	private HFController controller;


	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
			controller.SetBodyType(bodyType);
			controller.AdjustArrowScale();
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			controller.Drag(data);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			controller.EndDrag();
			finger = FINGER_NONE;
		}
	}
}
