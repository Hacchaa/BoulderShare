using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace BoulderNotes{
public class MobilePaintController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler{
    [SerializeField] private unitycoder_MobilePaint.MobilePaint mobilePaint;
	private int[] eTouches;
	private const int FINGER_NONE = -10;
	private float prevLength;
	private Vector2 baseP1;
	private Vector2 baseP2;
	private bool isUpdate = false;
	private bool isMove = false;
	private bool isOperationDetermined = false;
	[SerializeField] private Camera cam;
	private const float WEIGHT = 0.2f;
    [SerializeField] private float orthoZoomSpeed = 0.1f;
    [SerializeField] private float maxOrthoZoom = 200f;
    [SerializeField] private float minOrthoZoom = 10f;

    private enum TouchMode {None, Draw, Move};
    private TouchMode touchMode;
    private bool isDrawing ;
	// Use this for initialization
	void Awake () {
		prevLength = -1;
        touchMode = TouchMode.None;
	}
	void Start () {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};
	}	

	void LateUpdate(){
		isUpdate = false;
	}

	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
            mobilePaint.RegisterFingerID(data.pointerId);
            touchMode = TouchMode.Draw;
		}else if(eTouches[1] == FINGER_NONE && (touchMode == TouchMode.None || touchMode == TouchMode.Move)){
			eTouches[1] = data.pointerId;
			prevLength = -1;
			isOperationDetermined = false;
			//扱っている指の情報を取得する
			foreach(Touch touch in Input.touches){
				if (touch.fingerId == eTouches[0]){
					baseP1 = touch.position;
				}else if (touch.fingerId == eTouches[1]){
					baseP2 = touch.position;
				}
			}

            //mobilePaint.ClearFingerID();
		}
	}
    public void OnBeginDrag(PointerEventData data){
        if (eTouches[0] == data.pointerId && eTouches[1] == FINGER_NONE){
            mobilePaint.RegisterFingerID(data.pointerId);
            touchMode = TouchMode.Draw;
        }else if (eTouches[0] != FINGER_NONE && eTouches[1] != FINGER_NONE && (eTouches[0] == data.pointerId || eTouches[1] == data.pointerId)){
            touchMode = TouchMode.Move;
        }
    }

	public void OnDrag(PointerEventData data){
        if (touchMode == TouchMode.Move){
            DragNormal(data);
        }
	}

	public void DragNormal(PointerEventData data){
		Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;
/*
		//扱っている２本の指かどうか
		if (data.pointerId != eTouches[0] && data.pointerId != eTouches[1]){
			return ;
		}
		cameras.transform.parent.Rotate(0, data.delta.x * WEIGHT, 0);

		return ;*/

		//data.pointerIdが現在扱っている指かどうか
		if (isUpdate || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
			return ;
		}

		//扱っている指の情報を取得する
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == eTouches[0]){
				p1 = touch.position;
				dP1 = touch.deltaPosition;
			}else if (touch.fingerId == eTouches[1]){
				p2 = touch.position;
				dP2 = touch.deltaPosition;
			}
		}

		float length = Vector2.Distance(p1, p2);

		if(!isOperationDetermined){
			Vector2 vecP1, vecP2;
			vecP1 = p1 - baseP1;
			vecP2 = p2 - baseP2;
			//Debug.Log("vecp1magnitude "+ vecP1.magnitude + ", vecp2magnitude "+ vecP2.magnitude);
			if(vecP1.magnitude < 10 || vecP2.magnitude < 10){
				return ;
			}
			//Debug.Log(Vector2.Angle(vecP1, vecP2));
			if(Vector2.Angle(vecP1, vecP2) < 80.0f){
				isMove = true;
			}else{
				isMove = false;
			}
			isOperationDetermined = true;
		}

        //一本指の場合何もしない
        if (eTouches[1] == FINGER_NONE){
            return ;
        }


		//if(isMove){
            //壁を移動させる
            Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, 
                cam.gameObject.transform.InverseTransformPoint(cam.transform.position).z));
            Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f,
                cam.gameObject.transform.InverseTransformPoint(cam.transform.position).z));

            cam.transform.Translate(wP1Old - wP1, Space.World);
		//}else{
	
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = p1 - dP1;
			Vector2 touchOnePrevPos = p2 - dP2;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (p1 - p2).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			
			// If the camera is orthographic...
			if (cam.orthographic)
			{
				// ... change the orthographic size based on the change in distance between the touches.
				cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
				
				// Make sure the orthographic size never drops below zero.
				cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoZoom, maxOrthoZoom); //Mathf.Max(cam.orthographicSize, 0.1f);
			}
		//}
		isUpdate = true;
    }

	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
            if (eTouches[1] != FINGER_NONE){
                eTouches[0] = eTouches[1];
                eTouches[1] = FINGER_NONE;
            }else{
                eTouches[0] = FINGER_NONE;
                mobilePaint.ClearFingerID();
                touchMode = TouchMode.None;
            }
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
    }
}
}