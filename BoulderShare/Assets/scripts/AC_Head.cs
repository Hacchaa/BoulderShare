using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AC_Head : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	public Camera cam;
	public Transform head = null;
	public Transform body = null;
	public Transform basePos = null;
	private float depth;
	// Use this for initialization
	void Start () {
		finger = Observer.FINGER_NONE;
		depth = 0.0f;
	}	

	void LateUpdate(){
			transform.position = head.position;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			finger = data.pointerId;
			depth = transform.position.z;
		}
	}

	public void OnDrag(PointerEventData data){
		if (finger == data.pointerId){
			//float len = (cam.transform.position - target.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					depth - cam.transform.position.z));

			Vector3 localBody = basePos.InverseTransformPoint(body.position);
			Vector3 localP = basePos.InverseTransformPoint(p);
			Vector3 bodyToP = localP - localBody;

			float degXY = Vector2.Angle(
					Vector2.up,
					new Vector2(bodyToP.x, bodyToP.y));
			float degZY = Vector2.Angle(
					Vector2.up,
					new Vector2(bodyToP.z, bodyToP.y));

			degXY = Mathf.Min(degXY, 60.0f);
			degZY = Mathf.Min(degZY, 45.0f);

			if (bodyToP.x > 0.0f){
				degXY *= -1;
			}
			if (bodyToP.z < 0.0f){
				degZY *= -1;
			}
			
			body.localRotation = Quaternion.Euler(
				degZY,
				body.transform.localRotation.eulerAngles.y,
				degXY);
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
