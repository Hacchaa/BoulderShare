using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FinalIKView : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{
	private int[] eTouches;
	private const int FINGER_NONE = -10;
	[SerializeField] private Camera cam;
	private const float WEIGHT = 0.2f;

	// Use this for initialization
	void Awake () {
	}
	void Start () {
		eTouches = new int[] {FINGER_NONE, FINGER_NONE};
	}	

	public void OnPointerDown(PointerEventData data){
		if (eTouches[0] == FINGER_NONE){
			eTouches[0] = data.pointerId;
		}else if(eTouches[1] == FINGER_NONE){
			eTouches[1] = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		Vector2 p1, p2, dP1, dP2;
		p1 = p2 = dP1 = dP2 = Vector2.zero;

		//扱っている２本の指かどうか
		if (data.pointerId != eTouches[0] && data.pointerId != eTouches[1]){
			return ;
		}
		cam.transform.parent.Rotate(0, data.delta.x * WEIGHT, 0);
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
