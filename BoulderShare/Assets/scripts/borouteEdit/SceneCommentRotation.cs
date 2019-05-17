using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneCommentRotation :MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
	private static int FINGER_NONE = -10;
	private int finger = FINGER_NONE;
	[SerializeField] private SceneComment3D sceneComment;
	[SerializeField] private float WEIGHT = 0.5f;
	[SerializeField] private CameraManager cManager;

	public void OnPointerUp(PointerEventData data){
		
	}

	public void OnPointerDown(PointerEventData data){

	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			sceneComment.Rotate(0.0f, -data.delta.x * WEIGHT, 0.0f);
			cManager.Rotate3D(0.0f, -data.delta.x * WEIGHT, 0.0f);
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = FINGER_NONE;
		}
	}
}
