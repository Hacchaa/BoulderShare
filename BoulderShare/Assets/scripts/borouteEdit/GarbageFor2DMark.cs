using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GarbageFor2DMark : MonoBehaviour , IDropHandler{
	[SerializeField] private EditWallMark editWallMark;
	[SerializeField] private TwoDWallMarks twoDWallMarks;
	
	public void OnDrop(PointerEventData data){
		//Debug.Log("OnDrop");
		if (data.pointerDrag != null && data.pointerDrag.tag == "2DMark"){
			GameObject obj = data.pointerDrag;
			twoDWallMarks.DeleteMark(obj.name);
			Destroy(obj);
			editWallMark.CloseMarkOptions();
		}
	}
}
