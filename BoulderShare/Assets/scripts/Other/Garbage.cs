using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Garbage : MonoBehaviour , IDropHandler{

	// Use this for initialization
	void Start () {
		
	}
	
	public void OnDrop(PointerEventData data){
		//Debug.Log("OnDrop");
		if (data.pointerDrag != null && 
			(data.pointerDrag.tag == "Hold_Normal" || data.pointerDrag.tag == "Hold_Foot")){
			Destroy(data.pointerDrag);
		}
	}
}
