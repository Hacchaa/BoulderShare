using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelViewWindow : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private static int FINGER_NONE = -10;
	private static int finger = FINGER_NONE;
	private bool isDragged = false;
	private const float weight = 0.5f;

	[SerializeField]
	private EditScene es;
	[SerializeField]
	private Transform rotTarget;

	public void OnPointerDown(PointerEventData data){
		isDragged = false;
	}

	public void OnPointerUp(PointerEventData data){
		if (!isDragged){
			es.SwitchDimension();
		}
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
			isDragged = true;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			rotTarget.Rotate(0f, data.delta.x * weight, 0f);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = FINGER_NONE;
		}
	}



}
