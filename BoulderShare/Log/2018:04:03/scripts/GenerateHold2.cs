using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateHold2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private Camera curCamera;
	public GameObject hold_Normal;
	public GameObject hold_Foot;
	private GameObject hold;
	private GameObject holds;
	private static float HOLD_RATE = 0.1f;
	private static int finger = -1;

	// Use this for initialization
	void Start () {
		curCamera = GameObject.Find("Observer").GetComponent<Observer>().GetCamera();
		holds = GameObject.Find("Holds");
	}
	
	public void OnBeginDrag(PointerEventData data){
		GameObject target = null;
		if (finger == -1){
			if (gameObject.name == "Normal"){
				target = hold_Normal;
			}else if (gameObject.name =="Foot"){
				target = hold_Foot;
			}

			Vector3 p = curCamera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-curCamera.transform.position.z + target.transform.position.z));
			hold = Instantiate(target, p, Quaternion.identity, holds.transform);
			hold.transform.localScale = Vector3.one * HOLD_RATE * -curCamera.transform.position.z;

			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = curCamera.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-curCamera.transform.position.z + hold.transform.position.z));
	        hold.transform.position = p;
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			float width = Observer.WALL_W;
			float height = Observer.WALL_H;
			float x = Mathf.Abs(hold.transform.position.x);
			float y = Mathf.Abs(hold.transform.position.y);
			//wallの外にドロップした場合
			if (x > width/2 || y > height/2){
				Destroy(hold);
			}else{
				hold.GetComponent<Hold>().SetScalePos();
				hold.GetComponent<Renderer>().sortingLayerName = "Default";
			}
			finger = -1;

		}
	}
}
