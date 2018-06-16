using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragFMenu : MonoBehaviour, IEndDragHandler, IBeginDragHandler{
	public HScenes hs;
	private Transform target;
	private int finger ;
	private float startX ;
	private static float THRESHOLD = 5.0f;
	// Use this for initialization
	void Start () {
		finger = -1;
		target = transform.Find("Viewport").Find("Content");
	}
	
	public void OnBeginDrag(PointerEventData data){
		//Debug.Log("OnBeginDrag");
		if (finger == -1){
			startX = target.position.x;
			finger = data.pointerId;
		}
	}

	public void OnEndDrag(PointerEventData data){
		if (finger == data.pointerId){
			float endX = target.position.x;
			//Debug.Log("endX:"+endX+ " startX:"+startX);
			if (startX - endX > THRESHOLD){
				//Debug.Log("next");
				hs.NextScene2();
			}else if (endX - startX > THRESHOLD){
				//Debug.Log("prev");
				hs.PrevScene2();
			}
			finger = -1;
		}
	}
}
