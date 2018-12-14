using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GarbageFor2DMark : MonoBehaviour , IDropHandler{
	[SerializeField]
	private EditWallMark editWallMark;
	// Use this for initialization
	void Start () {
		
	}
	
	public void OnDrop(PointerEventData data){
		//Debug.Log("OnDrop");
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			Destroy(data.pointerDrag);
			editWallMark.CloseMarkOptions();
		}
	}
}
