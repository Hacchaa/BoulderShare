using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace BoulderNotes{
public class MobilePaintController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler{
    [SerializeField] private unitycoder_MobilePaint.MobilePaint mobilePaint;
	[SerializeField] private RectTransform boundsArea;
	private int[] eTouches;
	private const int FINGER_NONE = -100;
	private float prevLength;
	private Vector2 baseP1;
	private Vector2 baseP2;
	private bool isUpdate = false;

	[SerializeField] private Camera cam;
	private const float WEIGHT = 0.2f;
    [SerializeField] private float perspectiveZoomSpeed = 0.05f;
    [SerializeField] private float maxPerspectiveZoom = 60f;
    [SerializeField] private float minPerspectiveZoom = 20f;
	private MeshFilter meshFilter;

    private enum TouchMode {None, Draw, Move};
    private TouchMode touchMode;
    private bool isDrawing ;
	// Use this for initialization
	void Awake () {
        touchMode = TouchMode.None;
	}
	void Start () {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};
		meshFilter = mobilePaint.GetComponent<MeshFilter>();
	}	

	void LateUpdate(){
		isUpdate = false;
	}

	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE && (touchMode == TouchMode.None || touchMode == TouchMode.Move)){
			eTouches[1] = data.pointerId;
		}
	}
    public void OnBeginDrag(PointerEventData data){
		if (touchMode != TouchMode.None){
			return ;
		}
        if (eTouches[0] == data.pointerId && eTouches[1] == FINGER_NONE){
            mobilePaint.OnBeginDrag(data);
            touchMode = TouchMode.Draw;
        }else if (eTouches[0] != FINGER_NONE && eTouches[1] != FINGER_NONE && (eTouches[0] == data.pointerId || eTouches[1] == data.pointerId)){
            touchMode = TouchMode.Move;
        }
    }

	public void OnDrag(PointerEventData data){
        if (touchMode == TouchMode.Move){
            DragNormal(data);
        }else if(touchMode == TouchMode.Draw){
			mobilePaint.OnDrag(data);
		}
		//MoveWall(data);
	}
	public void OnPointerUp(PointerEventData data){
		if (eTouches[0] == data.pointerId){
            if (eTouches[1] != FINGER_NONE){
                eTouches[0] = eTouches[1];
                eTouches[1] = FINGER_NONE;
            }else{
                eTouches[0] = FINGER_NONE;

				if (touchMode == TouchMode.Draw){
					mobilePaint.OnEndDrag(data);
				}
                touchMode = TouchMode.None;
            }
		}else if(eTouches[1] == data.pointerId){
			eTouches[1] = FINGER_NONE;
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


        //一本指の場合何もしない
        if (eTouches[1] == FINGER_NONE){
            return ;
        }
		


		//if(isMove){
            //壁を移動させる
            Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, 
                cam.gameObject.transform.InverseTransformPoint(mobilePaint.transform.position).z));
            Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f,
                cam.gameObject.transform.InverseTransformPoint(mobilePaint.transform.position).z));

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
			
			// If the camera is perspectivegraphic...
			if (!cam.orthographic)
			{
				// ... change the perspectivegraphic size based on the change in distance between the touches.
				cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
				
				// Make sure the perspectivegraphic size never drops below zero.
				cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minPerspectiveZoom, maxPerspectiveZoom); //Mathf.Max(cam.perspectivegraphicSize, 0.1f);
			}
			//clampする
			Bounds();
		//}
		isUpdate = true;
    }
	
	public void MoveWall(PointerEventData data){
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
			//return ;
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


        //一本指の場合何もしない
        if (eTouches[1] == FINGER_NONE){
            //return ;
        }
		
		p1 = data.position;
		dP1 = data.delta;

		//if(isMove){
		//壁を移動させる
		Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3(p1.x, p1.y, 
			cam.gameObject.transform.InverseTransformPoint(mobilePaint.transform.position).z));
		Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3(p1.x - dP1.x, p1.y - dP1.y, 
			cam.gameObject.transform.InverseTransformPoint(mobilePaint.transform.position).z));
		//Debug.Log("wp1 "+wP1.x+ " "+wP1.y+" "+wP1.z);
		//Debug.Log("wP1Old "+wP1Old.x+ " "+wP1Old.y+" "+wP1Old.z);
		cam.transform.Translate(wP1Old - wP1, Space.World);	
		Bounds();	
	}
	public void Bounds(){
		float canvasScaleFactor = CanvasResolutionManager.Instance.GetRatioOfPtToPx();
		float depth = cam.transform.InverseTransformPoint(mobilePaint.transform.position).z;
		Vector3 botLeft = new Vector3(boundsArea.offsetMin.x * canvasScaleFactor, boundsArea.offsetMin.y * canvasScaleFactor, depth);
		Vector3 topRight = new Vector3(Screen.width + boundsArea.offsetMax.x * canvasScaleFactor, Screen.height + boundsArea.offsetMax.y * canvasScaleFactor, depth);

		botLeft = cam.ScreenToWorldPoint(botLeft);
		topRight = cam.ScreenToWorldPoint(topRight);

		Mesh mesh = meshFilter.mesh;
		Vector3 wallSize = mesh.vertices[2] - mesh.vertices[0];
/*
		Debug.Log("botLeft:"+botLeft.x + " "+ botLeft.y+" "+botLeft.z);
		Debug.Log("topRight:"+topRight.x + " "+ topRight.y+" "+topRight.z);
		Debug.Log("mesh.vertices[0]:"+mesh.vertices[0].x + " "+ mesh.vertices[0].y+" "+mesh.vertices[0].z);
		Debug.Log("mesh.vertices[2]:"+mesh.vertices[2].x + " "+ mesh.vertices[2].y+" "+mesh.vertices[2].z);
		Debug.Log("wallSize:"+wallSize.x + " "+ wallSize.y+" "+wallSize.z);
*/
		float x = cam.transform.position.x;
		float y = cam.transform.position.y;
		float z = cam.transform.position.z;

		//float diff = x - botLeft.x;
		//Debug.Log("diffx:"+diff);
		bool isInnerBounds = Mathf.Abs(botLeft.x - topRight.x) > wallSize.x;
		if (isInnerBounds){
			if (botLeft.x > mesh.vertices[0].x){
				x = mesh.vertices[0].x + (x - botLeft.x);
			}else if (topRight.x < mesh.vertices[2].x){
				x = mesh.vertices[2].x - (topRight.x - x);
			}
		}else{
			if (botLeft.x < mesh.vertices[0].x){
				x = mesh.vertices[0].x + (x - botLeft.x);
			}else if (topRight.x > mesh.vertices[2].x){
				x = mesh.vertices[2].x - (topRight.x - x);
			}
		}

		//diff = y - botLeft.y;
		//Debug.Log("diffy:"+ diff);
		isInnerBounds = Mathf.Abs(botLeft.y - topRight.y) > wallSize.y;
		if (isInnerBounds){
			if (botLeft.y > mesh.vertices[0].y){
				y = mesh.vertices[0].y + (y - botLeft.y);
			}else if (topRight.y < mesh.vertices[2].y){
				y = mesh.vertices[2].y - (topRight.y - y);
			}
		}else{
			if (botLeft.y < mesh.vertices[0].y){
				y = mesh.vertices[0].y + (y - botLeft.y);
			}else if (topRight.y > mesh.vertices[2].y){
				y = mesh.vertices[2].y - (topRight.y - y);
			}
		}	

		cam.transform.position = new Vector3(x, y, z);
		//Debug.Log("cam.transform.position:"+cam.transform.position.x + " "+ cam.transform.position.y+" "+cam.transform.position.z);
	}
}
}