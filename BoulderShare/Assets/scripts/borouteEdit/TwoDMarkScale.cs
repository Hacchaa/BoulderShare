using System.Collections;
using System.Collections.Generic;
using System;
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
	private Vector3 fixedScale = Vector3.one;
	[SerializeField] private Camera cam;
	[SerializeField] private CameraManager cManager;
	[SerializeField] private TwoDMark twoDMark;
	[SerializeField] private float defaultR = 0.2f;

	private static Action OnBeginDragAction;

	public static void AddOnBeginDragAction(Action a){
		if (a != null){
			OnBeginDragAction += a;
		}
	}
	public static void ResetOnBeginDragAction(){
		OnBeginDragAction = null;
	}

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
					-cManager.GetDepth()));

			offset = offset - twoDMark.transform.position;
			baseR = offset.magnitude;
			baseR = Mathf.Sqrt(baseR * baseR - Mathf.Pow(offset.x + offset.y, 2) / 2);
			
			offsetRate = twoDMark.transform.localScale.x;
			if (OnBeginDragAction != null){
				OnBeginDragAction();
			}
		}
	}
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z));

			p = p - twoDMark.transform.position - offset;
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
	        	twoDMark.transform.localScale = Vector3.one * offsetRate * rate;
	        }else if (offsetRate * rate <= SCALE_MIN){
	        	twoDMark.transform.localScale = Vector3.one * SCALE_MIN;
	        }else if (offsetRate * rate >= SCALE_MAX){
	        	twoDMark.transform.localScale = Vector3.one * SCALE_MAX;
	        }

	        FixScale();
	    }
	}
	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			finger = Observer.FINGER_NONE;
		}
	}

	public void FixScale(){
		float rate = cManager.Get2DZoomRate();
		Debug.Log("rate:"+rate);
		Vector3 lossy = transform.lossyScale;
		Vector3 local = transform.localScale;

		transform.localScale = new Vector3(
			local.x / lossy.x * fixedScale.x * rate,
			local.y / lossy.y * fixedScale.y * rate,
			local.z / lossy.z * fixedScale.z * rate);

		float r = defaultR * rate / twoDMark.transform.localScale.x;
		float parentR = twoDMark.GetR();
		Debug.Log("parentR:"+parentR);
		Debug.Log("r:"+r);
		float rad = 2*Mathf.PI * 45f / 360f;
		transform.localPosition = new Vector3(Mathf.Cos(rad)*(parentR + r), -(Mathf.Sin(rad)*(parentR + r)), 0.0f);
	}
/*
	public void FixScale(){
		transform.localScale = Vector3.one * cManager.Get2DDepthRate();

		float r = defaultR * cManager.Get2DDepthRate() / twoDMark.transform.localScale.x;
		float parentR = twoDMark.GetR();
		Debug.Log("parentR:"+parentR);
		Debug.Log("r:"+r);
		float rad = 2*Mathf.PI * 45f / 360f;
		transform.localPosition = new Vector3(Mathf.Cos(rad)*(parentR + r), -(Mathf.Sin(rad)*(parentR + r)), 0.0f);
	}*/
}
