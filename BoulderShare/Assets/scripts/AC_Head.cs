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
	private const float WEIGHT = 1.0f;
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
			//カメラからみたxz面上に頭を動かす
			Vector3 camDir = cam.transform.position - cam.transform.parent.position;
			Vector2 camXZ = new Vector2(camDir.x, camDir.z);
			camXZ = camXZ.normalized;

			//bodyのy軸回転だけcamXZを回転
			float rotY = Mathf.Deg2Rad * body.localRotation.eulerAngles.y;
			camXZ = new Vector2(
				camXZ.x * Mathf.Cos(rotY) - camXZ.y * Mathf.Sin(rotY),
				camXZ.x * Mathf.Sin(rotY) + camXZ.y * Mathf.Cos(rotY));

			//Debug.Log("camDir"+camDir);

			float degXY = camXZ.y * WEIGHT * -1;
			float degZY = camXZ.x * WEIGHT * -1;

			if (data.delta.x > 0.0f){
				degXY *= -1;
				degZY *= -1;
			}

			float dX = body.localRotation.eulerAngles.z + degXY;
			float dY = body.localRotation.eulerAngles.y;
			float dZ = body.localRotation.eulerAngles.x + degZY;

			if (dX >= 270.0f){
				dX -= 360.0f;
			}
			if (dZ >= 270.0f){
				dZ -= 360.0f;
			}

			dX = Mathf.Min(dX, 60.0f);
			dX = Mathf.Max(dX, -60.0f);
			dZ = Mathf.Min(dZ, 45.0f);
			dZ = Mathf.Max(dZ, -45.0f);

			body.localRotation = Quaternion.Euler(
				dZ,
				body.localRotation.eulerAngles.y,
				dX);
			
/*
			//float len = (cam.transform.position - target.position).magnitude;
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					cam.gameObject.transform.InverseTransformPoint(gameObject.transform.position).z));

			Vector3 localBody = basePos.InverseTransformPoint(body.position);
			Vector3 localP = basePos.InverseTransformPoint(p);
			Vector3 bodyToP = localP - localBody;
			Debug.Log("bodyToP"+bodyToP);
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
			}*/
			/*
			body.localRotation = Quaternion.Euler(
				degZY,
				body.transform.localRotation.eulerAngles.y,
				degXY);*/
				/*
				body.localRotation = Quaternion.Euler(
				degZY,
				0.0f,
				degXY);*/
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = Observer.FINGER_NONE;
		}
	}
}
