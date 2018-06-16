using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformWall3 : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private int[] eTouches;
	private float prevLength;
	private Camera cam;
	private Observer observer;
	private const float CAMERA_DEPTH_LL = 1.2f;
	private const float CAMERA_DEPTH_UL = 12.0f;
	
	// Use this for initialization
	void Start () {
		eTouches = new int[] {-1,-1};
		prevLength = -1;
		observer = GameObject.Find("Observer").GetComponent<Observer>();
		cam = observer.GetCamera();
	}	
	

	public void OnBeginDrag(PointerEventData data){
		if (eTouches[0] == -1){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == -1){
			eTouches[1] = data.pointerId;
		}

		if (eTouches[1] != -1){
			prevLength = -1;
		}
		observer.ReleaseFocus();
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

		
		Transform camTransform = cam.transform;
		float depth = Mathf.Abs(camTransform.position.z);
	
        if (eTouches[1] != -1){
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

        	camTransform.Translate(wP1Old - wP1);
        	Vector3 bPos = camTransform.position;

        	//バウンド処理
        	bPos.x = Mathf.Min(bPos.x, Observer.WALL_W/2);
        	bPos.x = Mathf.Max(bPos.x, -Observer.WALL_W/2);

        	bPos.y = Mathf.Min(bPos.y, Observer.WALL_H/2);
        	bPos.y = Mathf.Max(bPos.y, -Observer.WALL_H/2);

        	camTransform.position = bPos;
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (eTouches[0] == data.pointerId){
			eTouches[0] = eTouches[1];
			eTouches[1] = -1;
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = -1;
		}
	}

	public void OnPointerClick(PointerEventData data){
		observer.ReleaseFocus();
	}
}
