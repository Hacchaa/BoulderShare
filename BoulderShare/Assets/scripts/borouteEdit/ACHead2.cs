using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ACHead2 : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private static int finger ;
	private const int FINGER_NONE = -10;
	public Camera cam;
	public Transform head = null;
	public Transform body = null;
	private float depth;
	private const float WEIGHT = 1.0f;
	// Use this for initialization
	void Start () {
		finger = FINGER_NONE;
		depth = 0.0f;
	}	

	void LateUpdate(){
			transform.position = head.position;
	}

	public void OnBeginDrag(PointerEventData data){
		if (finger == FINGER_NONE){
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
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			finger = FINGER_NONE;
		}
	}
}
