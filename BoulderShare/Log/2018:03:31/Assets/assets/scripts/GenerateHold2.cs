using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateHold2 : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
	public Camera camera;
	public GameObject hold_Normal;
	public GameObject hold_Foot;
	private GameObject hold;
	private GameObject holds;
	private static float HOLD_RATE = 0.1f;
	private int finger ;
	private Observer observer;
	// Use this for initialization
	void Start () {
		observer = GameObject.Find("Observer").GetComponent<Observer>();
		holds = GameObject.Find("Holds");
		finger = -1;
	}
	
	public void OnPointerDown(PointerEventData data){
		GameObject target = null;
		if (finger == -1){
			if (gameObject.name == "Normal"){
				target = hold_Normal;
			}else if (gameObject.name =="Foot"){
				target = hold_Foot;
			}

			Vector3 p = camera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-camera.transform.position.z + target.transform.position.z));
			hold = Instantiate(target, p, Quaternion.identity, holds.transform);
			hold.transform.localScale = Vector3.one * HOLD_RATE * -camera.transform.position.z;

			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = camera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-camera.transform.position.z + hold.transform.position.z));
	        Vector3 pOld = camera.ScreenToWorldPoint(
	        	new Vector3(
	        		data.position.x - data.delta.x, 
	        		data.position.y - data.delta.y, 
	        		-camera.transform.position.z + hold.transform.position.z));

	        hold.transform.Translate(p - pOld);
	    }
	}

	public void OnPointerUp(PointerEventData data){
		if (data.pointerId == finger){
			finger = -1;
		}
		observer.FocusObject(hold.transform.Find("Hold_Scale").gameObject);
	}
}
