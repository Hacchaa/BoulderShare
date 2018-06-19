using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformWall3 : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
	, IBeginDragHandler{
	private int[] eTouches;
	private float prevLength;
	private static float ZOOM_LOWER_LIMIT = 1.2f;
	private static float ZOOM_UPPER_LIMIT = 12.0f;
	private static float BOUNDS_X = 3.5f;
	private static float BOUNDS_Y = 2.5f;
	private Camera camera;
	private Observer observer;
	
	// Use this for initialization
	void Start () {
		eTouches = new int[] {-1,-1};
		prevLength = -1;
		observer = GameObject.Find("Observer").GetComponent<Observer>();
		camera = observer.GetCamera();

	}	
	

	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == -1){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == -1){
			eTouches[1] = data.pointerId;
		}

		if (eTouches[1] != -1){
			prevLength = -1;
		}
	}

	public void OnBeginDrag(PointerEventData data){
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

		
		Transform cTrans = camera.transform;
		float depth = cTrans.position.z;
		depth = Mathf.Abs(depth);
	
        if (eTouches[1] != -1){
        	//２点間の距離
			float length;
			//二つの指の軸毎の距離の絶対値
			length = Vector2.Distance(p1, p2);


			//prevLengthが設定さえている場合
			//prevLengthとlengthの比で拡大、縮小する
			if (prevLength > 0 && length > 0){

				if (!(depth <= ZOOM_LOWER_LIMIT && length / prevLength > 1) &&
					!(depth >= ZOOM_UPPER_LIMIT && length / prevLength < 1 )){
					
					cTrans.Translate(
						0, 
						0, 
						cTrans.position.z * -(length / prevLength - 1));

					if (Mathf.Abs(cTrans.position.z) < ZOOM_LOWER_LIMIT){
			        	cTrans.position = new Vector3(
			        		cTrans.position.x, 
			        		cTrans.position.y, 
			        		-ZOOM_LOWER_LIMIT);
			        }else if (Mathf.Abs(cTrans.position.z) > ZOOM_UPPER_LIMIT){
			        	cTrans.position = new Vector3(
			        		cTrans.position.x, 
			        		cTrans.position.y, 
			        		-ZOOM_UPPER_LIMIT);
			        }
				}
			}
			
			prevLength = length;

        }else{
        	Vector3 wP1 = camera.ScreenToWorldPoint(new Vector3(p1.x, p1.y, depth));
        	Vector3 wP1Old = camera.ScreenToWorldPoint(new Vector3(p1.x - dP1.x, p1.y - dP1.y, depth));

        	cTrans.Translate(wP1Old - wP1);
        	Vector3 bPos = cTrans.position;

        	//バウンド処理
        	bPos.x = Mathf.Min(bPos.x, BOUNDS_X);
        	bPos.x = Mathf.Max(bPos.x, -BOUNDS_X);

        	bPos.y = Mathf.Min(bPos.y, BOUNDS_Y);
        	bPos.y = Mathf.Max(bPos.y, -BOUNDS_Y);

        	cTrans.position = bPos;
	    }
	}

	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
			eTouches[0] = eTouches[1];
			eTouches[1] = -1;
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = -1;
		}
	}
}
