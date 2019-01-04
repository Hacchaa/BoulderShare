using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThreeDView : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private int[] eTouches;
	private const int FINGER_NONE = -10;
	private float prevLength;
	[SerializeField]
	private GameObject cameras;
	private const float CAMERA_DEPTH_LL = 2.0f;
	private const float CAMERA_DEPTH_UL = 12.0f;
	private const float WEIGHT = 0.5f;

	// Use this for initialization
	void Awake () {
		prevLength = -1;
	}
	void Start () {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};
	}	

	public void OnBeginDrag(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
		}

		if (eTouches[1] != FINGER_NONE){
			prevLength = -1;
		}
	}

	public void OnDrag(PointerEventData data){
		Vector2 p1, p2, dP1;
		p1 = p2 = dP1 = Vector2.zero;

		//扱っている２本の指かどうか
		if (data.pointerId != eTouches[0] && data.pointerId != eTouches[1]){
			return ;
		}

		//扱っている指の情報を取得する
		foreach(Touch touch in Input.touches){
			if (touch.fingerId == eTouches[0]){
				p1 = touch.position;
				dP1 = touch.deltaPosition;
			}else if (touch.fingerId == eTouches[1]){
				p2 = touch.position;
			}
		}

		//マウスの左クリックの場合
		if (data.pointerId == -1){
			p1 = data.position;
			dP1 = data.delta;
		}

		
		Transform camTransform = cameras.transform;
		float depth = Mathf.Abs(camTransform.localPosition.z);
	
        if (eTouches[1] != FINGER_NONE){

        	//２点間の距離
			float length = Vector2.Distance(p1, p2);

			//prevLengthが設定さえている場合
			//prevLengthとlengthの比で拡大、縮小する
			if (prevLength > 0 && length > 0){

				if (!(depth <= CAMERA_DEPTH_LL && length / prevLength > 1) &&
					!(depth >= CAMERA_DEPTH_UL && length / prevLength < 1 )){
					
					camTransform.Translate(
						0, 
						0, 
						camTransform.localPosition.z * -(length / prevLength - 1));

					if (Mathf.Abs(camTransform.localPosition.z) < CAMERA_DEPTH_LL){
			        	camTransform.localPosition = new Vector3(
			        		camTransform.localPosition.x, 
			        		camTransform.localPosition.y, 
			        		-CAMERA_DEPTH_LL);
			        }else if (Mathf.Abs(camTransform.localPosition.z) > CAMERA_DEPTH_UL){
			        	camTransform.localPosition = new Vector3(
			        		camTransform.localPosition.x, 
			        		camTransform.localPosition.y, 
			        		-CAMERA_DEPTH_UL);
			        }
				}
			}
			
			prevLength = length;

        }else{
        	//y軸に回転させる
			camTransform.parent.Rotate(0, data.delta.x * WEIGHT, 0);
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (eTouches[0] == data.pointerId){
			eTouches[0] = eTouches[1];
			eTouches[1] = FINGER_NONE;
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
		}
	}
}
