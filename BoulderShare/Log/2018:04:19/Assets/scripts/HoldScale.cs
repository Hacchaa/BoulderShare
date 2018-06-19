using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldScale : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private int finger;
	private Transform parent;
	private Camera curCamera;
	private static float SCALE_MIN = 0.12f;
	private static float SCALE_MAX = 1.5f;
	private Vector3 offset ;
	private float baseR;
	private float offsetRate;

	// Use this for initialization
	void Start () {
		finger = -1;;
		parent = transform.parent;
		curCamera = GameObject.Find("Observer").GetComponent<Observer>().GetCamera();
	}
	
	public void OnBeginDrag(PointerEventData data){
		if (finger == -1){

			finger = data.pointerId;
			offset = curCamera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-curCamera.transform.position.z));

			offset = offset - parent.position;
			baseR = offset.magnitude;
			baseR = Mathf.Sqrt(baseR * baseR - Mathf.Pow(offset.x + offset.y, 2) / 2);
			
			offsetRate = parent.localScale.x;
		}
	}
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = curCamera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-curCamera.transform.position.z));

			p = p - parent.position - offset;
			float r = p.magnitude;
			r = Mathf.Sqrt(r * r - Mathf.Pow(p.x + p.y, 2) / 2);
			float rate ;

			//縮小条件
			if (p.x < p.y){
				rate = baseR / (baseR + r);
			}else{
				rate = (baseR + r) / baseR;
			}

	        if (offsetRate * rate > SCALE_MIN && offsetRate * rate < SCALE_MAX){
	        	parent.localScale = Vector3.one * offsetRate * rate;
	        }else if (offsetRate * rate <= SCALE_MIN){
	        	parent.localScale = Vector3.one * SCALE_MIN;
	        }else if (offsetRate * rate >= SCALE_MAX){
	        	parent.localScale = Vector3.one * SCALE_MAX;
	        }


	    }
	}
	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = -1;
		}
	}

}
