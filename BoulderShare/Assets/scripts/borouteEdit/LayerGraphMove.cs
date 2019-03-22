using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LayerGraphMove : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
	private int[] eTouches;
	private float prevLength;
	[SerializeField] private Vector2 bounds = new Vector2(10.0f, 10.0f);
	[SerializeField] private Camera cam;
	[SerializeField] private CameraManager cameraManager;
	private const int FINGER_NONE = -10;

	// Use this for initialization
	void Awake () {
		prevLength = -1;
		eTouches = new int[] {FINGER_NONE,FINGER_NONE};
	}

	public void IgnoreEvents(){
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	public void AcceptEvents(){
		gameObject.layer = LayerMask.NameToLayer("2D");
	}


	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
			prevLength = -1;
		}
	}

	public void OnDrag(PointerEventData data){
		Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;

		//扱っている２本の指かどうか
		if (eTouches[1] == FINGER_NONE || (data.pointerId != eTouches[0] && data.pointerId != eTouches[1])){
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

		float depth = Mathf.Abs(cameraManager.Get2DDepth());
		float length = Vector2.Distance(p1, p2);
	
		Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, depth));
    	Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f, depth));

    	cameraManager.Translate2D(wP1Old - wP1);
    	cameraManager.Bounds2D(bounds, transform.position.x, transform.position.y);

		//prevLengthが設定されてしまうている場合
		//prevLengthとlengthの比で拡大、縮小する
		if (prevLength > 0 && length > 0){
			cameraManager.Zoom2D(-(length/prevLength - 1));
		}
		prevLength = length;
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
