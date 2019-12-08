using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AC_Spine : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	public Camera cam;
	public Transform spine = null;
	public Transform body = null;
	private static float weight = 0.5f;
	// Use this for initialization
	void Start () {
		finger = Observer.FINGER_NONE;
	}	

	void LateUpdate(){
			transform.position = spine.position;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			Vector3 r = body.localRotation.eulerAngles;
			float deg = r.y;
			if (deg >= 270.0f){
				deg -= 360.0f;
			}
			deg += data.delta.x * weight;
			deg = Mathf.Min(deg, 90.0f);
			deg = Mathf.Max(deg, -90.0f);

			body.localRotation = Quaternion.Euler(
				r.x,
				deg,
				r.z);
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
