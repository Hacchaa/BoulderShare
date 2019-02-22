using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThreeDView : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
	private int[] eTouches;
	private const int FINGER_NONE = -10;
	private float prevLength;
	private Vector2 baseP1;
	private Vector2 baseP2;
	private bool isUpdate = false;
	private bool isMove = false;
	private bool isOperationDetermined = false;
	[SerializeField]
	private Transform camMoveTarget;
	[SerializeField] Transform camRotTarget;
	[SerializeField] private Camera cam;
	[SerializeField] private ThreeDWall threeDWall;
	private Bounds bounds;
	private const float CAMERA_DEPTH_LL = 2.0f;
	private const float CAMERA_DEPTH_UL = 12.0f;
	private const float WEIGHT = 0.2f;

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
	}

	public void OnDrag(PointerEventData data){
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
			camRotTarget.Rotate(0, data.delta.x * WEIGHT, 0);
			isUpdate = true;
			return;
		}

		float depth = Mathf.Abs(camMoveTarget.localPosition.z);
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
				cam.gameObject.transform.InverseTransformPoint(camRotTarget.position).z));
	    	Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f,
	    		cam.gameObject.transform.InverseTransformPoint(camRotTarget.position).z));

	    	camMoveTarget.Translate(wP1Old - wP1, Space.World);
	    	//camMoveTarget.position = wP1;

	    	//バウンド処理
	    	Vector3 bPos = camMoveTarget.localPosition;
			float height = bounds.size.y / 2f;
			float width = bounds.size.x / 2f;
			Debug.Log("bPos"+bPos);
	    	bPos.x = Mathf.Min(bPos.x, width);
	    	bPos.x = Mathf.Max(bPos.x, -width);
	    	bPos.y = Mathf.Min(bPos.y, height);
	    	bPos.y = Mathf.Max(bPos.y, -height);
			Debug.Log("bPosaf"+bPos);
	    	camMoveTarget.localPosition = bPos;

		}else{
			//prevLengthとlengthの比で拡大、縮小する
			if (prevLength > 0 && length > 0){
				if (!(depth <= CAMERA_DEPTH_LL && length / prevLength > 1) &&
					!(depth >= CAMERA_DEPTH_UL && length / prevLength < 1 )){
					
					camMoveTarget.Translate(
						0, 
						0, 
						camMoveTarget.localPosition.z * -(length / prevLength - 1));
					
					if (Mathf.Abs(camMoveTarget.localPosition.z) < CAMERA_DEPTH_LL){
			        	camMoveTarget.localPosition = new Vector3(
			        		camMoveTarget.localPosition.x, 
			        		camMoveTarget.localPosition.y, 
			        		-CAMERA_DEPTH_LL);
			        }else if (Mathf.Abs(camMoveTarget.localPosition.z) > CAMERA_DEPTH_UL){
			        	camMoveTarget.localPosition = new Vector3(
			        		camMoveTarget.localPosition.x, 
			        		camMoveTarget.localPosition.y, 
			        		-CAMERA_DEPTH_UL);
			        }
				}
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
	}
}
