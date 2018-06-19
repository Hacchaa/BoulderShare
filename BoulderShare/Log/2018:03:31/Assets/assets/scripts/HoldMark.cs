using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class HoldMark : MonoBehaviour ,IPointerClickHandler{
	public void OnPointerClick(PointerEventData data){
		RectTransform rt = GetComponent<RectTransform>();
		rt.pivot = new Vector2(0, 0);
	}
}
