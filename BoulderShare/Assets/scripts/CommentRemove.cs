using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommentRemove : MonoBehaviour , IPointerClickHandler{

	// Use this for initialization
	void Start () {
		
	}
	
	public void OnPointerClick(PointerEventData data){
		GameObject.Destroy(transform.parent.parent.gameObject);
	}
}
