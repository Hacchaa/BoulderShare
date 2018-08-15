using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenerateHold2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private Camera cam;
	public GameObject holdNPrefab;
	public GameObject holdFPrefab;
	private GameObject hold;
	private Transform holds;
	private Hold holdScript;
	private static float HOLD_RATE = 0.1f;
	private static int finger ;
	private static int num = 0;
	private const string GENTAG_NH = "Generate_NH";
	private const string GENTAG_FH = "Generate_FH";

	// Use this for initialization
	void Start () {
		cam = GameObject.Find("Observer").GetComponent<Observer>().GetCamera();
		holds = GameObject.Find("Wall").transform.Find("Holds");
		finger = Observer.FINGER_NONE;
	}

	public static void SetNum(int n){
		num = n;
	}
	
	public void OnBeginDrag(PointerEventData data){
		GameObject target = null;

		if (finger == Observer.FINGER_NONE){
			if (gameObject.tag == GENTAG_NH){
				target = holdNPrefab;
			}else if (gameObject.tag == GENTAG_FH){
				target = holdFPrefab;
			}
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z + target.transform.position.z));
			hold = Instantiate(target, p, Quaternion.identity, holds);
			hold.name = num+"";
			num++;
			holdScript = hold.GetComponent<Hold>();

			hold.transform.localScale = Vector3.one * HOLD_RATE * -cam.transform.position.z;

			finger = data.pointerId;
		}
	}

	public void OnDrag(PointerEventData data){
		if (data.pointerId == finger){
			Vector3 p = cam.ScreenToWorldPoint(
				new Vector3(
					data.position.x, 
					data.position.y, 
					-cam.transform.position.z + hold.transform.position.z));
	        hold.transform.position = p;
	    }
	}

	public void OnEndDrag(PointerEventData data){
		if (data.pointerId == finger){
			float x = Mathf.Abs(hold.transform.position.x);
			float y = Mathf.Abs(hold.transform.position.y);
			//wallの外にドロップした場合
			if (x > Observer.WALL_W / 2 || y > Observer.WALL_H / 2){
				Destroy(hold);
			}else{
				holdScript.Focus_P1();
				holdScript.SetSLN("Default");
			}
			finger = Observer.FINGER_NONE;

		}
	}
}
