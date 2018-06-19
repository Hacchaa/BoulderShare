using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CommentAdd : MonoBehaviour , IPointerClickHandler{
	public InputField input;
	// Use this for initialization
	void Start () {
		
	}
	
	public void OnPointerClick(PointerEventData data){
		input.ActivateInputField();
	}
}
