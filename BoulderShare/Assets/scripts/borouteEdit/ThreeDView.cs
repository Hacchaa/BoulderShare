using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ThreeDView : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
	public enum DragType{Normal, NoMove};
	private int[] eTouches;
	private const int FINGER_NONE = -10;
	private float prevLength;
	private Vector2 baseP1;
	private Vector2 baseP2;
	private bool isUpdate = false;
	private bool isMove = false;
	private bool isOperationDetermined = false;
	[SerializeField] private Camera cam;
	[SerializeField] private ThreeDWall threeDWall;
	private Bounds bounds;
	private const float WEIGHT = 0.2f;
	[SerializeField] private CameraManager cameraManager;
	private Action focusOutAction;

	private DragType dragType = DragType.Normal;

	// Use this for initialization
	void Awake () {
		prevLength = -1;
	}
	void Start () {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};
	}	

	void LateUpdate(){
		isUpdate = false;
	}

	public void SetDragType(DragType t){
		dragType = t;
	}


	public void SetFocusOutAction(Action action){
		focusOutAction = action;
	}

	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE){
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
			bounds = threeDWall.Get3DWallBounds();
		}

		if (focusOutAction != null){
			focusOutAction();
		}
	}

	public void OnDrag(PointerEventData data){
		if (dragType == DragType.Normal){
			DragNormal(data);
		}else if(dragType == DragType.NoMove){
			DragNoMove(data);
		}
	}

	public void DragNoMove(PointerEventData data){
		Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;

		//このイベントが発生した指が扱っている指かどうか
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

		//一本指の場合
		if (eTouches[1] == FINGER_NONE){
			//y軸に回転させる
			cameraManager.Rotate3D(0, data.delta.x * WEIGHT, 0);
			isUpdate = true;
			return;
		}

		float length = Vector2.Distance(p1, p2);

		//prevLengthとlengthの比で拡大、縮小する
		if (prevLength > 0 && length > 0){
			cameraManager.Zoom3D(-(length/prevLength - 1));
		}
		
		prevLength = length;
		isUpdate = true;

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

		//このイベントが発生した指が扱っている指かどうか
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

		//一本指の場合
		if (eTouches[1] == FINGER_NONE){
			//y軸に回転させる
			cameraManager.Rotate3D(0, data.delta.x * WEIGHT, 0);
			isUpdate = true;
			return;
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


		if(isMove){
				//壁を移動させる
				Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, 
					cam.gameObject.transform.InverseTransformPoint(cameraManager.GetRootWorldPos()).z));
		    	Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f,
		    		cam.gameObject.transform.InverseTransformPoint(cameraManager.GetRootWorldPos()).z));

		    	cameraManager.Translate3D(wP1Old - wP1, Space.World);
		    	cameraManager.Bounds3D(bounds);
		}else{
			//prevLengthとlengthの比で拡大、縮小する
			if (prevLength > 0 && length > 0){
				cameraManager.Zoom3D(-(length/prevLength - 1));
			}
			prevLength = length;
		}
		isUpdate = true;
    }

	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
			eTouches[0] = eTouches[1];
			eTouches[1] = FINGER_NONE;
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
		cameraManager.SetRootPosToMovePosWithFixedHierarchyPos();
	}
}
