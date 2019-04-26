using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TwoDMarkScale : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {
	static private int finger = Observer.FINGER_NONE;
	private Transform parent;
	private static float SCALE_MIN = 0.12f;
	private static float SCALE_MAX = 1.5f;
	private Vector3 offset ;
	private float baseR;
	private float offsetRate;
	[SerializeField] private Camera cam;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private Transform root;

	//rayをブロックして親に伝えないようにする
	public void OnPointerUp(PointerEventData data){

	}
	public void OnPointerDown(PointerEventData data){

	}
	public void OnBeginDrag(PointerEventData data){
		if (finger == Observer.FINGER_NONE){
			finger = data.pointerId;
			//タップしたスクリーン座標に交わるように y = -x から垂線を引いた交点の、原点からの距離(BaseR)を求める
			offset = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cManager.Get2DDepth()));

			offset = offset - root.position;
			baseR = offset.magnitude;
			baseR = Mathf.Sqrt(baseR * baseR - Mathf.Pow(offset.x + offset.y, 2) / 2);
			
			offsetRate = root.localScale.x;
		}
	}
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z));

			p = p - root.position - offset;
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
	        	root.localScale = Vector3.one * offsetRate * rate;
	        }else if (offsetRate * rate <= SCALE_MIN){
	        	root.localScale = Vector3.one * SCALE_MIN;
	        }else if (offsetRate * rate >= SCALE_MAX){
	        	root.localScale = Vector3.one * SCALE_MAX;
	        }


	    }
	}
	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = Observer.FINGER_NONE;
		}
	}

}
