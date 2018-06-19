using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldScale : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
	private int finger;
	private Transform parent;
	private Camera camera;
	private static float R = 0.5f;
	private static float SCALE_MIN = 0.12f;
	private static float SCALE_MAX = 1.5f;
	// Use this for initialization
	void Start () {
		finger = -1;;
		parent = transform.parent;
		camera = GameObject.Find("Observer").GetComponent<Observer>().GetCamera();
	}
	
	public void OnPointerDown(PointerEventData data){
		finger = data.pointerId;
	}
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = camera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-camera.transform.position.z));

			p = p - transform.parent.position;
		
	        //xとy大きい方を使用する
	        float length = Mathf.Max(Mathf.Abs(p.x), Mathf.Abs(p.y));

	        if (length / R > SCALE_MIN && length / R < SCALE_MAX){
	        	parent.localScale = Vector3.one * length / R;
	        }else if (length / R <= SCALE_MIN){
	        	parent.localScale = Vector3.one * SCALE_MIN;
	        }else if (length / R >= SCALE_MAX){
	        	parent.localScale = Vector3.one * SCALE_MAX;
	        }


	    }
	}
	public void OnPointerUp(PointerEventData data){
		if (data.pointerId == finger){
			finger = -1;
		}
	}

}
