using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Garbage : MonoBehaviour , IPointerUpHandler{

	// Use this for initialization
	void Start () {
		
	}
	
	public void OnPointerUp(PointerEventData data){
		GameObject obj;

		obj = data.pointerDrag;
		if (obj.tag == "Hold"){
			Destroy(obj);
		}
	}
}
