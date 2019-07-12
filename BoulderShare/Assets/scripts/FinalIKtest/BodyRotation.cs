using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyRotation : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	protected static int finger = MyUtility.FINGER_NONE;
	[SerializeField] private float rotateWeight = 0.5f;
	[SerializeField] private Transform spineAvatar;
	[SerializeField] private Transform neckAvatar;
	[SerializeField] private Transform model;
	[SerializeField] private Camera cam;
	[SerializeField] private float baseDepth;
	[SerializeField] private Vector3 offset;

	public void FitTargetInAvatar(){
		transform.position = spineAvatar.TransformPoint(offset);
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == MyUtility.FINGER_NONE){
			finger = data.pointerId;
			baseDepth = cam.gameObject.transform.InverseTransformPoint(transform.position).z;
		}
	}
    public void OnDrag(PointerEventData data){
    	if (finger == data.pointerId){
			//カメラから見た軸の傾きを求める
			Vector3 camAxis = cam.WorldToScreenPoint(neckAvatar.position) - cam.WorldToScreenPoint(spineAvatar.position);
			camAxis = camAxis.normalized;
			float dot = Vector2.Dot(new Vector2(-camAxis.y, camAxis.x), data.delta);
			model.Rotate(-Vector3.up * dot * rotateWeight);
		}
    }
   	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = MyUtility.FINGER_NONE;
		}
	}
}
