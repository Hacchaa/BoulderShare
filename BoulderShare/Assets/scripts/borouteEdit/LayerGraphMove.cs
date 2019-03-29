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
	[SerializeField] private Vector3 initPos;

	private const int FINGER_NONE = -10;
    private const float CAMERA_DEPTH_UB = -1.0f;
    private const float CAMERA_DEPTH_LB = -15.0f;
    private const float CAMERA_DEPTH_DEF = -6.5f;

	// Use this for initialization
	void Awake () {
		prevLength = -1;
		eTouches = new int[] {FINGER_NONE,FINGER_NONE};
	}

	public void IgnoreEvents(){
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	public void AcceptEvents(){
		gameObject.layer = LayerMask.NameToLayer("LayerGraph");
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

		float depth = Mathf.Abs(GetDepth());
		float length = Vector2.Distance(p1, p2);
	
		Vector3 wP1 = cam.ScreenToWorldPoint(new Vector3((p1.x + p2.x) / 2.0f, (p1.y + p2.y) / 2.0f, depth));
    	Vector3 wP1Old = cam.ScreenToWorldPoint(new Vector3((p1.x - dP1.x + p2.x - dP2.x) / 2.0f, (p1.y - dP1.y + p2.y - dP2.y) / 2.0f, depth));


    	Translate(wP1Old - wP1);
    	Bounds(bounds, transform.position.x, transform.position.y);

		//prevLengthが設定されてしまうている場合
		//prevLengthとlengthの比で拡大、縮小する
		if (prevLength > 0 && length > 0){
			Zoom(-(length/prevLength - 1));
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


	public void Translate(Vector3 v, Space relativeTo= Space.Self){
        cam.transform.Translate(v, relativeTo);
    }

    public float GetDepth(){
        return cam.transform.localPosition.z;
    }

    public void Bounds(Vector2 size, float baseX = 0, float baseY = 0){
        Vector3 p = cam.transform.localPosition;
        float height = size.y;
        float width = size.x;
        p.x = Mathf.Clamp(p.x, baseX-width/2f, baseX+width/2f);
        p.y = Mathf.Clamp(p.y, baseY-height/2f, baseY+height/2f);

        cam.transform.localPosition = p;
    }

    public void Zoom(float r){
        Vector3 p = cam.transform.localPosition;
        float depth = Mathf.Clamp(p.z + p.z * r, CAMERA_DEPTH_LB, CAMERA_DEPTH_UB);
        cam.transform.localPosition = new Vector3(p.x, p.y, depth);

    }

    public void ResetCamPosAndDepth(){
        cam.transform.localPosition = initPos;
    }
    public void SetCameraActive(bool b){
    	cam.gameObject.SetActive(b);
    }
}
