using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InclineSetter : MonoBehaviour {
	[SerializeField]
	private TextMeshProUGUI inclineText;
	[SerializeField]
	private Slider inclineSlider;
	[SerializeField]
	private ThreeDWall threeDWall;

	public int GetValue(){
		return (int)inclineSlider.value*5;
	}

	public void SyncInclineValue(){
		float value = threeDWall.GetIncline();
		inclineSlider.value = value/5;
		inclineText.text = "" + value;
	}

	public void SetInclineValueFromSlider(){
		threeDWall.SetIncline(GetValue());
		inclineText.text = "" + GetValue();
	}
}
