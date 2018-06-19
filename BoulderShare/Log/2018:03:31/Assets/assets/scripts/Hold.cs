using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hold : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
	,IBeginDragHandler {
	private int finger;
	private Camera camera;
	private Observer observer;
	private GameObject child;
	private GameObject focus;
	private GameObject scaleMark;
	private GameObject removeMark;

	private static float R = 1.0f;

	// Use this for initialization
	void Start () {
		finger = -1;
		GameObject tmp = GameObject.Find("Observer");
		observer = tmp.GetComponent<Observer>();
		camera = tmp.GetComponent<Observer>().GetCamera();
		child = transform.Find("Hold_Scale").gameObject;
		focus = GameObject.Find("Canvas").transform.Find("Focus").gameObject;
		scaleMark = focus.transform.Find("Scale").gameObject;
		removeMark = focus.transform.Find("Remove").gameObject;
	}
	
	public void OnPointerDown(PointerEventData data){
		finger = data.pointerId;
/*
		observer.FocusObject(focus);
		Vector3 p1 = camera.WorldToScreenPoint(transform.position);
		//Vector3 p2 = camera.WorldToScreenPoint(trnasform.position.x - )
		focus.transform.position = p1;
		//focus.transform.SetParent(transform);

		float r = transform.localScale.x * R;


		//r += 50;
		//scaleMark.transform.Translate()
*/
	}

	public void OnBeginDrag(PointerEventData data){
		observer.ReleaseFocus();
	}
	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = camera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-camera.transform.position.z));
	        Vector3 pOld = camera.ScreenToWorldPoint(
	        	new Vector3(
	        		data.position.x - data.delta.x, 
	        		data.position.y - data.delta.y, 
	        		-camera.transform.position.z));

	        transform.Translate(p - pOld);
	    }
	}
	public void OnPointerUp(PointerEventData data){
		if (data.pointerId == finger){
			finger = -1;
		}
		observer.FocusObject(child);
	}
}
