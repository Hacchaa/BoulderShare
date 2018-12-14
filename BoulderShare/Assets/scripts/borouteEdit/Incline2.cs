using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Incline2 : MonoBehaviour {
	[SerializeField]
	private Text inclineText;
	[SerializeField]
	private Slider slider;
	[SerializeField]
	private EditPose ep;

	public int GetValue(){
		return (int)slider.value*5;
	}

	public void SetValue(int value){
		Debug.Log("incline2 setvalue");
		slider.value = value/5;
		inclineText.text = "" + value;
	}

	public void SetInclineValue(){
		ep.SetIncline(GetValue());
		inclineText.text = "" + GetValue();
	}
}
