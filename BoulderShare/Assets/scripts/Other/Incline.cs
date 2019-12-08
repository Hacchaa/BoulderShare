using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Incline : MonoBehaviour {
	public AvatarControl ac;
	public Text inclineText;
	public Slider slider;

	// Use this for initialization
	void Start () {
		
	}
	
	public void SetInclineValue(){
		ac.SetIncline((int)slider.value*5);
		inclineText.text = "" + slider.value*5;
	}

	public int GetIncline(){
		return (int)slider.value*5;
	}

	public void SetIncline(int value){
		slider.value = value/5;
		ac.SetIncline(value);
		inclineText.text = "" + value;
	}
}
