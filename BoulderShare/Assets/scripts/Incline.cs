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
		ac.SetIncline((int)slider.value);
		inclineText.text = "" + slider.value;
	}
}
