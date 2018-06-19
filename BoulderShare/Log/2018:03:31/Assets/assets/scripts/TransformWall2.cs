using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransformWall2 : MonoBehaviour{
	//public float speed = 0.1f;
	private int[] eTouches;
	private float prevLength;
	private static int ZOOM_LOWER_LIMIT = 1;
	private static int ZOOM_UPPER_LIMIT = 12;
	private static int BOUNDS_X = 3;
	private static int BOUNDS_Y = 2;
	private Camera camera;
	// Use this for initialization
	void Start () {
		eTouches = new int[] {-1,-1};
		prevLength = -1;
		camera = GetComponent<Camera>();
	}
/*	
	void Update(){
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
			Debug.Log("1");
			RaycastHit hit;
			Ray ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
			//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)){
				Debug.Log("2");

				if (old != Vector2.zero){
					Camera.main.transform.Translate(
						old.x - hit.point.x,
						old.y - hit.point.y,
						0);
				}
				old = hit.point;
			}
			
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).DeltaPosition;

            // Move object across XY plane
            Camera.main.transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
        
        }else if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
        	old = Vector2.zero;
        }
	}*/

	void Update(){
		if (Input.touchCount > 0){
			//扱っている指の情報を取得する
			Vector2 p1, p2, dP1;
			p1 = p2 = dP1 = Vector2.zero;

			foreach(Touch touch in Input.touches){
				if (touch.fingerId == eTouches[0]){
					p1 = touch.position;
					dP1 = touch.deltaPosition;
					continue;
				}else if (touch.fingerId == eTouches[1]){
					p2 = touch.position;
					continue;
				}

				//空きがある場合
				if (touch.phase == TouchPhase.Began && !isFullTouch()){
					Ray ray = camera.ScreenPointToRay(touch.position);
					//Debug.DrawLine(ray.origin, ray.direction * 100, Color.yellow, 1, true);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, Mathf.Infinity)){
						if(eTouches[0] == -1){
							eTouches[0] = touch.fingerId;
							p1 = touch.position;
						}else{
							eTouches[1] = touch.fingerId;
							p2 = touch.position;
						}
					}
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
					}
				}
				
				prevLength = length;
	        }
	        else if(eTouches[0] != -1){
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
	        	/*
		        // Move object across XY plane
	            camera.transform.Translate(
	            	-dP1.x * speed * depth, 
	            	-dP1.y * speed * depth,
	            	0);*/


	        }

            //後処理
            foreach(Touch touch in Input.touches){
            	if (touch.phase == TouchPhase.Ended){
            		if (touch.fingerId == eTouches[0]){
            			eTouches[0] = eTouches[1];
            			eTouches[1] = -1;
            			prevLength = -1;
            		}else if(touch.fingerId == eTouches[1]){
            			eTouches[1] = -1;
            			prevLength = -1;
            		}
            	}
            }
			/*
			if (Input.GetTouch(0).phase == TouchPhase.Moved) {	
				float depth = camera.transform.position.z;
				depth = Mathf.Abs(depth);

	            // Get movement of the finger since last frame
	            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

	            // Move object across XY plane
	            camera.transform.Translate(
	            	-touchDeltaPosition.x * speed * depth, 
	            	-touchDeltaPosition.y * speed * depth,
	            	0);
	      	}*/

	    }
	}

	private bool isEffectiveTouch(Touch touch){
		for(int i = 0 ; i < eTouches.Length ; i++){
			if (touch.fingerId == eTouches[i]){
				return true;
			}
		}
		return false;
	}
	private bool isFullTouch(){
		return eTouches[1] != -1; 
	}
}
