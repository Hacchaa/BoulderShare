using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TwoDWallImage : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler {
	private int[] eTouches;
	private float prevLength;
	private Bounds bounds;
	private bool isOn = false;

	[SerializeField]
	private Camera cam;
	[SerializeField]
	private TwoDWall twoDWall;

	private const float CAMERA_DEPTH_LL = 1.2f;
	private const float CAMERA_DEPTH_UL = 12.0f;
	private const float CAMERA_DEPTH_DEF = 10.0f;
	private const int FINGER_NONE = -10;


	// Use this for initialization
	void Awake () {
		prevLength = -1;
		eTouches = new int[] {FINGER_NONE,FINGER_NONE};
	}

	public void ResetCamPosAndDepth(){
		cam.gameObject.transform.position = 
			new Vector3(0.0f, 0.0f, -CAMERA_DEPTH_DEF);
	}


	public void OnBeginDrag(PointerEventData data){

		bounds = twoDWall.GetWallBounds();
		
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

		
		Transform camTransform = cam.transform;
		float depth = Mathf.Abs(camTransform.position.z);
	
        if (eTouches[1] != FINGER_NONE){
        	//２点間の距離
			float length = Vector2.Distance(p1, p2);

			//prevLengthが設定されてしまうている場合
			//prevLengthとlengthの比で拡大、縮小する
			if (prevLength > 0 && length > 0){

				if (!(depth <= CAMERA_DEPTH_LL && length / prevLength > 1) &&
					!(depth >= CAMERA_DEPTH_UL && length / prevLength < 1 )){
					
					camTransform.Translate(
						0, 
						0, 
						camTransform.position.z * -(length / prevLength - 1));

					if (Mathf.Abs(camTransform.position.z) < CAMERA_DEPTH_LL){
			        	camTransform.position = new Vector3(
			        		camTransform.position.x, 
			        		camTransform.position.y, 
			        		-CAMERA_DEPTH_LL);
			        }else if (Mathf.Abs(camTransform.position.z) > CAMERA_DEPTH_UL){
			        	camTransform.position = new Vector3(
			        		camTransform.position.x, 
			        		camTransform.position.y, 
			        		-CAMERA_DEPTH_UL);
			        }
				}
			}
			
			prevLength = length;

        }else{
        	Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3(p1.x, p1.y, depth));
        	Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3(p1.x - dP1.x, p1.y - dP1.y, depth));

        	//Debug.Log((wP1Old - wP1));
        	camTransform.Translate(wP1Old - wP1);
        	Vector3 bPos = camTransform.position;

        	//バウンド処理;
			float height = bounds.size.y;
			float width = bounds.size.x;
        	bPos.x = Mathf.Min(bPos.x, width/2);
        	bPos.x = Mathf.Max(bPos.x, -width/2);

        	bPos.y = Mathf.Min(bPos.y, height/2);
        	bPos.y = Mathf.Max(bPos.y, -height/2);

        	camTransform.position = bPos;
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

	public void OnPointerEnter(PointerEventData data){
		isOn = true;
	}

	public void OnPointerExit(PointerEventData data){
		isOn = false;
	}

	public bool IsOnPointerEnter(){
		return isOn;
	}
}
