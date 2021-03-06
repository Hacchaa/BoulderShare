﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneFocusElem : MonoBehaviour , IPointerEnterHandler{
	private SceneFocus sf;
	private Image image;
	// Use this for initialization
	
	void Awake(){
		image = GetComponent<Image>();
	}


	void Start () {
		sf = transform.parent.gameObject.GetComponent<SceneFocus>();
	}

	
	public void OnPointerEnter(PointerEventData data){
		if (Observer.currentPhase == (int)Observer.Phase.SCENE_EDIT){
			string name = data.pointerEnter.name;
			//Debug.Log(name);
			if (name == "RightHand"){
				sf.SetChoice((int)AvatarControl.BODYS.RH);
			}else if (name == "LeftHand"){
				sf.SetChoice((int)AvatarControl.BODYS.LH);
			}else if (name == "RightFoot"){
				sf.SetChoice((int)AvatarControl.BODYS.RF);
			}else if (name == "LeftFoot"){
				sf.SetChoice((int)AvatarControl.BODYS.LF);
			}
		}
	}
	public void Emphasis(){
		image.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
	}

	public void DeEmphasis(){
		image.color = Color.white;
	}
}
