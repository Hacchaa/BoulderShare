using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformWall3 : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler{
	private int[] eTouches;
	private float prevLength;
	private Camera curCamera;
	private Observer observer;
	
	// Use this for initialization
	void Start () {
		eTouches = new int[] {-1,-1};
		prevLength = -1;
		observer = GameObject.Find("Observer").GetComponent<Observer>();
		curCamera = observer.GetCamera();

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

		
		Transform cTrans = curCamera.transform;
		float depth = cTrans.position.z;
		depth = Mathf.Abs(depth);
	
        if (eTouches[1] != -1){
        	//２点間の距離
			float length;
			float zoom_LL = Observer.CAMERA_DEPTH_LL;
			float zoom_UL = Observer.CAMERA_DEPTH_UL;
			//二つの指の軸毎の距離の絶対値
			length = Vector2.Distance(p1, p2);


			//prevLengthが設定さえている場合
			//prevLengthとlengthの比で拡大、縮小する
			if (prevLength > 0 && length > 0){

				if (!(depth <= zoom_LL && length / prevLength > 1) &&
					!(depth >= zoom_UL && length / prevLength < 1 )){
					
					cTrans.Translate(
						0, 
						0, 
						cTrans.position.z * -(length / prevLength - 1));

					if (Mathf.Abs(cTrans.position.z) < zoom_LL){
			        	cTrans.position = new Vector3(
			        		cTrans.position.x, 
			        		cTrans.position.y, 
			        		-zoom_LL);
			        }else if (Mathf.Abs(cTrans.position.z) > zoom_UL){
			        	cTrans.position = new Vector3(
			        		cTrans.position.x, 
			        		cTrans.position.y, 
			        		-zoom_UL);
			        }
				}
			}
			
			prevLength = length;

        }else{
        	float width = Observer.WALL_W;
        	float height = Observer.WALL_H;

        	Vector3 wP1 = curCamera.ScreenToWorldPoint(new Vector3(p1.x, p1.y, depth));
        	Vector3 wP1Old = curCamera.ScreenToWorldPoint(new Vector3(p1.x - dP1.x, p1.y - dP1.y, depth));

        	cTrans.Translate(wP1Old - wP1);
        	Vector3 bPos = cTrans.position;

        	//バウンド処理
        	bPos.x = Mathf.Min(bPos.x, width/2);
        	bPos.x = Mathf.Max(bPos.x, -width/2);

        	bPos.y = Mathf.Min(bPos.y, height/2);
        	bPos.y = Mathf.Max(bPos.y, -height/2);

        	cTrans.position = bPos;
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
